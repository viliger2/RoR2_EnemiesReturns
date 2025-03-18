using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    public class SpearThrow : BaseState
    {
        public static GameObject projectilePrefab;

        public static float baseDuration = 5.6f;

        public static float baseAttack = 3f;

        public static float damageCoefficient = 3f;

        public static float force = 0f;

        private float duration;

        private float attack;

        private bool hasAttacked;

        private CharacterBody target;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            attack = baseAttack / attackSpeedStat;
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
                    foreach(var ai in characterBody.master.aiComponents)
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
            PlayAnimation("Gesture, Override", "ThrowSword", "Throw.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > attack && isAuthority && !hasAttacked)
            {
                FireProjectile();
                hasAttacked = true;
            }

            if(fixedAge > duration && isAuthority)
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
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
