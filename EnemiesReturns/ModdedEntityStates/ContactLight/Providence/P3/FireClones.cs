using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3
{
    [RegisterEntityState]
    public class FireClones : BaseState
    {
        public static float baseDuration = 3f;

        public static int minProjectileCount = 5;

        public static int maxProjectileCount = 10;

        public static float delayBetweenProjectiles = 0.2f;

        public static GameObject projectilePrefab;

        public static float predictionTime => Configuration.General.ProvidenceP1UtilityPredictionTimer.Value;

        private RoR2.Projectile.Predictor predictor;

        private float projectileTimer;

        private int projectileCount;

        private int projectilesFired;

        public override void OnEnter()
        {
            base.OnEnter();
            
            SetupPredictor();

            projectileCount = Mathf.Clamp((int)Mathf.Min(maxProjectileCount, Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.25f, healthComponent.fullHealth, (float)maxProjectileCount, (float)minProjectileCount)), minProjectileCount, maxProjectileCount);
            projectileTimer += delayBetweenProjectiles;
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

            if (projectilesFired >= projectileCount && fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
            }

            projectileTimer -= GetDeltaTime();
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
                damage = damageStat * 2f,
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
