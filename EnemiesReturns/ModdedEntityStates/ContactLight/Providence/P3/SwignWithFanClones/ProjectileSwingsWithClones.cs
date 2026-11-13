using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseProjectilePrimary;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
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
    public class ProjectileSwingsWithClones : BaseLeftRightSwing
    {
        public override float damageCoefficient => ProvidencePhase3.P3PrimaryDamage.Value;

        public override float baseDuration => ProvidencePhase3.P3PrimaryDuration.Value;

        public override string hitBoxGroupName => "SwordHitbox";

        public override float procCoefficient => ProvidencePhase3.P3PrimaryProcCoefficient.Value;

        public override float pushAwayForce => ProvidencePhase3.P3PrimaryPushAwayForce.Value;

        public override Vector3 forceVector => Vector3.zero;

        public override float hitPauseDuration => 0.1f;

        public override string mecanimHitboxActiveParameter => "Slash1.attack";

        public override float shorthopVelocityFromHit => 0f;

        public override string beginStateSoundString => ""; // TODO

        public override string beginSwingSoundString => "ER_Providence_Swing_Play"; // TODO

        public override bool forceForwardVelocity => false;

        public override bool scaleHitPauseDurationAndVelocityWithAttackSpeed => false;

        public override bool ignoreAttackSpeed => false;

        public override DamageTypeCombo damageType => DamageTypeCombo.GenericPrimary;

        public override float earlyExit => 3.5f;

        public override string swingEffectMuzzleString { get => _swingEffectMuzzleString; set => _swingEffectMuzzleString = value; }

        public static string fireCloneMecanimParam = "FireClone.attack";

        public static GameObject projectilePrefab;

        public static float maxSearchDistance => ProvidencePhase3.P3PrimaryCloneSearchDistance.Value;

        public static float maxSearchAngle => ProvidencePhase3.P3PrimaryCloneMaxSearchAngle.Value;

        public static float cloneProjectileSpeed => ProvidencePhase3.P3PrimaryCloneSpeed.Value;

        public static float cloneProjectileDamage => ProvidencePhase3.P3PrimaryCloneDamage.Value;

        public static float projectileSpread => ProvidencePhase3.P3PrimaryCloneSpread.Value;

        public static int cloneCount => ProvidencePhase3.P3PrimaryCloneCount.Value;

        private bool firedClone;

        private string _swingEffectMuzzleString;

        public override EntityState GetNextStateIfMissed()
        {
            return new GenericCharacterMain();
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("UpperBodyOnly", "Slash", "combo.playbackRate", duration, 0.1f);
        }

        public override void AuthorityFixedUpdate()
        {
            base.AuthorityFixedUpdate();
            if (animator.GetFloat(fireCloneMecanimParam) > 0.9f && !firedClone)
            {
                FireCloneProjectile();
                firedClone = true;
            }
        }

        private void FireCloneProjectile()
        {
            var distance = maxSearchDistance;

            var aimRay = GetAimRay();
            var target = FindTarget(aimRay);
            if (target)
            {
                distance = Vector3.Distance(aimRay.origin, target.transform.position);
            }

            Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
            Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

            float angle = projectileSpread / (cloneCount - 1);

            Vector3 direction = Quaternion.AngleAxis(-projectileSpread * 0.5f, axis) * aimRay.direction;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);

            Ray aimRay2 = new Ray(aimRay.origin, direction);

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
                    rotation = Util.QuaternionSafeLookRotation(aimRay2.direction),
                    useFuseOverride = true,
                    useSpeedOverride = true,
                    speedOverride = cloneProjectileSpeed,
                    projectilePrefab = projectilePrefab
                };
                ProjectileManager.instance.FireProjectile(projectileInfo);

                aimRay2.direction = rotation * aimRay2.direction;
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

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("UpperBodyOnly", "BufferEmpty", 0.1f);
        }
    }
}
