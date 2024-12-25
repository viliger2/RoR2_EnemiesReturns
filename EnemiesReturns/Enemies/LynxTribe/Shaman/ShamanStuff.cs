using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.LynxTribe.Shaman.Storm;
using EnemiesReturns.ModCompats.PrefabAPICompat;
using RoR2.Audio;
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

        // TODO: use ProjectileInflictTimedBuff instead
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

        public GameObject CreateReduceHealingVisualEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/HealingDisabledEffect.prefab").WaitForCompletion();

            UnityEngine.Object.DestroyImmediate(prefab.GetComponent<LocalCameraEffect>());
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("CameraEffect").gameObject);

            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Visual/Mesh").gameObject);
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Visual/Mesh").gameObject);

            return prefab;
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
            buff.buffColor = Color.white;

            return buff;
        }

        // TODO: with two leafs for other tribe members
        public GameObject CreateSpawnEffect(GameObject prefab)
        {
            var spawnEffect = MyPrefabAPI.InstantiateClone(prefab, "LynxShamanSpawnEffect", false);

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

        public GameObject CreateShamanPushBackSummonEffect(GameObject prefab)
        {
            prefab.AddComponent<EffectComponent>().applyScale = true;

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;

            prefab.AddComponent<DestroyOnParticleEnd>();

            prefab.transform.Find("TornadoMeshLow").gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Junk/Assassin/matAssassinSwingTrail.mat").WaitForCompletion();
            var lic = prefab.transform.Find("Light").gameObject.AddComponent<LightIntensityCurve>();
            lic.timeMax = 1f;
            //lic.curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

            lic.curve = new AnimationCurve(new Keyframe[]
            {
                new Keyframe
                {
                    time = 0f,
                    value = 0f
                },
                new Keyframe
                {
                    time = 0.95f,
                    value = 1f,
                },
                new Keyframe
                {
                    time = 1f,
                    value = 0f
                }
            });

            vfxAttributes.optionalLights = prefab.GetComponentsInChildren<Light>();

            return prefab;
        }

        public GameObject CreateShamanPushBackExplosionEffect(GameObject prefab)
        {
            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = "ER_Shaman_FirePushBack_Play"; // TODO

            var shakeEmmiter = prefab.AddComponent<ShakeEmitter>();
            shakeEmmiter.shakeOnStart = true;
            shakeEmmiter.wave = new Wave
            {
                amplitude = 1f,
                frequency = 120f,
                cycleOffset = 0f
            };
            shakeEmmiter.duration = 0.2f;
            shakeEmmiter.radius = 30f;
            shakeEmmiter.amplitudeTimeDecay = true;

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.optionalLights = prefab.GetComponentsInChildren<Light>();

            prefab.AddComponent<DestroyOnParticleEnd>();

            var lic = prefab.transform.Find("Point Light").gameObject.AddComponent<LightIntensityCurve>();
            lic.timeMax = 0.3f;
            lic.curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

            prefab.transform.Find("SparksOut").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/EliteEarth/matAffixEarthTargetBillboard.mat").WaitForCompletion();
            prefab.transform.Find("Flash, Colored").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGlow1Soft.mat").WaitForCompletion();
            prefab.transform.Find("Flash, White").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxShamanExplosionFlash", CreatePushBackExplosionFlashWhiteMaterial);
            prefab.transform.Find("Sphere, Distortion").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunnerMineDistortion.mat").WaitForCompletion();
            prefab.transform.Find("Core").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/TPHealingNova/matGlowFlowerRings.mat").WaitForCompletion();
            prefab.transform.Find("Sphere, Color").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxShamanExplosionAreaIndicator", CreatePushBackExplosionAreaIndicatorMaterial);

            return prefab;
        }

        public GameObject CreateShamanTrackingProjectileSummonEffect(AnimationCurveDef acd)
        {
            var prefab = MyPrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/NovaOnHeal/DevilOrbEffect.prefab").WaitForCompletion(), "LynxShamanTrackingProjectileEffect", false);
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
            if(prefab.TryGetComponent<LODGroup>(out var lodGroup))
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
            objectScaleCurve.timeMax = 2.3f;

            prefab.AddComponent<DestroyOnTimer>().duration = 2.3f;

            return prefab;
        }

        public GameObject CreateShamanProjectileImpactEffect(GameObject prefab, Texture2D texRamp)
        {
            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Low;

            var effectComponent = prefab.AddComponent<EffectComponent>();

            prefab.AddComponent<DestroyOnTimer>().duration = 0.55f;

            prefab.transform.Find("Nova Sphere").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxShamanProjectileImpactNova", CreateShamanProjectileImpactNovaMaterial);
            prefab.transform.Find("Sparks").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxShamanProjectileImpactSparks", CreateShamanProjectileImpactSparksMaterial, texRamp);
            prefab.transform.Find("Flares").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxShamanProjectileImpactFlares", CreateShamanProjectileImpactFlaresMaterial);

            return prefab;
        }
        
        public Material CreateShamanProjectileImpactFlaresMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamBillboard2.mat").WaitForCompletion());
            material.name = "matLynxShamanProjectileImpactFlares";

            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampHealing.png").WaitForCompletion());

            return material;
        }

        public Material CreateShamanProjectileImpactSparksMaterial(Texture2D texRamp)
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamBillboard2.mat").WaitForCompletion());
            material.name = "matLynxShamanProjectileImpactSparks";

            material.SetTexture("_RemapTex", texRamp);

            return material;
        }

        public Material CreateShamanProjectileImpactNovaMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabTripleBeamSphere2.mat").WaitForCompletion());
            material.name = "matLynxShamanProjectileImpactNova";

            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampHealingVariant.png").WaitForCompletion());

            return material;
        }

        public GameObject CreateShamanTrackingProjectile(GameObject prefab, GameObject ghost, GameObject impact, LoopSoundDef loopSound)
        {
            prefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            var projectileController = prefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = ghost;
            projectileController.flightSoundLoop = loopSound;
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
            projectileSingleTarget.impactEffect = impact;
            projectileSingleTarget.hitSoundString = "ER_Shaman_Projectile_Impact_Play";

            var projectileDamage = prefab.AddComponent<ProjectileDamage>();
            projectileDamage.damageType.AddModdedDamageType(ApplyReducedHealing);

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

            prefab.AddComponent<TeamFilter>();

            return prefab;
        }

        public LoopSoundDef CreateProjectileFlightSoundLoop()
        {
            var loopSound = ScriptableObject.CreateInstance<LoopSoundDef>();
            (loopSound as ScriptableObject).name = "lsdShamanProjectile";
            loopSound.startSoundName = "ER_Shaman_Projectile_Flight_Loop_Play";
            loopSound.stopSoundName = "ER_Shaman_Projectile_Flight_Loop_Stop";

            return loopSound;
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
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matHealingCross.mat").WaitForCompletion());
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

        public Material CreatePushBackExplosionFlashWhiteMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion());
            material.name = "matLynxShamanExplosionFlash";

            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampHealing.png").WaitForCompletion());

            return material;
        }

        public Material CreatePushBackExplosionAreaIndicatorMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunnerMineAreaIndicator.mat").WaitForCompletion());
            material.name = "matLynxShamanExplosionAreaIndicator";

            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampBeetleQueen.png").WaitForCompletion());

            return material;
        }
    }
}
