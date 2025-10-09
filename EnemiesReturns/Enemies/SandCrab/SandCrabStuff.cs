using EnemiesReturns.Behaviors;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System.Collections.Generic;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace EnemiesReturns.Enemies.SandCrab
{
    public class SandCrabStuff
    {
        public GameObject CreateBubbleImpactEffect(GameObject prefab)
        {
            var foamTransform = prefab.transform.Find("Effects/Foam");

            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.soundName = "ER_SandCrab_Bubbles_Death_Play";

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            var destroyOnEnd = prefab.AddComponent<DestroyOnParticleEnd>();
            destroyOnEnd.trackedParticleSystem = foamTransform.GetComponent<ParticleSystem>();

            foamTransform.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.matOpaqueWaterSplash_mat).WaitForCompletion();

            prefab.transform.Find("Effects/ImpactRing").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matSandCrabBubbleImpactRing", CreateBubbleImpactRingMaterial);

            var shakeEmitter = prefab.AddComponent<ShakeEmitter>();
            shakeEmitter.shakeOnStart = true;
            shakeEmitter.wave = new Wave()
            {
                amplitude = 0.1f,
                frequency = 1f,
                cycleOffset = 0f
            };
            shakeEmitter.duration = 0.1f;
            shakeEmitter.radius = 20f;
            shakeEmitter.amplitudeTimeDecay = true;

            prefab.transform.Find("Effects/Flash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.matTracerBrightTransparent_mat).WaitForCompletion();

            return prefab;
        }

        public LoopSoundDef CreateBubbleFlightLoop()
        {
            var loopSound = ScriptableObject.CreateInstance<LoopSoundDef>();
            (loopSound as ScriptableObject).name = "lsdSandCrabBubbleFlight";
            loopSound.startSoundName = "ER_SandCrab_Bubbles_FlightLoop_Play";
            loopSound.stopSoundName = "ER_SandCrab_Bubbles_FlightLoop_Stop";

            return loopSound;
        }

        public GameObject CreateSnipEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion().InstantiateClone("SandCrabSnipEffect", false);

            var particleSystem = clonedEffect.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.startRotation3D = false;
            main.startSize3D = false;

            clonedEffect.transform.localScale = new Vector3(2f, 2f, 2f);

            return clonedEffect;
        }

        public GameObject CreateBubbleGhost(GameObject ghostPrefab, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var lifetime = Configuration.SandCrab.BubbleLifetime.Value;

            ghostPrefab.transform.Find("Models").localScale = new Vector3(Configuration.SandCrab.BubbleSize.Value, Configuration.SandCrab.BubbleSize.Value, Configuration.SandCrab.BubbleSize.Value);

            ghostPrefab.AddComponent<ProjectileGhostController>().inheritScaleFromProjectile = false;

            var vfxAttributes = ghostPrefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            ghostPrefab.AddComponent<EffectManagerHelper>();

            var scaleCurveLoop = ghostPrefab.AddComponent<ObjectScaleCurve>();
            scaleCurveLoop.useOverallCurveOnly = false;
            scaleCurveLoop.curveX = acdLookup["acdSandCrabBubbleWobbleXZ"].curve;
            scaleCurveLoop.curveZ = acdLookup["acdSandCrabBubbleWobbleXZ"].curve;
            scaleCurveLoop.curveY = acdLookup["acdSandCrabBubbleWobbleY"].curve;
            scaleCurveLoop.overallCurve = acdLookup["acdLinearCurve"].curve;
            scaleCurveLoop.timeMax = 3f;
            scaleCurveLoop.resetOnAwake = false;

            var scaleCurveInitial = ghostPrefab.AddComponent<ObjectScaleCurve>();
            scaleCurveInitial.useOverallCurveOnly = true;
            scaleCurveInitial.overallCurve = acdLookup["acdSandCrabBubbleInitialScale"].curve;
            scaleCurveInitial.timeMax = lifetime * 0.125f;
            scaleCurveInitial.resetOnAwake = false;

            ghostPrefab.AddComponent<ObjectScaleCurveDisableOnMaxTime>().curve = scaleCurveInitial;

            ghostPrefab.AddComponent<ObjectScaleCurveResetOnMaxTime>().curve = scaleCurveLoop;

            var enabler = ghostPrefab.AddComponent<ComponentEnabler>();
            enabler.component = scaleCurveLoop;

            var enabler2 = ghostPrefab.AddComponent<ComponentEnabler>();
            enabler2.component = scaleCurveInitial;

            var awakeEvents = ghostPrefab.AddComponent<OnEnableEvent>();
            awakeEvents.action = new UnityEngine.Events.UnityEvent();
            awakeEvents.action.AddPersistentListener(enabler.DisableComponent);
            awakeEvents.action.AddPersistentListener(enabler2.EnableComponent);

            var timer = ghostPrefab.AddComponent<RoR2.EntityLogic.Timer>();
            timer.duration = lifetime * 0.125f;
            timer.resetTimerOnEnable = true;
            timer.playTimerOnEnable = true;
            timer.loop = false;
            timer.timeStepType = RoR2.EntityLogic.Timer.TimeStepType.FixedTime;
            timer.action = new UnityEngine.Events.UnityEvent();
            timer.action.AddPersistentListener(enabler.EnableComponent);

            var meshRenderer = ghostPrefab.transform.Find("Models/Sphere").GetComponent<MeshRenderer>();
            meshRenderer.material = ContentProvider.GetOrCreateMaterial("matSandCrabBubble", CreateBubbleMaterial);

            ghostPrefab.transform.Find("SplatEffect").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.matOpaqueWaterSplash_mat).WaitForCompletion();

            return ghostPrefab;
        }

        public GameObject CreateBubbleProjectile(GameObject projectilePrefab, GameObject projectileGhost, AnimationCurveDef acdBubbleSpeed, GameObject impactEffect, LoopSoundDef flightLoop)
        {
            var lifetime = Configuration.SandCrab.BubbleLifetime.Value;

            projectilePrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            projectilePrefab.GetComponent<SphereCollider>().radius = Configuration.SandCrab.BubbleSize.Value / 2f;

            var hurtBoxTransform = projectilePrefab.transform.Find("Model/HurtBox");
            hurtBoxTransform.GetComponent<SphereCollider>().radius = Configuration.SandCrab.BubbleSize.Value / 2f;
            var proximityDetonatorTransform = projectilePrefab.transform.Find("ProximityDetonator");
            proximityDetonatorTransform.GetComponent<SphereCollider>().radius = Configuration.SandCrab.BubbleSize.Value / 2f;

            var dissableCollisions = projectilePrefab.AddComponent<DisableCollisionsBetweenColliders>();
            dissableCollisions.collidersA = new Collider[] { projectilePrefab.GetComponent<SphereCollider>(), proximityDetonatorTransform.GetComponent<SphereCollider>() };
            dissableCollisions.collidersB = new Collider[] { hurtBoxTransform.GetComponent<SphereCollider>() };        

            var projectileController = projectilePrefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = projectileGhost;
            projectileController.flightSoundLoop = flightLoop;
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = 1f;

            var projectileNetworkTransform = projectilePrefab.AddComponent<ProjectileNetworkTransform>();
            projectileNetworkTransform.positionTransmitInterval = 0.03333334f;
            projectileNetworkTransform.interpolationFactor = 1f;
            projectileNetworkTransform.allowClientsideCollision = false;

            var projectileSimple = projectilePrefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = lifetime;
            projectileSimple.desiredForwardSpeed = Configuration.SandCrab.BubbleSpeed.Value; // TODO: curve multiplies this value, not assignes value by itself, so at the begining we can do like x5, then stop and then set it to x1
            projectileSimple.updateAfterFiring = true;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = acdBubbleSpeed.curve;
            projectileSimple.lifetimeExpiredEffect = impactEffect;
            projectileSimple.oscillate = false; 

            var oscillate = projectilePrefab.AddComponent<ProjectileOscillate>();
            oscillate.oscillateY = true;
            oscillate.oscillateMagnitude = 1.0f;
            oscillate.oscillateSpeed = 2f;
            oscillate.randomStartingOffset = true;

            projectilePrefab.AddComponent<ProjectileDamage>();
            projectilePrefab.AddComponent<ProjectileTargetComponent>();

            var targetFinder = projectilePrefab.AddComponent<ProjectileDirectionalTargetFinder>();
            targetFinder.lookRange = 600f;
            targetFinder.lookCone = 180f;
            targetFinder.targetSearchInterval = 1f;
            targetFinder.onlySearchIfNoTarget = true;
            targetFinder.allowTargetLoss = false;
            targetFinder.testLoS = true;
            targetFinder.ignoreAir = false;
            targetFinder.enabled = false;

            var steerTowards = projectilePrefab.AddComponent<ProjectileSteerTowardTarget>();
            steerTowards.rotationSpeed = 125f;

            var enabler = projectilePrefab.AddComponent<ComponentEnabler>();
            enabler.component = targetFinder;

            var timer = projectilePrefab.AddComponent<RoR2.EntityLogic.Timer>();
            timer.duration = lifetime * 0.125f;
            timer.resetTimerOnEnable = true;
            timer.playTimerOnEnable = true;
            timer.loop = false;
            timer.timeStepType = RoR2.EntityLogic.Timer.TimeStepType.FixedTime;
            timer.action = new UnityEngine.Events.UnityEvent();
            timer.action.AddPersistentListener(enabler.EnableComponent);

            var teamFilter = projectilePrefab.GetOrAddComponent<TeamFilter>();

            var characterBody = projectilePrefab.AddComponent<CharacterBody>();
            characterBody.baseMaxHealth = Configuration.SandCrab.BubbleBaseHealth.Value;
            characterBody.levelMaxHealth = Configuration.SandCrab.BubbleHealthPerLevel.Value;
            characterBody.bodyFlags = CharacterBody.BodyFlags.Masterless | CharacterBody.BodyFlags.ResistantToAOE;
            characterBody.doNotReassignToTeamBasedCollisionLayer = true;

            var teamComponent = projectilePrefab.GetOrAddComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = true;

            var assigner = projectilePrefab.AddComponent<AssignTeamFilterToTeamComponent>();

            var healthComponent = projectilePrefab.GetOrAddComponent<HealthComponent>();
            healthComponent.body = characterBody;
            healthComponent.globalDeathEventChanceCoefficient = Configuration.SandCrab.BubbleGlobalDeathProcCoefficient.Value;

            var modelLocator = projectilePrefab.GetOrAddComponent<ModelLocator>();
            modelLocator.modelTransform = projectilePrefab.transform.Find("Model");
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;
            modelLocator.dontReleaseModelOnDeath = true;

            var hurtBox = hurtBoxTransform.gameObject.AddComponent<HurtBox>();
            hurtBox.healthComponent = healthComponent;
            hurtBox.isBullseye = true;
            hurtBox.isSniperTarget = false;
            hurtBox.damageModifier = HurtBox.DamageModifier.Normal;

            var group = projectilePrefab.transform.Find("Model").gameObject.AddComponent<HurtBoxGroup>();
            group.hurtBoxes = new HurtBox[] { hurtBox };
            group.mainHurtBox = hurtBox;
            hurtBox.hurtBoxGroup = group;
            hurtBox.indexInGroup = 0;

            var impactExplosion = projectilePrefab.AddComponent<ProjectileImpactExplosion>();
            impactExplosion.falloffModel = BlastAttack.FalloffModel.None;
            impactExplosion.blastRadius = Configuration.SandCrab.BubbleExplosionSize.Value;
            impactExplosion.blastDamageCoefficient = 1f;
            impactExplosion.blastProcCoefficient = 1f;
            impactExplosion.projectileHealthComponent = healthComponent;

            impactExplosion.impactEffect = impactEffect; 
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.impactOnWorld = true;
            impactExplosion.timerAfterImpact = false;
            impactExplosion.explodeOnLifeTimeExpiration = true;
            impactExplosion.lifetime = lifetime;
            impactExplosion.lifetimeRandomOffset = 1f;
            impactExplosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            var proximityDetonatorGameObject = proximityDetonatorTransform.gameObject;

            var proximityDetonator = proximityDetonatorGameObject.AddComponent<MineProximityDetonator>();
            proximityDetonator.myTeamFilter = teamFilter;

            var helper = proximityDetonatorGameObject.AddComponent<ProjectileMineProximityDetonatorHelper>();
            helper.impactExplosion = impactExplosion;

            PrefabAPI.RegisterNetworkPrefab(projectilePrefab);

            return projectilePrefab;
        }

        public Material CreateBubbleMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_BarrierOnKill.matBarrier_mat).WaitForCompletion());
            material.name = "matSandCrabBubble";
            material.SetColor("_TintColor", new Color(107f/255f, 139f/255f, 1f, 1f));
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common.texCloudDifferenceBW1_png).WaitForCompletion());
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_ColorRamps.texRampDefault_png).WaitForCompletion());
            material.SetFloat("_InvFade", 2f);
            material.SetFloat("_Boost", 1f);
            material.SetFloat("_AlphaBoost", 1.540995f);
            material.SetFloat("_AlphaBias", 0.1219502f);
            material.SetVector("_CutoffScroll", new Vector4(1f, 3f, 1f, 0f));

            return material;
        }

        public Material CreateBubbleImpactRingMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.matOmniRing2_mat).WaitForCompletion());
            material.name = "matSandCrabBubbleImpactRing";
            material.SetColor("_TintColor", new Color(187f / 255f, 215f / 255f, 1f, 1f));
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_VFX.texOmniShockwave1Mask_png).WaitForCompletion());
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_ColorRamps.texRampDefault_png).WaitForCompletion());

            return material;
        }
    }
}
