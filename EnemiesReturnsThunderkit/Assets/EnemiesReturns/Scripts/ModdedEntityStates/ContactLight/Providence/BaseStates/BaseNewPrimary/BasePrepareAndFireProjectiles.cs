using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary
{
    public abstract class BasePrepareAndFireProjectiles : BaseState
    {
        public static GameObject effectPrefab;

        public static GameObject projectilePrefab;

        public abstract float baseDuration { get; }

        public abstract float baseStartProjectileDuration { get; }

        public abstract float baseProjectileAnimationDuration { get; }

        public abstract float baseProjectileDelay { get; }

        public abstract float damageCoefficient { get; }

        public abstract string[] originChildNames { get; }

        public abstract int projectileCount { get; }

        private float duration;

        private float startProjectileDuration;

        private float projectileAnimationDuration;

        private float projectileDelay;

        private float projectileTimer;

        private bool startedSecondaryAnimation;

        private bool startedSpawningProjectiles;

        private Animator animator;

        private Transform[] originTransforms;

        private int projectileFiredCount;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            startProjectileDuration = baseStartProjectileDuration / attackSpeedStat;
            projectileAnimationDuration = baseProjectileAnimationDuration / attackSpeedStat;
            projectileDelay = baseProjectileDelay / attackSpeedStat;
            projectileTimer = 0f;

            animator = GetModelAnimator();

            originTransforms = Array.ConvertAll(originChildNames, childName => FindModelChild(childName));

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
                    startedSecondaryAnimation = true;
                }
                if(animator && animator.GetFloat("FireProjectiles.attack") > 0.9f)
                {
                    startedSpawningProjectiles = true;
                }
            }

            if (startedSpawningProjectiles)
            {
                if(projectileTimer <= 0f)
                {
                    if (isAuthority)
                    {
                        FireProjectile();
                    }
                    projectileFiredCount++;
                    projectileTimer += projectileDelay;
                }
            }

            if(isAuthority && fixedAge > duration && projectileFiredCount >= projectileCount)
            {
                outer.SetNextState(GetNextState());
            }

            characterDirection.forward = GetAimRay().direction.normalized;
        }

        public abstract EntityState GetNextState();

        private void FireProjectile()
        {
            var currentOriginTransform = originTransforms[projectileFiredCount % originTransforms.Length];
            if (currentOriginTransform)
            {
                if (currentOriginTransform.childCount > 0)
                {
                    UnityEngine.Object.Destroy(currentOriginTransform.GetChild(0).gameObject);
                }
                if (isAuthority)
                {
                    var aimVector = GetAimRay();
                    var projectileInfo = new FireProjectileInfo
                    {
                        crit = RollCrit(),
                        damage = damageStat * damageCoefficient,
                        damageTypeOverride = DamageTypeCombo.GenericPrimary,
                        owner = gameObject,
                        rotation = Util.QuaternionSafeLookRotation(aimVector.direction),
                        projectilePrefab = projectilePrefab,
                        position = currentOriginTransform.position
                    };
                    ProjectileManager.instance.FireProjectile(projectileInfo);
                }
            }
        }

        private void SpawnEffects()
        {
            foreach (var origin in originTransforms)
            {
                if (origin && effectPrefab)
                {
                    var chargeEffectInstance = UnityEngine.Object.Instantiate(effectPrefab, origin.position, origin.rotation);
                    chargeEffectInstance.transform.parent = origin;
                    Util.PlaySound("ER_Providence_ProjectileEffect_Spawn_Play", origin.gameObject);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
