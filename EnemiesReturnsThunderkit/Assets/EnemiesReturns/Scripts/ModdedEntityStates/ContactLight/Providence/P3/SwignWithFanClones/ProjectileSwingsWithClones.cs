using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseProjectilePrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.SwignWithFanClones
{
    [RegisterEntityState]
    public class ProjectileSwingsWithClones : BasePrimaryWeaponSwing
    {
        public static GameObject projectilePrefab;

        public static int cloneCount = 3;

        public static float defaultDistance = 50f;

        public static float maxAngle = 75;

        public static float cloneProjectileDamage = 2f;

        public static float cloneProjectileSpeed = 50f;

        public override float swingDamageCoefficient => 2f;

        public override float swingProcCoefficient => 1f;

        public override float swingForce => 0f;

        public override GameObject hitEffect => null;

        public override string swingSoundEffect => "";

        private bool clonesFired;

        public override void AuthorityFireAttack()
        {
            base.AuthorityFireAttack();
            if (!clonesFired)
            {
                var distance = defaultDistance;

                var aimRay = GetAimRay();
                var target = FindTarget(aimRay);
                if (target)
                {
                    distance = Vector3.Distance(aimRay.origin, target.transform.position);
                }

                Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                Vector3 axis = Vector3.Cross(aimRay.direction, rhs);
                float angle = 45f / (cloneCount - 1);

                Vector3 direction = Quaternion.AngleAxis((0f - 1) * 0.5f, axis) * aimRay.direction;
                Quaternion quaternion = Quaternion.AngleAxis(angle, axis);

                for (int i = 0; i < cloneCount; i++)
                {
                    var projectileInfo = new RoR2.Projectile.FireProjectileInfo()
                    {
                        crit = RollCrit(),
                        damage = damageStat * cloneProjectileDamage,
                        damageTypeOverride = DamageTypeCombo.GenericPrimary,
                        fuseOverride = distance / cloneProjectileSpeed,
                        maxDistance = distance,
                        owner = gameObject,
                        position = aimRay.origin,
                        rotation = quaternion,
                        useFuseOverride = true,
                        useSpeedOverride = true,
                        speedOverride = cloneProjectileSpeed,
                        projectilePrefab = projectilePrefab
                    };
                    ProjectileManager.instance.FireProjectile(projectileInfo);

                    quaternion = Util.QuaternionSafeLookRotation(quaternion * direction);
                }


                clonesFired = true;
            }
        }

        private GameObject FindTarget(Ray aimRay)
        {
            BullseyeSearch search = new BullseyeSearch();
            search.teamMaskFilter = TeamMask.allButNeutral;
            if (teamComponent)
            {
                search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            }
            search.maxDistanceFilter = defaultDistance;
            search.maxAngleFilter = maxAngle;
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
