using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Hunter
{
    public class HunterStuff
    {
        public GameObject CreateHunterAttackEffect(GameObject prefab)
        {
            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true;

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 0.3f;

            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC2/Chef/matChefCleaverGhost2.mat").WaitForCompletion();

            return prefab;
        }

        public GameObject CreateHunterAttackSpearTipEffect(GameObject prefab)
        {
            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true;

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;

            var destroyOnParticleEnd = prefab.AddComponent<DestroyOnParticleEnd>();
            destroyOnParticleEnd.trackedParticleSystem = prefab.transform.Find("Cone").gameObject.GetComponent<ParticleSystem>();

            var material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matHealTrail.mat").WaitForCompletion();
            foreach (var psr in prefab.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                psr.material = material;
            }

            return prefab;
        }

    }
}
