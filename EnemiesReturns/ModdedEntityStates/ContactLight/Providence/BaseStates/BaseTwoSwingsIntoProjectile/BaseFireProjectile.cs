using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile
{
    public abstract class BaseFireProjectiles : BaseState
    {
        public abstract float baseDuration { get; }

        public abstract float baseSpawnProjectiles { get; }

        public abstract float damageCoefficient { get; }

        public abstract int projectileCount { get; }

        public abstract float baseProjectileDelay { get; }

        public abstract GameObject effectPrefab { get; }

        public abstract GameObject projectilePrefab { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public abstract string[] originChildNames { get; }

        private float duration;

        private float spawnTime;

        private float projectileDelay;

        private float projectileTimer;

        private Transform[] originTransforms;

        private int projectileFiredCount;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            spawnTime = baseSpawnProjectiles / attackSpeedStat;
            projectileDelay = baseProjectileDelay / attackSpeedStat;

            originTransforms = Array.ConvertAll(originChildNames, childName => FindModelChild(childName));

            foreach (var origin in originTransforms)
            {
                if (origin && effectPrefab)
                {
                    var chargeEffectInstance = UnityEngine.Object.Instantiate(effectPrefab, origin.position, origin.rotation);
                    chargeEffectInstance.transform.parent = origin;
                    chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = spawnTime;
                }
            }

            PlayCrossfade(layerName, animationStateName, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > spawnTime && isAuthority && projectilePrefab && projectileFiredCount < projectileCount)
            {
                if (projectileTimer <= 0)
                {
                    var aimVector = GetAimRay();
                    var projectileInfo = new FireProjectileInfo()
                    {
                        crit = RollCrit(),
                        damage = damageStat * damageCoefficient,
                        damageTypeOverride = DamageTypeCombo.GenericPrimary,
                        owner = gameObject,
                        rotation = Util.QuaternionSafeLookRotation(aimVector.direction),
                        projectilePrefab = projectilePrefab,
                    };
                    var currentOriginTransform = originTransforms[projectileFiredCount % originTransforms.Length];
                    if (currentOriginTransform)
                    {
                        projectileInfo.position = currentOriginTransform.position;
                        ProjectileManager.instance.FireProjectile(projectileInfo);
                    }

                    projectileFiredCount++;
                    projectileTimer += projectileDelay;
                }
                projectileTimer -= GetDeltaTime();
            }
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
