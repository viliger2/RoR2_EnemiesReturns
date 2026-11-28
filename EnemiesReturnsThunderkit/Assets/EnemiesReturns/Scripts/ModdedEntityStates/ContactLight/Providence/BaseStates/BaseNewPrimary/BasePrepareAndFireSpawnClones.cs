using EnemiesReturns.Behaviors.CutsceneHelpers;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
using static RoR2.BrotherUltPattern;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary
{
    public abstract class BasePrepareAndFireSpawnClones : BaseState
    {
        public class Pattern
        {
            public Vector3 center { get; set; }

            public Vector3 rotation { get; set; } = Vector3.forward;

            public virtual Vector3[] Generate()
            {
                return new Vector3[] { center };
            }
        }

        public class Circle : Pattern
        {
            public int count = 8;

            public float radius = 11.5f;

            public override Vector3[] Generate()
            {
                if (count <= 0) return new Vector3[0];

                Vector3[] points = new Vector3[count];
                float angleStep = 360f / count;

                for (int i = 0; i < count; i++)
                {
                    float angle = (i * angleStep) * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                    points[i] = center + offset;
                }

                return points;
            }
        }

        public class Cross : Pattern
        {
            public int pointsPerArm = 2;

            public float distance = 8.5f;

            public float rotationInternal = 45f;

            public override Vector3[] Generate()
            {
                if (pointsPerArm < 0) pointsPerArm = 0;

                Vector3[] points = new Vector3[1 + pointsPerArm * 4];

                int index = 0;
                points[index++] = center;

                Quaternion rot = Quaternion.AngleAxis(rotationInternal, Vector3.up);
                var axisA = rot * Vector3.right;
                var axisB = rot * Vector3.forward;

                for (int i = 1; i <= pointsPerArm; i++)
                {
                    float d = distance * i;

                    points[index++] = center + axisA * d; // right
                    points[index++] = center - axisA * d; // left
                    points[index++] = center + axisB * d; // forward / up
                    points[index++] = center - axisB * d; // back  / down
                }

                return points;
            }
        }

        public class Triangle : Pattern
        {
            public int rowCount = 3;

            public float distanceBetweenRows = 6.5f;

            public float distanceBetweenPoints = 8f;

            public override Vector3[] Generate()
            {
                if (rowCount <= 0) return new Vector3[0];

                int total = rowCount * (rowCount + 1) / 2;
                Vector3[] points = new Vector3[total];

                float rowCenterOffset = (rowCount - 1) * 0.5f;

                float rotation = Vector3.Angle(this.rotation, Vector3.forward);

                Quaternion rot = Quaternion.AngleAxis(rotation, Vector3.up);
                var axisA = rot * Vector3.right;
                var axisB = rot * Vector3.forward;

                int index = 0;
                for (int r = 0; r < rowCount; r++)
                {
                    int pointsInRow = r + 1;
                    float v = (r - rowCenterOffset) * distanceBetweenRows;
                    float halfSpan = r * 0.5f;

                    for (int i = 0; i < pointsInRow; i++)
                    {
                        float u = (i - halfSpan) * distanceBetweenPoints;
                        points[index++] = center + axisA * u + axisB * v;
                    }
                }

                return points;
            }
        }

        //public static Pattern[] patterns = new Pattern[]
        //{
        //    new Circle()
        //    {
        //        count = 8,
        //        radius = 11.5f,
        //    },
        //    new Cross()
        //    {
        //        pointsPerArm = 2,
        //        distance = 8.5f,
        //        rotationInternal = 0f
        //    },
        //    new Cross()
        //    {
        //        pointsPerArm = 2,
        //        distance = 8.5f,
        //        rotationInternal = 45f
        //    },
        //    new Triangle()
        //    {
        //        rowCount = 3,
        //        distanceBetweenPoints = 8f,
        //        distanceBetweenRows = 6.5f
        //    }
        //};

        public abstract Pattern[] patterns { get; }

        public static GameObject effectPrefab;

        public static GameObject projectilePrefab;

        public abstract float baseDuration { get; }

        public abstract float baseStartProjectileDuration { get; }

        public abstract float baseProjectileAnimationDuration { get; }

        public abstract float damageCoefficient { get; }

        public abstract float projectileLifetime { get; }

        public static float baseAdditionalEffectDuration = 1.25f; // how long it takes in default animation to go from start of animation to firing projectiles

        private float duration;

        private float startProjectileDuration;

        private float projectileAnimationDuration;

        private bool startedSecondaryAnimation;

        private bool startedSpawningProjectiles;

        private Animator animator;

        private Vector3[] spawnPoints;

        private Transform originPoint;

        private float additionalEffectDuration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            startProjectileDuration = baseStartProjectileDuration / attackSpeedStat;
            projectileAnimationDuration = baseProjectileAnimationDuration / attackSpeedStat;
            additionalEffectDuration = baseAdditionalEffectDuration / attackSpeedStat;

            animator = GetModelAnimator();

            if (isAuthority)
            {
                var pattern = patterns[UnityEngine.Random.Range(0, patterns.Length)];
                pattern.center = FindTarget();
                pattern.rotation = GetAimRay().direction;
                spawnPoints = pattern.Generate();
            }

            originPoint = FindModelChild("ClonesSpawnOrigin");
            if (!originPoint)
            {
                originPoint = gameObject.transform;
            }

            PlayCrossfade("Gesture, Override", "SwingPrep", "combo.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > startProjectileDuration)
            {
                if (!startedSecondaryAnimation)
                {
                    PlayAnimation("Projectile, Override", "FireProjectile", "FireProjectiles.playbackRate", projectileAnimationDuration);
                    if (isAuthority)
                    {
                        SpawnEffects();
                    }
                    startedSecondaryAnimation = true;
                }
                if(animator && animator.GetFloat("FireProjectiles.attack") > 0.9f)
                {
                    if (!startedSpawningProjectiles)
                    {
                        if (isAuthority)
                        {
                            FireProjectiles();
                        }
                    }
                    startedSpawningProjectiles = true;
                }
            }

            if(isAuthority && fixedAge > duration && startedSpawningProjectiles)
            {
                outer.SetNextState(GetNextState());
            }

            characterDirection.forward = GetAimRay().direction.normalized;
        }

        public abstract EntityState GetNextState();

        private void FireProjectiles()
        {
            foreach(var target in spawnPoints)
            {
                var distance = Vector3.Distance(target, originPoint.position);

                var projectileInfo = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    damage = damageStat * damageCoefficient,
                    fuseOverride = projectileLifetime,
                    maxDistance = distance,
                    owner = gameObject,
                    position = originPoint.position,
                    rotation = Util.QuaternionSafeLookRotation((target - originPoint.position).normalized),
                    useFuseOverride = true,
                    useSpeedOverride = true,
                    speedOverride = distance / projectileLifetime,
                    projectilePrefab = projectilePrefab,
                    damageTypeOverride = DamageTypeCombo.GenericPrimary
                };

                ProjectileManager.instance.FireProjectile(projectileInfo);
            }
        }

        private void SpawnEffects()
        {
            if (!effectPrefab)
            {
                return;
            }

            foreach (var target in spawnPoints)
            {
                if (effectPrefab)
                {
                    var effectData = new EffectData()
                    {
                        genericFloat = projectileLifetime + additionalEffectDuration, 
                        origin = target,
                        rotation = Quaternion.identity,
                    };

                    EffectManager.SpawnEffect(effectPrefab, effectData, true);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private Vector3 FindTarget()
        {
            BullseyeSearch search = new BullseyeSearch();
            search.teamMaskFilter = TeamMask.allButNeutral;
            if (teamComponent)
            {
                search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            }
            search.maxDistanceFilter = 9999f;
            search.maxAngleFilter = 90f;
            var aimRay = GetAimRay();
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.filterByLoS = false;
            search.sortMode = BullseyeSearch.SortMode.Angle;
            search.RefreshCandidates();
            //search.FilterByPlayers();
            var hurtBox = search.GetResults().FirstOrDefault();
            if (hurtBox)
            {
                return hurtBox.healthComponent.body.corePosition;
            }
            else
            {
                foreach (var ai in characterBody.master.aiComponents)
                {
                    if (!ai.currentEnemy.characterBody)
                    {
                        continue;
                    }

                    return ai.currentEnemy.characterBody.corePosition;
                }
            }

            return aimRay.origin + aimRay.direction * 30f;
        }

    }
}
