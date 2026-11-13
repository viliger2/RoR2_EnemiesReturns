using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Special;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Special
{
    [RegisterEntityState]
    public class RotatingLaserBegin : BaseState
    {
        public static float baseDuration = 2.7f;

        public static GameObject laserProjectile;

        public static float damageCoefficient => ProvidencePhase3.P3LaserDamage.Value;

        private Animator animator;

        private bool spawnedLaser;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = GetModelAnimator();
            PlayCrossfade("Gesture, Override", "BeamInit", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(animator && animator.GetFloat("Beam.attack") > 0.9f && !spawnedLaser && isAuthority)
            {
                FireProjectileAuthority();
                spawnedLaser = true;
            }

            if (fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new RotatingLaserHold());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public void FireProjectileAuthority()
        {
            Vector3 newPosition;
            if (characterMotor.Motor.GroundingStatus.IsStableOnGround)
            {
                newPosition = characterBody.footPosition;
            }
            else if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 10000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                newPosition = hitInfo.point;
            }
            else
            {
                newPosition = transform.position;
            }

            var projectileInfo = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damageStat * damageCoefficient,
                damageTypeOverride = DamageTypeCombo.GenericSpecial,
                force = 0f,
                owner = gameObject,
                position = newPosition,
                projectilePrefab = laserProjectile,
                rotation = Quaternion.identity
            };

            ProjectileManager.instance.FireProjectile(projectileInfo);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
