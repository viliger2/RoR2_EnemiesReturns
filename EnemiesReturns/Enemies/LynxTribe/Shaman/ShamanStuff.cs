using EnemiesReturns.ModCompats.PrefabAPICompat;
using Mono.Cecil.Cil;
using MonoMod.Cil;
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

namespace EnemiesReturns.Enemies.LynxTribe.Shaman
{
    public class ShamanStuff
    {
        public static R2API.DamageAPI.ModdedDamageType ApplyReducedHealing;

        public static BuffDef ReduceHealing;

        public static void OnHitEnemy(DamageInfo damageInfo, CharacterBody attackerBody, GameObject victim)
        {
            if (damageInfo.HasModdedDamageType(ApplyReducedHealing))
            {
                if(victim.TryGetComponent<CharacterBody>(out var victimBody))
                {
                    victimBody.AddTimedBuff(ReduceHealing, EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesDebuffDuration.Value);
                }
            }
        }

        public static void HealthComponent_Heal(MonoMod.Cil.ILContext il)
        { 
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchLdarg(out _),
                x => x.MatchStloc(out _)))
            {
                c.Index--;
                c.Emit(OpCodes.Ldarg_0); // self
                c.EmitDelegate<System.Func<float, RoR2.HealthComponent, float>>((amount, self) =>
                {
                    if (self.body && self.body.HasBuff(ShamanStuff.ReduceHealing))
                    {
                        amount *= (1f - EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesHealingFraction.Value);
                    }
                    return amount;
                });
                c.Emit(OpCodes.Starg, 1);
                c.Emit(OpCodes.Ldarg_1);
            }
            else
            {
                Log.Warning("ILHook failed: HealthComponent_Heal");
            }
        }

        public BuffDef CreateReduceHealingBuff(Sprite sprite)
        {
            BuffDef buff = ScriptableObject.CreateInstance<BuffDef>();
            (buff as ScriptableObject).name = "bdReduceHealing";
            if (sprite)
            {
                buff.iconSprite = sprite;
            }
            buff.isDebuff = true;
            buff.canStack = false;
            buff.isCooldown = false;
            buff.isDOT = false;
            buff.isHidden = false;
            buff.buffColor = new Color(90/255, 211/255, 74/255);

            return buff;
        }

        public GameObject CreateSummonStormParticles(GameObject prefab)
        {
            prefab.GetComponentInChildren<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoDiseaseTrail.mat").WaitForCompletion();
            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/PassiveHealing/matWoodSpriteFlare.mat").WaitForCompletion();
            return prefab;
        }

        public GameObject SetupShamanMaskMaterials(GameObject prefab)
        {
            prefab.GetComponentInChildren<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGhostEffect.mat").WaitForCompletion();
            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGhostParticleReplacement.mat").WaitForCompletion();
            return prefab;
        }

        public GameObject CreateShamanTrackingProjectileGhost()
        {
            var prefab = MyPrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NovaOnHeal/DevilOrbEffect.prefab").WaitForCompletion(), "LynxShamanTrackingProjectileGhost", false);

            if(prefab.TryGetComponent<OrbEffect>(out var orbEffect))
            {
                UnityEngine.Object.DestroyImmediate(orbEffect);
            }
            if(prefab.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                UnityEngine.Object.DestroyImmediate(rigidbody);
            }
            var akComponents = prefab.GetComponents<AkEvent>();
            for(int i = akComponents.Length - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(akComponents[i]);
            }
            if (prefab.TryGetComponent<AkGameObj>(out var akGameObj))
            {
                UnityEngine.Object.DestroyImmediate(akGameObj);
            }
            if(prefab.TryGetComponent<EffectComponent>(out var effectComponent))
            {
                UnityEngine.Object.DestroyImmediate(effectComponent);
            }
            prefab.AddComponent<ProjectileGhostController>().inheritScaleFromProjectile = false;

            return prefab;
        }

        public GameObject CreateShamanTrackingProjectile(GameObject prefab, GameObject ghost)
        {
            prefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            var projectileController = prefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = ghost;
            //projectileController.flightSoundLoop = ; TODO
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectileProcCoefficient.Value;

            var projectileNetworkTransform = prefab.AddComponent<ProjectileNetworkTransform>();
            projectileNetworkTransform.positionTransmitInterval = 0.03333334f;
            projectileNetworkTransform.interpolationFactor = 1f;
            projectileNetworkTransform.allowClientsideCollision = false;
            
            var projectileSimple = prefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesLifetime.Value;
            projectileSimple.desiredForwardSpeed = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesSpeed.Value;
            projectileSimple.updateAfterFiring = true; // maybe? so it won't fall down

            var projectileSingleTarget = prefab.AddComponent<ProjectileSingleTargetImpact>();
            projectileSingleTarget.destroyWhenNotAlive = true;
            projectileSingleTarget.destroyOnWorld = true;
            //projectileSingleTarget.impactEffect =  TODO
            //projectileSingleTarget.hitSoundString = TODO

            var projectileDamage = prefab.AddComponent<ProjectileDamage>();
            projectileDamage.damageType.damageType = DamageType.Generic;

            prefab.AddComponent<ProjectileTargetComponent>();

            var targetFinder = prefab.AddComponent<ProjectileDirectionalTargetFinder>();
            targetFinder.lookRange = 600f;
            targetFinder.lookCone = 180f;
            targetFinder.targetSearchInterval = 1f;
            targetFinder.onlySearchIfNoTarget = true;
            targetFinder.allowTargetLoss = false;
            targetFinder.testLoS = true;
            targetFinder.ignoreAir = false;

            var steerTowards = prefab.AddComponent<ProjectileSteerTowardTarget>();
            steerTowards.rotationSpeed = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesTurnSpeed.Value;

            var moddedDamage = prefab.AddComponent<R2API.DamageAPI.ModdedDamageTypeHolderComponent>();
            moddedDamage.Add(ApplyReducedHealing);

            prefab.AddComponent<TeamFilter>();

            return prefab;
        }

        public GameObject CreateShamanTeleportOut(GameObject prefab)
        {
            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = false;
            effectComponent.parentToReferencedTransform = false;
            effectComponent.applyScale = true;
            effectComponent.soundName = ""; // TODO
            effectComponent.disregardZScale = false;

            prefab.AddComponent<DestroyOnTimer>().duration = 0.75f;

            var lightTransform = prefab.transform.Find("Light");
            var lightIntencityCurve = lightTransform.gameObject.AddComponent<LightIntensityCurve>();
            lightIntencityCurve.timeMax = 0.5f;
            lightIntencityCurve.curve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // TODO

            vfxAttributes.optionalLights = new Light[] { lightTransform.GetComponent<Light>() };

            foreach(var component in prefab.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                component.material = ContentProvider.GetOrCreateMaterial("matLynxShamanTeleport", CreateTeleportEffectMaterial);
            }

            return prefab;
        }

        public GameObject CreateShamanTeleportIn(GameObject prefab)
        {
            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = false;
            effectComponent.parentToReferencedTransform = false;
            effectComponent.applyScale = true;
            effectComponent.soundName = ""; // TODO
            effectComponent.disregardZScale = false;

            prefab.AddComponent<DestroyOnTimer>().duration = 0.75f;

            var lightTransform = prefab.transform.Find("Light");
            var lightIntencityCurve = lightTransform.gameObject.AddComponent<LightIntensityCurve>();
            lightIntencityCurve.timeMax = 0.5f;
            lightIntencityCurve.curve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // TODO

            vfxAttributes.optionalLights = new Light[] { lightTransform.GetComponent<Light>() };

            foreach (var component in prefab.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                component.material = ContentProvider.GetOrCreateMaterial("matLynxShamanTeleport", CreateTeleportEffectMaterial);
            }

            return prefab;
        }

        public Material CreateTeleportEffectMaterial()
        {
            var material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matHealingCross.mat").WaitForCompletion();
            material.name = "matLynxShamanTeleport";

            var banditExplosionMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Bandit2/matBandit2Explosion.mat").WaitForCompletion();
            material.SetTexture("_MainTex", banditExplosionMat.GetTexture("_MainTex"));
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLightning2.png").WaitForCompletion());
            material.SetInt("_CloudsOn", 1);
            material.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudOrganicNormal.png").WaitForCompletion());
            material.SetTextureScale("_Cloud1Tex", new Vector2(0.5f, 0.5f));
            material.SetVector("_CutoffScroll", new Vector4(1f, 1f, 0f, 0f));

            return material;
        }
    }
}
