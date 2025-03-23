using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash
{

    public class LeapDash : BaseLeapDash
    {
        public override float damageCoefficient => 2f;

        public override float liftOffTimer => 1.7f / 1f; // 1 is animation speed

        public override float force => 0f;

        public override float procCoefficient => 1f;

        public override float blastAttackRadius => 10f;

        public override float upwardVelocity => 30f;

        public override float forwardVelocity => 70f;

        public override float minimumY => 0.05f;

        public override float aimVelocity => 0f;

        public override float airControl => 1f;

        public override float additionalGravity => 0f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SwordFlipBegin";

        public static GameObject projectilePrefab;

        public static float projectileSpawnTime = 0.2f;

        public static float projectileDamage = 2f;

        private float timer;

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new LeapDashExit());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += GetDeltaTime();
            if (isAuthority && timer >= projectileSpawnTime)
            {
                Vector3 position;
                if (characterMotor.Motor.GroundingStatus.IsStableOnGround)
                {
                    position = characterBody.footPosition;
                } else if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 10000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    position = hitInfo.point;
                } else
                {
                    position = transform.position;
                }

                var projectileInfo = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    owner = base.gameObject,
                    position = position,
                    projectilePrefab = projectilePrefab,
                    rotation = Quaternion.identity,
                    damage = damageStat * projectileDamage
                };

                ProjectileManager.instance.FireProjectile(projectileInfo);

                timer -= projectileSpawnTime;
            }
        }
    }
}
