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
    public class SpawnRotatingLaser : BaseState
    {
        public static float baseDuration = 3f;

        public static float damageCoefficient = 2f;

        public static GameObject projectilePrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "SwordLaserBegin", "SwordBeam.playbackRate", baseDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > baseDuration && isAuthority)
            {
                FireProjectileAuthority();
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
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
                projectilePrefab = projectilePrefab,
                rotation = Quaternion.identity,
            };

            ProjectileManager.instance.FireProjectile(projectileInfo);
        }
    }
}
