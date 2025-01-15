using EnemiesReturns.Behaviors;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class TotemStuff
    {
        public GameObject CreateShamanTotemSpawnEffect(GameObject prefab)
        {
            var spawnEffect = PrefabAPI.InstantiateClone(prefab, "LynxShamanTotemSpawnEffect", false);

            spawnEffect.transform.Find("PurpleLeaves").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeaf.mat").WaitForCompletion();
            spawnEffect.transform.Find("YellowLeaves").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeafAlt.mat").WaitForCompletion();
            //prefab.transform.Find("YellowLeaves").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matTreebotTreeLeaf2", LynxStormBody.CreateTreebotTreeLeaf2Material); ;

            spawnEffect.AddComponent<EffectComponent>().positionAtReferencedTransform = true;

            var vfxattributes = spawnEffect.AddComponent<VFXAttributes>();
            vfxattributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxattributes.vfxIntensity = VFXAttributes.VFXIntensity.High;

            spawnEffect.AddComponent<DestroyOnParticleEnd>();

            return spawnEffect;
        }

        public GameObject CreateTribesmenSpawnEffect(GameObject prefab)
        {
            var spawnEffect = prefab.InstantiateClone("LynxTribeSpawnEffect", false);

            spawnEffect.transform.Find("PurpleLeaves").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeaf.mat").WaitForCompletion();
            //prefab.transform.Find("YellowLeaves").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Treebot/matTreebotTreeLeafAlt.mat").WaitForCompletion();
            spawnEffect.transform.Find("YellowLeaves").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matTreebotTreeLeaf2", LynxStormBody.CreateTreebotTreeLeaf2Material);

            spawnEffect.AddComponent<EffectComponent>().positionAtReferencedTransform = true;

            var vfxattributes = spawnEffect.AddComponent<VFXAttributes>();
            vfxattributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxattributes.vfxIntensity = VFXAttributes.VFXIntensity.High;

            spawnEffect.AddComponent<DestroyOnParticleEnd>();

            return spawnEffect;
        }

        public GameObject CreateFirewallProjectile(GameObject prefab, AnimationCurveDef curveDef)
        {
            prefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            prefab.AddComponent<TeamFilter>();

            prefab.transform.Find("Sphere").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matTeamAreaIndicatorIntersectionMonster.mat").WaitForCompletion();

            var projectileController = prefab.AddComponent<ProjectileController>();
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = 1f; 
            projectileController.cannotBeDeleted = true;

            var projectileDamage = prefab.AddComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.IgniteOnHit; 

            var objectScaleCurve = prefab.AddComponent<ObjectScaleCurve>();
            objectScaleCurve.useOverallCurveOnly = true;
            objectScaleCurve.overallCurve = curveDef.curve;
            objectScaleCurve.timeMax = 5f; 

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 5f;

            var firefall = prefab.transform.Find("Firewall").gameObject;
            var firewallAttack = firefall.AddComponent<FirewallAttack>();
            firewallAttack.projectileController = projectileController;
            firewallAttack.projectileDamage = projectileDamage;

            prefab.RegisterNetworkPrefab();

            return prefab;
        }

        public GameObject CreateGroundpoundShakeEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanSpawnEffect.prefab").WaitForCompletion().InstantiateClone("LynxTotemShakeEffect", false);

            prefab.GetComponent<VFXAttributes>().DoNotPool = true; // we want to use DetachParticleOnDestroyAndEndEmission

            prefab.GetComponent<VFXAttributes>().optionalLights = Array.Empty<Light>();

            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("ParticleLoop/Point light").gameObject);
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("ParticleLoop/Sparks, Trail").gameObject);
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("ParticleLoop/Sparks,Wiggly").gameObject);

            var detach = prefab.AddComponent<DetachParticleOnDestroyAndEndEmission>();
            detach.particleSystem = prefab.transform.Find("ParticleLoop/Dust").GetComponent<ParticleSystem>();

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 0.9f;

            return prefab;
        }

        public GameObject CreateGroundpoundPoundEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("ColossusStompEffect", false);

            var shakeEmitter = clonedEffect.GetComponent<ShakeEmitter>();
            shakeEmitter.duration = 0.5f;
            shakeEmitter.radius = 100f;
            shakeEmitter.wave.amplitude = 0.3f;
            shakeEmitter.wave.frequency = 200f;

            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Large").gameObject);
            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Small").gameObject);

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            var scale = 5.5f * (EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundRadius.Value / 25f);
            clonedEffect.transform.localScale = new Vector3(scale, scale, scale);

            return clonedEffect;
        }

        public GameObject CreateStormSummonOrb(GameObject prefab)
        {
            prefab.AddComponent<EffectComponent>();

            var orbEffect = prefab.AddComponent<OrbEffect>();
            orbEffect.movementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f); // this thing dictades the lerp value between spawn and target, so constant would mean it instantly spawns on target
            orbEffect.faceMovement = true;

            var vfxComponent = prefab.AddComponent<VFXAttributes>();
            vfxComponent.vfxPriority = VFXAttributes.VFXPriority.Low;
            vfxComponent.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            prefab.GetComponentInChildren<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoDiseaseTrail.mat").WaitForCompletion();
            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/PassiveHealing/matWoodSpriteFlare.mat").WaitForCompletion();
            return prefab;
        }
    }
}
