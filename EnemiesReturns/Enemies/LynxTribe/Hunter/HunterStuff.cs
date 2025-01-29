using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Hunter
{
    public class HunterStuff
    {
        public GameObject CreateLungeSlideEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoSlideVFX.prefab").WaitForCompletion().InstantiateClone("LynxHunterLungeSlideEffect", false);
            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true;

            var vfxAttributes = prefab.GetComponent<VFXAttributes>();
            if (vfxAttributes)
            {
                vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            }

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 0.8f;

            var particleSystem = prefab.transform.Find("Debris").GetComponent<ParticleSystem>();
            var main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;

            var particleSystem2 = prefab.transform.Find("Dust").GetComponent<ParticleSystem>();
            var main2 = particleSystem.main;
            main2.scalingMode = ParticleSystemScalingMode.Hierarchy;

            return prefab;
        }

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
