using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
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
            if(timer > attackDelay)
            {
                if (isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(projectilePrefab, transform.position, Quaternion.identity, base.gameObject, damageStat * damageCoefficient, 0f, RollCrit(), damageType: new RoR2.DamageTypeCombo(DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Primary));
                }
                timer -= attackDelay;
            }

            if(fixedAge > duration && isAuthority)
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
