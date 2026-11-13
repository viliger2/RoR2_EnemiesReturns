using EnemiesReturns.Configuration.ContactLight.Providence;
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

        public static float dashDamageCoefficient => ProvidencePhase2.P2SecondaryCloneDamage.Value;

        public static int minClones => ProvidencePhase2.P2SecondaryCloneMinSpawns.Value;

        public static int maxClones => ProvidencePhase2.P2SecondaryCloneMaxSpawns.Value;

        public static float minClonesHealth => ProvidencePhase2.P2SecondaryCloneMinHealthThreshold.Value;

        public static float maxClonesHealth => ProvidencePhase2.P2SecondaryCloneMaxHealthThreshold.Value;

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

            int clonesCount = (int)Mathf.Min(maxClones, Util.Remap(healthComponent.health, healthComponent.fullHealth * maxClonesHealth, healthComponent.fullHealth * minClonesHealth, (float)maxClones, (float)minClones));

            if(clonesCount == 0)
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
                projectilePrefab = projectileClone,
                rotation = Util.QuaternionSafeLookRotation(inputBank.aimDirection),
                fuseOverride = 1f
            };

            ProjectileManager.instance.FireProjectile(info);
        }
    }
}

