using EnemiesReturns.Behaviors;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static Rewired.UI.ControlMapper.ControlMapper;

namespace EnemiesReturns.Enemies.SandCrab
{
    public class SandCrabStuff
    {
        public GameObject CreateSnipEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion().InstantiateClone("SpitterBiteEffect", false);

            var particleSystem = clonedEffect.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.startRotationX = new ParticleSystem.MinMaxCurve(0f, 0f);
            main.startRotationY = new ParticleSystem.MinMaxCurve(140f, 140f);

            particleSystem.gameObject.transform.localScale = new Vector3(2f, 2f, 2f);

            return clonedEffect;
        }

        public GameObject CreateBubbleProjectile(GameObject projectilePrefab, AnimationCurveDef acdBubbleSpeed)
        {
            projectilePrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            var hurtBoxTransform = projectilePrefab.transform.Find("Model/HurtBox");
            var proximityDetonatorTransform = projectilePrefab.transform.Find("ProximityDetonator");

            var dissableCollisions = projectilePrefab.AddComponent<DisableCollisionsBetweenColliders>();
            dissableCollisions.collidersA = new Collider[] { projectilePrefab.GetComponent<SphereCollider>(), proximityDetonatorTransform.GetComponent<SphereCollider>() };
            dissableCollisions.collidersB = new Collider[] { hurtBoxTransform.GetComponent<SphereCollider>() };        

            var projectileController = projectilePrefab.AddComponent<ProjectileController>();
            //projectileController.ghostPrefab = ; // TODO
            //projectileController.flightSoundLoop = ; // TODO
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = 1f; // TODO

            var projectileNetworkTransform = projectilePrefab.AddComponent<ProjectileNetworkTransform>();
            projectileNetworkTransform.positionTransmitInterval = 0.03333334f;
            projectileNetworkTransform.interpolationFactor = 1f;
            projectileNetworkTransform.allowClientsideCollision = false;

            var projectileSimple = projectilePrefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 12f; // TODO: same value as on ProjectileImpactExplosion
            projectileSimple.desiredForwardSpeed = 5f; // TODO: curve multiplies this value, not assignes value by itself, so at the begining we can do like x5, then stop and then set it to x1
            projectileSimple.updateAfterFiring = true;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = acdBubbleSpeed.curve;
            //projectileSimple.lifetimeExpiredEffect = impact; // TODO
            projectileSimple.oscillate = false; // TODO: this is z oscillate, not y

            var oscillate = projectilePrefab.AddComponent<ProjectileOscillate>();
            oscillate.oscillateY = true;
            oscillate.oscillateMagnitude = 1.0f;
            oscillate.oscillateSpeed = 2f;
            oscillate.randomStartingOffset = true;

            //var projectileSingleTarget = projectilePrefab.AddComponent<ProjectileSingleTargetImpactDoNotCollideWithSameTeam>();
            //projectileSingleTarget.destroyWhenNotAlive = true;
            //projectileSingleTarget.destroyOnWorld = true;
            ////projectileSingleTarget.impactEffect = impact; // TODO
            ////projectileSingleTarget.hitSoundString = "ER_Shaman_Projectile_Impact_Play"; // TODO

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
            timer.duration = 12f * 0.125f; // TODO: 12 is duration, should be the same across all, 0.125f is middle of 0 speed of speed curve
            timer.resetTimerOnEnable = true;
            timer.playTimerOnEnable = true;
            timer.loop = false;
            timer.timeStepType = RoR2.EntityLogic.Timer.TimeStepType.FixedTime;
            timer.action = new UnityEngine.Events.UnityEvent();
            timer.action.AddPersistentListener(enabler.EnableComponent);

            var teamFilter = projectilePrefab.GetOrAddComponent<TeamFilter>();

            var characterBody = projectilePrefab.AddComponent<CharacterBody>();
            characterBody.baseMaxHealth = 30f;
            characterBody.levelMaxHealth = 9f;
            characterBody.bodyFlags = CharacterBody.BodyFlags.Masterless;
            characterBody.doNotReassignToTeamBasedCollisionLayer = true;

            var teamComponent = projectilePrefab.GetOrAddComponent<TeamComponent>();
            teamComponent.hideAllyCardDisplay = true;

            var assigner = projectilePrefab.AddComponent<AssignTeamFilterToTeamComponent>();

            var healthComponent = projectilePrefab.GetOrAddComponent<HealthComponent>();
            healthComponent.body = characterBody;

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
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            impactExplosion.blastRadius = 7f;
            impactExplosion.blastDamageCoefficient = 1f;
            impactExplosion.blastProcCoefficient = 1f;
            impactExplosion.projectileHealthComponent = healthComponent;

            //impactExplosion.impactEffect = ; // TODO
            impactExplosion.destroyOnEnemy = true;
            impactExplosion.destroyOnWorld = true;
            impactExplosion.impactOnWorld = true;
            impactExplosion.timerAfterImpact = false;
            impactExplosion.lifetime = 12f;
            impactExplosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            var proximityDetonatorGameObject = proximityDetonatorTransform.gameObject;

            var proximityDetonator = proximityDetonatorGameObject.AddComponent<MineProximityDetonator>();
            proximityDetonator.myTeamFilter = teamFilter;

            var helper = proximityDetonatorGameObject.AddComponent<ProjectileMineProximityDetonatorHelper>();
            helper.impactExplosion = impactExplosion;

            //var proximityDetonator = proximityDetonatorGameObject.AddComponent<Behaviors.MineProximityDetonatorWithGameObjectCheck>();
            //proximityDetonator.myTeamFilter = teamFilter;

            //var helper = proximityDetonatorGameObject.AddComponent<ProjectileMineProximityDetonatorHelper>();
            //helper.impactExplosion = impactExplosion;

            PrefabAPI.RegisterNetworkPrefab(projectilePrefab);

            return projectilePrefab;
        }
    }
}
