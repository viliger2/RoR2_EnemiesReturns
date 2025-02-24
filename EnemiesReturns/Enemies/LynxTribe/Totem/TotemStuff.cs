using EnemiesReturns.Behaviors;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.LynxTribe.Storm;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class TotemStuff
    {
        public static DeployableSlot SummonLynxTribeDeployable;

        public void RegisterDeployableSlot()
        {
            SummonLynxTribeDeployable = R2API.DeployableAPI.RegisterDeployableSlot(GetSummonLynxTribeDeployableLimit);
        }

        private static int GetSummonLynxTribeDeployableLimit(CharacterMaster master, int countMultiplier)
        {
            return EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonTribeMaxCount.Value;
        }

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

        public GameObject CreateGroundpoundProjectile(GameObject prefab)
        {
            prefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            prefab.AddComponent<TeamFilter>();

            var projectileController = prefab.AddComponent<ProjectileController>();
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = 1f;
            projectileController.cannotBeDeleted = true;

            var projectileDamage = prefab.AddComponent<ProjectileDamage>();

            var groundpountHitbox = prefab.transform.Find("GroundpoundHitbox").gameObject;
            var hitboxAttack = groundpountHitbox.AddComponent<HitboxAttackClientside>();
            hitboxAttack.projectileController = projectileController;
            hitboxAttack.projectileDamage = projectileDamage;
            hitboxAttack.force = EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundForce.Value;
            hitboxAttack.damage = EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundDamage.Value;
            hitboxAttack.procCoefficient = EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundProcCoefficient.Value;
            hitboxAttack.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary);

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 0.25f;

            prefab.RegisterNetworkPrefab();

            return prefab;
        }

        public GameObject CreateEyeGlowEffect(GameObject prefabOriginal, AnimationCurveDef curveDef, float duration)
        {
            var prefab = prefabOriginal.InstantiateClone("TotemEyeGlow" + duration, false);
            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.parentToReferencedTransform = true;
            effectComponent.positionAtReferencedTransform = true;

            var vfxComponent = prefab.AddComponent<VFXAttributes>();
            vfxComponent.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxComponent.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var lic = prefab.transform.Find("Light").gameObject.AddComponent<LightIntensityCurve>();
            lic.curve = curveDef.curve;
            lic.timeMax = duration;
            lic.loop = false;
            lic.randomStart = false;
            lic.enableNegativeLights = false;

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = duration + 0.1f;

            return prefab;
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

        public GameObject CreateStoneParticlesEffect(GameObject prefab)
        {
            prefab.AddComponent<EffectComponent>();

            var material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDebris1.mat").WaitForCompletion();
            prefab.transform.Find("Particles/Debris").GetComponent<ParticleSystemRenderer>().material = material;
            prefab.transform.Find("Particles/Debris (1)").GetComponent<ParticleSystemRenderer>().material = material;
            prefab.transform.Find("Particles/Debris (2)").GetComponent<ParticleSystemRenderer>().material = material;

            var vfxComponent = prefab.AddComponent<VFXAttributes>();
            vfxComponent.vfxPriority = VFXAttributes.VFXPriority.Low;
            vfxComponent.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            prefab.AddComponent<DestroyOnParticleEnd>();

            return prefab;
        }

        public GameObject CreateSummonStormsStaffParticle(AnimationCurveDef acd)
        {
            var prefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NovaOnHeal/DevilOrbEffect.prefab").WaitForCompletion(), "LynxTotemSummonStormStaffEffect", false);
            if (prefab.TryGetComponent<OrbEffect>(out var orbEffect))
            {
                UnityEngine.Object.DestroyImmediate(orbEffect);
            }
            if (prefab.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                UnityEngine.Object.DestroyImmediate(rigidbody);
            }
            var akComponents = prefab.GetComponents<AkEvent>();
            for (int i = akComponents.Length - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(akComponents[i]);
            }
            if (prefab.TryGetComponent<AkGameObj>(out var akGameObj))
            {
                UnityEngine.Object.DestroyImmediate(akGameObj);
            }
            if (prefab.TryGetComponent<LODGroup>(out var lodGroup))
            {
                UnityEngine.Object.DestroyImmediate(lodGroup);
            }

            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("mdlDevilOrb").gameObject);

            prefab.GetComponentInChildren<LightIntensityCurve>().timeMax = 3f;

            var effectComponent = prefab.GetComponent<EffectComponent>();
            effectComponent.parentToReferencedTransform = true;
            effectComponent.positionAtReferencedTransform = true;
            //effectComponent.applyScale = true;

            var objectScaleCurve = prefab.AddComponent<ObjectScaleCurve>();
            objectScaleCurve.useOverallCurveOnly = true;
            objectScaleCurve.overallCurve = acd.curve;
            objectScaleCurve.timeMax = 2.6f;

            prefab.AddComponent<DestroyOnTimer>().duration = 2.6f;

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
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("LynxTotemGroundpoundEffect", false);

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

            var scale = 4.0f * (EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundRadius.Value / 25f);
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
