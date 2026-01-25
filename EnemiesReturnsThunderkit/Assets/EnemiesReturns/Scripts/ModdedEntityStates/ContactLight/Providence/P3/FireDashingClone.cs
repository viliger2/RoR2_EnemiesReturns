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
        public static GameObject projectilePrefab;

        public static int baseProjectileCount = 1;

        public static float projectilesPerPlayer = 0.5f;

        public static int clonesCount = 2;

        public static float dashDamageCoefficient;

        public static float delayBetweenProjectiles = 0.5f;

        private float projectileTimer;

        private int projectileCount;

        private int firedCount;

        private List<CharacterBody> bodies;

        public override void OnEnter()
        {
            base.OnEnter();
            //bodies = Utils.GetActiveAndAlivePlayerBodies();
            if (bodies.Count == 0)
            {
                outer.SetNextStateToMain();
                return;
            }

            projectileCount = baseProjectileCount + (int)Math.Round(projectilesPerPlayer * (bodies.Count - 1), MidpointRounding.ToEven);
        }

        private void FireProjectileAuthority()
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
                rotation = Util.QuaternionSafeLookRotation(inputBank.aimDirection),
                fuseOverride = 1f
            };

            ProjectileManager.instance.FireProjectile(info);
        }

    }
}
