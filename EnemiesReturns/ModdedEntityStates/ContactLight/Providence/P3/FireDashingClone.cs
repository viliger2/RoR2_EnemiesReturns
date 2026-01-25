using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3
{
    [RegisterEntityState]
    public class FireDashingClone : BaseState
    {
        public static float baseDuration = 3f;

        public static GameObject projectilePrefab;

        public static int baseProjectileCount = 1;

        public static float projectilesPerPlayer = 0.5f;

        public static int clonesCount = 2;

        public static float dashDamageCoefficient = 2f;

        public static float delayBetweenProjectiles = 0.5f;

        private float projectileTimer;

        private int projectileCount;

        private int firedCount;

        private List<CharacterBody> bodies;

        public override void OnEnter()
        {
            base.OnEnter();
            bodies = Utils.GetActiveAndAlivePlayerBodies();
            if (bodies.Count == 0)
            {
                outer.SetNextStateToMain();
                return;
            }

            projectileCount = baseProjectileCount + (int)Math.Round(projectilesPerPlayer * (bodies.Count - 1), MidpointRounding.ToEven);
            projectileTimer = delayBetweenProjectiles;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (firedCount < projectileCount)
                {
                    if (projectileTimer <= 0f)
                    {
                        var target = bodies[UnityEngine.Random.Range(0, bodies.Count)];
                        FireProjectileAuthority(target.transform);
                        firedCount++;
                        projectileTimer += delayBetweenProjectiles;
                    }
                    projectileTimer -= GetDeltaTime();
                }
            }

            if (firedCount >= projectileCount && fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireProjectileAuthority(Transform target)
        {
            if (!isAuthority)
            {
                return;
            }

            var info = new FireProjectileInfo()
            {
                comboNumber = (byte)(clonesCount - 1),
                crit = RollCrit(),
                damage = damageStat * dashDamageCoefficient,
                damageTypeOverride = DamageSource.Secondary,
                force = 1000f,
                owner = gameObject,
                position = transform.position,
                procChainMask = new ProcChainMask(),
                projectilePrefab = projectilePrefab,
                rotation = Util.QuaternionSafeLookRotation((target.position - transform.position).normalized),
                fuseOverride = 1f
            };

            ProjectileManager.instance.FireProjectile(info);
        }

    }
}
