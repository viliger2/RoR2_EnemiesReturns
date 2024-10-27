using EnemiesReturns.Configuration;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class FireChargedSpit : BaseState
    {
        public static float baseDuration = 2.1f;

        public static float baseChargeTime = 0.2f;

        public static string targetMuzzle = "MuzzleMouth";

        public static GameObject projectilePrefab;

        public static float projectileSpeed = 55f;

        public static float minimumDistance = 0f;

        public static float maximumDistance = 70f;


        public static float timeToTarget => EnemiesReturns.Configuration.Spitter.ChargedProjectileFlyTime.Value; // used to calculate projectile speed when we were able to find the target in direct vision, othewise projectileSpeed is used

        public static float damageCoefficient => EnemiesReturns.Configuration.Spitter.ChargedProjectileDamage.Value;

        public static float projectileForce => EnemiesReturns.Configuration.Spitter.ChargedProjectileForce.Value;

        private float duration;

        private float chargeTime;

        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            chargeTime = baseChargeTime / attackSpeedStat;
            Util.PlaySound("ER_Spiiter_Spit_Play", gameObject);
            PlayCrossfade("Gesture, Override", "FireChargedSpit", "ChargedSpit.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge <= chargeTime)
            {
                return;
            }

            if (!hasFired && base.isAuthority)
            {
                FireProjectile();
                hasFired = true;
            }

            if (base.isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
            if (characterBody)
            {
                characterBody.SetAimTimer(1f);
            }
        }

        // this entire thing is pretty much copy paste of Mushrum's FireGrenade
        private void FireProjectile()
        {
            // getting aim ray to find where creature is looking
            Ray aimRay = GetAimRay();
            // I have no idea what's the purpose of this ray
            Ray ray = new Ray(aimRay.origin, Vector3.up);
            // getting position of projectile's initial point
            var transform = FindModelChild(targetMuzzle);
            if (transform)
            {
                ray.origin = transform.position;
            }

            // bullseyesearch is a cone by default
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

            // getting first hitbox that we found
            var hurtBox = search.GetResults().FirstOrDefault();

            // flag for direction, or something
            bool flag = false;

            // checking if it exists and if it doesn't just shooting a ray from creature to target
            Vector3 vector = Vector3.zero;
            if (hurtBox)
            {
                vector = hurtBox.transform.position;
                flag = true;
            }
            else if (Physics.Raycast(aimRay, out var hitInfo, 1000f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore))
            {
                vector = hitInfo.point;
                flag = true;
            }

            // getting magnitude
            float magnitude = projectileSpeed;
            if (flag)
            {
                // getting vector between target and creature
                Vector3 vectorToTarget = vector - ray.origin;
                // making Vector2 out of that so we can calculate magnitude
                Vector2 vector2 = new Vector2(vectorToTarget.x, vectorToTarget.z);
                float magnitude2 = vector2.magnitude;
                // dividing vector by its own magnitude for some reason
                Vector2 anotherVector2 = vector2 / magnitude2;
                // limiting projectile range
                if (magnitude2 < minimumDistance)
                {
                    magnitude2 = minimumDistance;
                }
                if (magnitude2 > maximumDistance)
                {
                    magnitude2 = maximumDistance;
                }
                // getting initial speed
                float y = Trajectory.CalculateInitialYSpeed(timeToTarget, vectorToTarget.y);
                float num = magnitude2 / timeToTarget;
                // finally getting direction for projectile
                Vector3 direction = new Vector3(anotherVector2.x * num, y, anotherVector2.y * num);
                magnitude = direction.magnitude;
                ray.direction = direction;
            }

            Quaternion rotation = Util.QuaternionSafeLookRotation(ray.direction);
            ProjectileManager.instance.FireProjectile(projectilePrefab, ray.origin, rotation, gameObject, damageStat * damageCoefficient, projectileForce, RollCrit(), DamageColorIndex.Default, null, magnitude);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
