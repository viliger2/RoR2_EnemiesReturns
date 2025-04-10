using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Archer
{
    [RegisterEntityState]
    public class FireArrow : BaseState
    {
        public static string targetMuzzle = "BowString";

        public static float projectileSpeed = 100f;

        public static float minimumDistance = 0f;

        public static float maximumDistance = 65f;

        public static float timeToTarget => EnemiesReturns.Configuration.LynxTribe.LynxArcher.FireArrowTimeToTarget.Value;

        public static float baseDuration = 2.0f;

        public static float baseShoot = 0.83f;

        public static float animationAimUpdate = 0.05f;

        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxArcher.FireArrowDamage.Value;

        public static float projectileForce => EnemiesReturns.Configuration.LynxTribe.LynxArcher.FireArrowForce.Value;

        public static GameObject projectilePrefab;

        private float duration;

        private Animator animator;

        private Transform muzzleTransform;

        private Transform arrowTransform;

        private float timer;

        private float shoot;

        private bool hasFired;

        private float aimTarget;

        private float aimCurrentVelocity;

        private Predictor predictor;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            shoot = baseShoot / attackSpeedStat;
            PlayCrossfade("Gesture", "ShotFull", "Shot.playbackRate", duration, 0.1f);
            Util.PlayAttackSpeedSound("ER_Archer_PrepareArrow_Play", gameObject, attackSpeedStat);

            animator = GetModelAnimator();
            animator.SetLayerWeight(animator.GetLayerIndex("AttackPitch"), 1f);

            aimTarget = 0f;
            aimCurrentVelocity = 0f;

            muzzleTransform = FindModelChild(targetMuzzle);
            arrowTransform = FindModelChild("Arrow");

            predictor = new Predictor(characterBody.transform);

            if (!muzzleTransform)
            {
                muzzleTransform = base.gameObject.transform;
            }

            if (characterBody)
            {
                characterBody.SetAimTimer(3f);
            }
        }

        public override void Update()
        {
            base.Update();
            timer += Time.deltaTime;
            if (timer > animationAimUpdate && !hasFired)
            {
                aimTarget = 1 - Mathf.Clamp01(Util.Remap(GetAimAngle(), 45f, 100f, 0f, 1f)); // 45 and 100 are aproximate angles of aim animation
                timer -= animationAimUpdate;
            }
            animator.SetFloat("Shot.aimPitchCycle", Mathf.SmoothDamp(animator.GetFloat("Shot.aimPitchCycle"), aimTarget, ref aimCurrentVelocity, 0.1f, 360f, Time.deltaTime)); // magic numbers from AimAnimator
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (predictor != null)
            {
                predictor.Update();
            }
            if (fixedAge >= shoot && !hasFired)
            {
                if (isAuthority)
                {
                    FireProjectile();
                }
                if (arrowTransform)
                {
                    arrowTransform.gameObject.SetActive(false);
                }
                Util.PlayAttackSpeedSound("ER_Archer_FireArrow_Play", gameObject, attackSpeedStat);
                hasFired = true;
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
            animator.SetLayerWeight(animator.GetLayerIndex("AttackPitch"), 0f);
            if (arrowTransform)
            {
                arrowTransform.gameObject.SetActive(true);
            }
        }

        private void FireProjectile()
        {
            Ray aimRay = GetAimRay();
            Ray ray = new Ray(aimRay.origin, Vector3.up);
            if (muzzleTransform)
            {
                ray.origin = muzzleTransform.position;
            }

            Vector3 vector;
            bool flag = false;
            if (!predictor.hasTargetTransform)
            {
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
                    vector = predictor.GetTargetTransform().position;
                    flag = true;
                }
                else
                {
                    vector = aimRay.origin + aimRay.direction * 1000f;
                    flag = true;
                }
            }
            else
            {
                predictor.GetPredictedTargetPosition(timeToTarget, out vector);
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
            ProjectileManager.instance.FireProjectile(projectilePrefab, ray.origin, rotation, gameObject, damageStat * damageCoefficient, projectileForce, RollCrit(), DamageColorIndex.Default, null, magnitude, DamageSource.Primary);
        }

        private float GetAimAngle()
        {
            Ray aimRay = GetAimRay();
            Ray ray = new Ray(aimRay.origin, Vector3.up);
            if (muzzleTransform)
            {
                ray.origin = muzzleTransform.position;
            }

            Vector3 vector;
            if (!predictor.hasTargetTransform)
            {
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
                    vector = predictor.GetTargetTransform().position;
                }
                else
                {
                    vector = aimRay.origin + aimRay.direction * 1000f;
                }
            }
            else
            {
                predictor.GetPredictedTargetPosition(shoot, out vector);
            }

            Vector3 vectorToTarget = vector - ray.origin;
            Vector2 vector2 = new Vector2(vectorToTarget.x, vectorToTarget.z);
            float magnitude2 = vector2.magnitude;
            Vector2 anotherVector2 = vector2 / magnitude2;
            if (magnitude2 < minimumDistance)
            {
                magnitude2 = minimumDistance;
            }
            if (magnitude2 > maximumDistance)
            {
                magnitude2 = maximumDistance;
            }
            float y = Trajectory.CalculateInitialYSpeed(timeToTarget, vectorToTarget.y);
            float num = magnitude2 / timeToTarget;
            Vector3 direction = new Vector3(anotherVector2.x * num, y, anotherVector2.y * num);

            return Vector3.Angle(Vector3.up, direction);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
