using EnemiesReturns.Behaviors.Judgement;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Aeonian
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject flowerPrefab;

        public static float effectDelay = 0.4f;

        public static GameObject effectPrefab;

        private bool effectSpawned;

        public override void CreateDeathEffects()
        {
            base.CreateDeathEffects();
            if(!isVoidDeath && !destroyModelOnDeath && !isBrittle)
            {
                TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(cachedModelTransform.gameObject);
                temporaryOverlayInstance.duration = 0.5f;
                temporaryOverlayInstance.destroyObjectOnEnd = true;
                temporaryOverlayInstance.originalMaterial = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.matShatteredGlass_mat).WaitForCompletion();
                temporaryOverlayInstance.inspectorCharacterModel = cachedModelTransform.gameObject.GetComponent<CharacterModel>();
                temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                temporaryOverlayInstance.animateShaderAlpha = true;
                temporaryOverlayInstance.transmit = false;

                if (flowerPrefab)
                {
                    var position = transform.position;
                    if(Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 1000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                    {
                        position = hitInfo.point;
                    }

                    var flowerInstance = UnityEngine.Object.Instantiate(flowerPrefab, position, Quaternion.identity);
                    if (characterBody)
                    {
                        if (flowerInstance.TryGetComponent<MoonFlowerScaler>(out var scaler))
                        {
                            scaler.scale = characterBody.bestFitActualRadius;
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(!effectSpawned && fixedAge > effectDelay)
            {
                SpawnEffect();
                effectSpawned = true;
            }
        }

        public void SpawnEffect()
        {
            EffectData data = new EffectData()
            {
                origin = characterBody.corePosition,
                scale = characterBody.bestFitActualRadius
            };

            EffectManager.SpawnEffect(effectPrefab, data, false);
        }

    }
}
