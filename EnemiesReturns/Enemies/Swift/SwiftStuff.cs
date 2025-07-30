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

    }
}
