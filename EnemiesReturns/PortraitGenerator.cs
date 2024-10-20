﻿using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit.Pillar;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Spitter;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine;
using RoR2.UI;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    public class PortraitGenerator
    {
        [ConCommand(commandName = "returns_body_generate_portraits", flags = ConVarFlags.None, helpText = "Generates portraits for all EnemiesReturns bodies.")]
        private static void CCBodyGeneratePortraits(ConCommandArgs args)
        {
            var addressable = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/IconGenerator.prefab").WaitForCompletion();
            if (addressable)
            {
                var modelPanel = addressable.GetComponentInChildren<ModelPanel>();
                if (modelPanel)
                {
                    modelPanel.modelPostProcessVolumePrefab = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
            }

            RoR2Application.instance.StartCoroutine(GeneratePortraits(args.TryGetArgBool(0) ?? false));
        }

        private static IEnumerator GeneratePortraits(bool forceRegeneration)
        {
            Debug.Log("Starting portrait generation.");
            ModelPanel modelPanel = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/IconGenerator")).GetComponentInChildren<ModelPanel>();
            modelPanel.renderSettings = new RenderSettingsState
            {
                ambientIntensity = 1f,
                ambientLight = Color.white,
                ambientMode = AmbientMode.Flat,
                ambientGroundColor = Color.white
            };
            yield return new WaitForEndOfFrame();
            modelPanel.BuildRenderTexture();
            yield return new WaitForEndOfFrame();
            yield return GeneratePortrait(modelPanel, SpitterFactory.SpitterBody);
            yield return GeneratePortrait(modelPanel, ColossusFactory.ColossusBody);
            yield return GeneratePortrait(modelPanel, IfritFactory.IfritBody);
            yield return GeneratePortrait(modelPanel, IfritPillarFactory.Enemy.IfritPillarBody);
            yield return GeneratePortrait(modelPanel, Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBody.prefab").WaitForCompletion());
            UnityEngine.Object.Destroy(modelPanel.transform.root.gameObject);
            Debug.Log("Portrait generation complete.");
        }

        private static IEnumerator GeneratePortrait(ModelPanel modelPanel, GameObject gameObject)
        {
            CharacterBody characterBody = gameObject.GetComponent<CharacterBody>();
            if ((bool)characterBody)
            {
                float num = 1f;
                try
                {
                    Debug.LogFormat("Generating portrait for {0}", gameObject.name);
                    modelPanel.modelPrefab = gameObject.GetComponent<ModelLocator>()?.modelTransform.gameObject;
                    //modelPanel.modelPostProcessVolumePrefab = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cube);
                    modelPanel.SetAnglesForCharacterThumbnail(setZoom: true);
                    PrintController printController;
                    if ((object)(printController = modelPanel.modelPrefab?.GetComponentInChildren<PrintController>()) != null)
                    {
                        num = Mathf.Max(num, printController.printTime + 1f);
                    }
                    TemporaryOverlay temporaryOverlay;
                    if ((object)(temporaryOverlay = modelPanel.modelPrefab?.GetComponentInChildren<TemporaryOverlay>()) != null)
                    {
                        num = Mathf.Max(num, temporaryOverlay.duration + 1f);
                    }
                    var particles = modelPanel.modelPrefab.GetComponentsInChildren<ParticleSystem>();
                    foreach (var particle in particles)
                    {
                        UnityEngine.Object.Destroy(particle.gameObject);
                    }
                    var lights = modelPanel.modelPrefab.GetComponentsInChildren<Light>();
                    foreach (var light in lights)
                    {
                        UnityEngine.Object.Destroy(light.gameObject);
                    }

                }
                catch (Exception message)
                {
                    Debug.Log(message);
                }
                RoR2Application.onLateUpdate += UpdateCamera;
                yield return new WaitForSeconds(num);
                modelPanel.SetAnglesForCharacterThumbnail(setZoom: true);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                try
                {
                    Texture2D texture2D = new Texture2D(modelPanel.renderTexture.width, modelPanel.renderTexture.height, TextureFormat.ARGB32, mipChain: false, linear: false);
                    RenderTexture active = RenderTexture.active;
                    RenderTexture.active = modelPanel.renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, modelPanel.renderTexture.width, modelPanel.renderTexture.height), 0, 0, recalculateMipMaps: false);
                    RenderTexture.active = active;
                    byte[] array = texture2D.EncodeToPNG();
                    FileStream fileStream = new FileStream("Assets/RoR2/GeneratedPortraits/" + gameObject.name + ".png", FileMode.Create, FileAccess.Write);
                    fileStream.Write(array, 0, array.Length);
                    fileStream.Close();
                }
                catch (Exception message2)
                {
                    Debug.Log(message2);
                }
                RoR2Application.onLateUpdate -= UpdateCamera;
                yield return new WaitForEndOfFrame();
            }
            void UpdateCamera()
            {
                modelPanel.SetAnglesForCharacterThumbnail(setZoom: true);
            }
        }

    }
}
