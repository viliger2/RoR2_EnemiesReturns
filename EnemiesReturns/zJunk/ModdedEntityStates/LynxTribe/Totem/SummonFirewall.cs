using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Totem
{
    public class SummonFirewall : BaseState
    {
        public static float baseDuration = 4f;

        public static float damageCoefficient = 3f;

        public static float baseAttackDelay = 1f;

        public static GameObject projectilePrefab;

        private float attackDelay;

        private float duration;

        private float timer;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            attackDelay = baseAttackDelay / attackSpeedStat;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += GetDeltaTime();
            if (timer > attackDelay)
            {
                if (isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefab, transform.position, Quaternion.identity, gameObject, damageStat * damageCoefficient, 0f, RollCrit(), damageType: new DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Primary));
                }
                timer -= attackDelay;
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
