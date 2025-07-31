using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.Swift
{
    public class SwiftStuff
    {
        public GameObject CreateDiveChargeEffect(GameObject effectPrefab)
        {
            var effectComponent = effectPrefab.AddComponent<EffectComponent>();
            effectComponent.parentToReferencedTransform = true;

            var vfxAttributes = effectPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            effectPrefab.AddComponent<DestroyOnParticleEnd>();

            effectPrefab.transform.Find("Particles/Glow").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.matArcaneCircleWisp_mat).WaitForCompletion();
            effectPrefab.transform.Find("Particles/Sparks").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.matTracer_mat).WaitForCompletion();

            return effectPrefab;
        }

        public GameObject CreateDiveGroundImpactEffect()
        {
            var bellPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Bell.BellBall_prefab).WaitForCompletion();

            var stickEffecttransform = bellPrefab.transform.Find("StickEffect");
            var effectPrefab = stickEffecttransform.gameObject.InstantiateClone("SwiftDiveImpactEffect", false);

            var effectComponent = effectPrefab.AddComponent<EffectComponent>();

            var vfxComponent = effectPrefab.AddComponent<VFXAttributes>();
            vfxComponent.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxComponent.vfxPriority = VFXAttributes.VFXPriority.Low;

            var destroyOnEnd = effectPrefab.AddComponent<DestroyOnParticleEnd>();

            return effectPrefab;
        }

    }
}
