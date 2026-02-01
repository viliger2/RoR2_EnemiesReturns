using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class LeftRightSwing : BaseLeftRightSwing
    {
        public override float baseFirstSwing => 1.5f;

        public override float baseSecondSwing => 1.5f;

        public override float damageCoefficient => 2f;

        public override string layerName => "UpperBodyOnly";

        public override string firstSwingStateName => "Slash1";

        public override string secondSwingStateName => "Slash2";

        public override string playbackParam => "combo.playbackRate";

        public override string hitboxName => "Sword";

        public override string firstAttackParam => "Slash1.attack";

        public override string secondAttackParam => "Slash2.attack";

        public static GameObject cloneProjectile;

        public static float cloneFireDelay = 0.2f;

        public static float maxSearchDistance = 250f;

        public static float maxSearchAngle = 75f;

        public static float cloneProjectileSpeed = 50f;

        public static float cloneProjectileDamage = 2f;

        public static float projectileHealthThreshold = 0.85f;

        private bool firedClone;

        private float cloneTimer;

        public override EntityState GetNextStateIfMissed()
        {
            return new FireProjectiles();
        }

        public override void FireSecondAttack()
        {
            base.FireSecondAttack();
            if (healthComponent.healthFraction <= projectileHealthThreshold)
            {
                CheckAndFireClone();
            }
        }

        private void CheckAndFireClone()
        {
            if (!firedClone)
            {
                if (cloneTimer <= 0)
                {
                    FireCloneProjectile();
                    firedClone = true;
                }
                cloneTimer -= GetDeltaTime();
            }
        }

        private void FireCloneProjectile()
        {
            var aimRay = GetAimRay();
            var target = FindTarget(aimRay);
            if (!target)
            {
                return;
            }

            var distance = Vector3.Distance(aimRay.origin, target.transform.position);

            var projectileInfo = new RoR2.Projectile.FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damageStat * cloneProjectileDamage,
                damageTypeOverride = DamageTypeCombo.GenericPrimary,
                fuseOverride = distance / cloneProjectileSpeed,
                maxDistance = distance,
                owner = gameObject,
                position = aimRay.origin,
                rotation = Util.QuaternionSafeLookRotation((target.transform.position - aimRay.origin).normalized),
                useFuseOverride = true,
                useSpeedOverride = true,
                speedOverride = cloneProjectileSpeed,
                projectilePrefab = cloneProjectile
            };

            ProjectileManager.instance.FireProjectile(projectileInfo);
        }

        private GameObject FindTarget(Ray aimRay)
        {
            BullseyeSearch search = new BullseyeSearch();
            search.teamMaskFilter = TeamMask.allButNeutral;
            if (teamComponent)
            {
                search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            }
            search.maxDistanceFilter = maxSearchDistance;
            search.maxAngleFilter = maxSearchAngle;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.RefreshCandidates();
            var hurtBox = search.GetResults().FirstOrDefault();
            if (hurtBox)
            {
                return hurtBox.healthComponent.gameObject;
            }

            return null;
        }

    }
}
