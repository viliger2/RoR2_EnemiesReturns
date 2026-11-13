using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class FireClones : BaseState
    {
        public static GameObject predictedPositionEffect;

        public static float predictionEffectMinusTimer = 1f;

        public static float projectileLifetime = 1.5f;

        public static float projectileDamage => ProvidencePhase2.P2UtilityCloneDamage.Value;

        public static int minProjectileCount => ProvidencePhase2.P2UtilityCloneMinCount.Value;

        public static int maxProjectileCount => ProvidencePhase2.P2UtilityCloneMaxCount.Value;

        public static float delayBetweenProjectiles => ProvidencePhase2.P2UtilityCloneDelayBetweenProjectiles.Value;

        public static float minProjectileHealth => ProvidencePhase2.P2UtilityCloneMinHealthThreshold.Value;

        public static float maxProjectileHealth => ProvidencePhase2.P2UtilityCloneMaxHealthThreshold.Value;

        public static float predictionTime => ProvidencePhase2.P2UtilityPredictionTimer.Value;

        public Vector3 predictedPosition;

        private CharacterModel characterModel;

        private HurtBoxGroup hurtboxGroup;

        private int originalLayer;

        internal float predictionEffectTimer;

        private bool spawnedPrediction;

        public static GameObject projectilePrefab;

        private RoR2.Projectile.Predictor predictor;

        private float projectileTimer;

        private int projectileCount;

        private int projectilesFired;

        private bool playedSound;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            if (NetworkServer.active)
            {
                CleanseSystem.CleanseBodyServer(base.characterBody, true, false, false, true, false, false);
                base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            if (modelTransform)
            {
                characterModel = modelTransform.GetComponent<CharacterModel>();
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }

            if (characterModel)
            {
                characterModel.invisibilityCount++;
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter++;
            }
            originalLayer = base.gameObject.layer;
            base.gameObject.layer = LayerIndex.GetAppropriateFakeLayerForTeam(base.teamComponent.teamIndex).intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            
            SetupPredictor();

            projectileCount = Mathf.Clamp((int)Mathf.Min(maxProjectileCount, Util.Remap(healthComponent.health, healthComponent.fullHealth * maxProjectileHealth, healthComponent.fullHealth * minProjectileHealth, (float)maxProjectileCount, (float)minProjectileCount)), minProjectileCount, maxProjectileCount);
            projectileTimer += delayBetweenProjectiles;

            predictionEffectTimer = projectileCount * delayBetweenProjectiles + projectileLifetime - predictionEffectMinusTimer;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (predictor != null && !predictor.hasTargetTransform)
            {
                FindTarget();
            }
            else
            {
                predictor.Update();
            }

            if (fixedAge >= predictionEffectTimer && !spawnedPrediction)
            {
                if (isAuthority)
                {
                    predictedPosition = transform.position;
                    if (predictor.hasTargetTransform)
                    {
                        predictor.GetPredictedTargetPosition(predictionTime, out predictedPosition);
                    }
                    var closestNode = SceneInfo.instance.groundNodes.FindClosestNode(predictedPosition, HullClassification.Golem);
                    if (closestNode != RoR2.Navigation.NodeGraph.NodeIndex.invalid)
                    {
                        if (SceneInfo.instance.groundNodes.GetNodePosition(closestNode, out var nodePosition))
                        {
                            predictedPosition = nodePosition;
                        }
                    }

                    EffectManager.SimpleEffect(predictedPositionEffect, predictedPosition, Quaternion.identity, true);
                }
                spawnedPrediction = true;
            }

            if (projectileTimer <= 0f && projectilesFired < projectileCount)
            {
                if (isAuthority)
                {
                    predictor.GetPredictedTargetPosition(predictionTime, out var position);
                    FireProjectileAuthority(position);
                    projectileTimer += delayBetweenProjectiles;
                }
                projectilesFired++;
            }

            if(fixedAge > predictionEffectTimer + projectileLifetime - 1 && !playedSound)
            {
                Util.PlaySound("ER_Providence_GroundSlam_Play", base.gameObject);
                playedSound = true;
            }

            if ((projectilesFired >= projectileCount && fixedAge > predictionEffectTimer + projectileLifetime) && isAuthority)
            {
                base.characterMotor.Motor.SetPositionAndRotation(predictedPosition + Vector3.up * 0.05f, Quaternion.identity);
                outer.SetNextState(new Attack());
            }

            projectileTimer -= GetDeltaTime();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
            if (characterModel)
            {
                characterModel.invisibilityCount--;
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
            base.gameObject.layer = originalLayer;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        private void SetupPredictor()
        {
            predictor = new RoR2.Projectile.Predictor(characterBody.transform);
            FindTarget();
        }

        private void FireProjectileAuthority(Vector3 position)
        {
            if (!isAuthority)
            {
                return;
            }

            var info = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damageStat * projectileDamage,
                damageTypeOverride = DamageSource.Utility,
                owner = gameObject,
                position = position,
                rotation = Quaternion.identity,
                projectilePrefab = projectilePrefab
            };

            ProjectileManager.instance.FireProjectile(info);
        }

        private void FindTarget()
        {
            var aimRay = GetAimRay();
            BullseyeSearch search = new BullseyeSearch()
            {
                searchOrigin = aimRay.origin,
                searchDirection = aimRay.direction,
                filterByLoS = false,
                teamMaskFilter = TeamMask.allButNeutral,
                sortMode = BullseyeSearch.SortMode.Angle
            };
            if (teamComponent)
            {
                search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            }
            search.RefreshCandidates();

            var hurtBox = search.GetResults().FirstOrDefault();

            if (hurtBox)
            {
                predictor.SetTargetTransform(hurtBox.transform);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
