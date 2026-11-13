using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class LeftRightSwing : BaseLeftRightSwing
    {
        public override float damageCoefficient => ProvidencePhase2.P2PrimaryDamage.Value;

        public override float baseDuration => ProvidencePhase2.P2PrimaryDuration.Value;

        public override string hitBoxGroupName => "SwordHitbox";

        public override float procCoefficient => ProvidencePhase2.P2PrimaryProcCoefficient.Value;

        public override float pushAwayForce => ProvidencePhase2.P2PrimaryPushAwayForce.Value;

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

        public override float earlyExit => ProvidencePhase2.P2PrimaryEarlyExit.Value;

        public override string swingEffectMuzzleString { get => _swingEffectMuzzleString; set => _swingEffectMuzzleString = value; }

        public static string fireCloneMecanimParam = "FireClone.attack";

        public static GameObject cloneProjectile;

        public static float maxSearchDistance => ProvidencePhase2.P2PrimaryCloneSearchDistance.Value;

        public static float maxSearchAngle => ProvidencePhase2.P2PrimaryCloneMaxSearchAngle.Value;

        public static float cloneProjectileSpeed => ProvidencePhase2.P2PrimaryCloneSpeed.Value;

        public static float cloneProjectileDamage => ProvidencePhase2.P2PrimaryCloneDamage.Value;

        public static float projectileHealthThreshold => ProvidencePhase2.P2PrimaryCloneHealthThreshold.Value;

        private string _swingEffectMuzzleString;

        private bool firedClone;

        public override EntityState GetNextStateIfMissed()
        {
            return new FireProjectiles();
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("UpperBodyOnly", "SlashP2", "combo.playbackRate", duration, 0.1f);
        }

        public override void AuthorityFixedUpdate()
        {
            base.AuthorityFixedUpdate();
            if (animator.GetFloat(fireCloneMecanimParam) > 0.9f && healthComponent.healthFraction <= projectileHealthThreshold && !firedClone)
            {
                FireCloneProjectile();
                firedClone = true;
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
