using EnemiesReturns.Enemies.ArcherBug;
using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Enemies.Ifrit;
using EnemiesReturns.Enemies.Ifrit.Pillar;
using EnemiesReturns.Enemies.LynxTribe.Archer;
using EnemiesReturns.Enemies.LynxTribe.Hunter;
using EnemiesReturns.Enemies.LynxTribe.Scout;
using EnemiesReturns.Enemies.LynxTribe.Shaman;
using EnemiesReturns.Enemies.LynxTribe.Totem;
using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Enemies.Spitter;
using RoR2;
using RoR2.UI;
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace EnemiesReturns
{
    public class PortraitGenerator
    {
        [ConCommand(commandName = "returns_render_portraits", flags = ConVarFlags.None, helpText = "Generates portraits for all EnemiesReturns bodies.")]
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
            var iconGenerator = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/UI/IconGenerator"));
            ModelPanel modelPanel = iconGenerator.GetComponentInChildren<ModelPanel>();
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
            yield return GeneratePortrait(modelPanel, ArcherBugBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, BodyCatalog.GetBodyPrefab(Enemies.Judgement.SetupJudgementPath.ArraignP1BodyIndex));
            yield return GeneratePortrait(modelPanel, BodyCatalog.GetBodyPrefab(Enemies.Judgement.SetupJudgementPath.ArraignP2BodyIndex));
            yield return GeneratePortrait(modelPanel, SpitterBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, ColossusBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, IfritBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, PillarEnemyBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, MechanicalSpiderEnemyBody.BodyPrefab);
            modelPanel.lights[0].transform.rotation = Quaternion.Euler(0f, 126f, 0);
            yield return GeneratePortrait(modelPanel, ArcherBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, ArcherBodyAlly.BodyPrefab);
            modelPanel.lights[0].transform.rotation = Quaternion.Euler(0f, 180f, 0);
            yield return GeneratePortrait(modelPanel, HunterBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, HunterBodyAlly.BodyPrefab);
            modelPanel.lights[0].transform.rotation = Quaternion.Euler(24.6338f, 143.193f, -0.0001f);
            yield return GeneratePortrait(modelPanel, ScoutBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, ScoutBodyAlly.BodyPrefab);
            yield return GeneratePortrait(modelPanel, ShamanBody.BodyPrefab);
            yield return GeneratePortrait(modelPanel, ShamanBodyAlly.BodyPrefab);
            yield return GeneratePortrait(modelPanel, TotemBody.BodyPrefab);
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
