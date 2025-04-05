using EntityStates;
using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign
{
    public abstract class BaseWeaponThrow : BaseState
    {
        public abstract GameObject projectilePrefab { get; }

        public abstract float baseDuration { get; }

        public abstract float damageCoefficient { get; }

        public abstract float force { get; }

        public abstract string layerName { get; }

        public abstract string animName { get; }

        public abstract string playbackRateParamName { get; }

        private float duration;

        private bool hasAttacked;

        private CharacterBody target;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            if (NetworkServer.active && isAuthority)
            {
                var bodies = Utils.GetActiveAndAlivePlayerBodies();
                foreach (var body in bodies)
                {
                    if (body && body.characterMotor && !body.characterMotor.isGrounded)
                    {
                        target = body;
                    }
                }
                if (target)
                {
                    foreach (var ai in characterBody.master.aiComponents)
                    {
                        ai.currentEnemy.gameObject = target.gameObject;
                        ai.enemyAttention = duration + 1f;
                    }
                    inputBank.aimDirection = target.gameObject.transform.position - GetAimRay().origin;
                }
                if (characterDirection)
                {
                    characterDirection.moveVector = inputBank.aimDirection;
                }
            }
            PlayAnimation(layerName, animName, playbackRateParamName, duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            characterDirection.forward = GetAimRay().direction.normalized;
            if (modelAnimator.GetFloat("WeaponThrow.throw") > 0.9f && isAuthority && !hasAttacked)
            {
                FireProjectile();
                hasAttacked = true;
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireProjectile()
        {
            if (isAuthority)
            {
                var aimRay = GetAimRay();

                var info = new FireProjectileInfo
                {
                    crit = RollCrit(),
                    damage = damageStat * damageCoefficient,
                    force = force,
                    owner = base.gameObject,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    projectilePrefab = projectilePrefab,
                    damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary),
                };
                if (target)
                {
                    info.target = target.gameObject;
                }

                ProjectileManager.instance.FireProjectile(info);
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
