using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Secondary
{
    [RegisterEntityState]
    public class DashAttack : P1.Secondary.DashAttack
    {
        public static GameObject projectileClone;

        public static float dashDamageCoefficient = 3f;

        public static int minClones = 1;

        public static int maxClones = 3;

        public override void AuthorityOnFinish()
        {
            FireProjectileAuthority();
            base.AuthorityOnFinish();
        }

        private void FireProjectileAuthority()
        {
            if (!isAuthority)
            {
                return;
            }

            int clonesCount = (int)Mathf.Min(maxClones, Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.25f, healthComponent.fullHealth, (float)maxClones, (float)minClones));

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
                projectilePrefab = projectileClone,
                rotation = Util.QuaternionSafeLookRotation(inputBank.aimDirection),
                fuseOverride = 1f
            };

            ProjectileManager.instance.FireProjectile(info);
        }
    }
}

