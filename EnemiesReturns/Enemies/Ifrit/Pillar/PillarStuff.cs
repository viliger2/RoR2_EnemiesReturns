using R2API;
using RoR2;
using RoR2.EntityLogic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.Ifrit.Pillar
{
    public class PillarStuff
    {
        public static R2API.ModdedProcType PillarExplosion;

        public GameObject CreateExplosionEffect()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBoss/OmniExplosionVFXRoboBallBossDeath.prefab").WaitForCompletion().InstantiateClone("IfritPylonExplosionEffect", false);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<OmniEffect>());

            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
            }

            return gameObject;
        }

        public GameObject CreateExlosionEffectAlt()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClayBoss/ClayBossDeath.prefab").WaitForCompletion().InstantiateClone("IfritPylonExplosionEffectAlt", false);
            gameObject.GetComponent<EffectComponent>().applyScale = true;

            var scale = gameObject.transform.localScale.x * (EnemiesReturns.Configuration.Ifrit.PillarExplosionRadius.Value / 30f);
            gameObject.transform.localScale = new Vector3(scale, scale, scale);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<AwakeEvent>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<DelayedEvent>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Corpse>());

            UnityEngine.Object.DestroyImmediate(gameObject.transform.Find("mdlClayBossShattered").gameObject);

            UnityEngine.Object.DestroyImmediate(gameObject.transform.Find("Particles/Goo").gameObject);

            return gameObject;
        }

        public GameObject CreateSpawnEffect()
        {
            var cloneEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanSpawnEffect.prefab").WaitForCompletion().InstantiateClone("IfritPillarSpawnEffect", false);

            var shakeEmitter = cloneEffect.GetComponent<ShakeEmitter>();
            shakeEmitter.duration = 3f;
            shakeEmitter.radius = 10f;

            var components = cloneEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            var light = cloneEffect.GetComponentInChildren<Light>();
            if (light)
            {
                light.range = 5f;
            }

            cloneEffect.transform.localScale = new Vector3(1f, 1f, 1f);

            return cloneEffect;
        }

        public GameObject CreateDeathFallEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGuard/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("IfritPillarFallEffect", false);

            UnityEngine.Object.DestroyImmediate(clonedEffect.GetComponent<ShakeEmitter>());

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Large").gameObject);
            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Small").gameObject);

            clonedEffect.transform.localScale = new Vector3(1f, 1f, 1f);

            return clonedEffect;
        }
    }
}
