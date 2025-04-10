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

        public override float force => 0f;

        public override float procCoefficient => 1f;

        public override float blastAttackRadius => 10f;

        public override float upwardVelocity => 30f;

        public override float forwardVelocity => 80f;

        public override float minimumY => 0.05f;

        public override float aimVelocity => 20f;

        public override float airControl => 10f;

        public override float additionalGravity => 0f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SwordFlipBegin";

        public static GameObject projectilePrefab;

        public static float distanceBetweenProjectiles = 13f;

        public static float projectileDamage = 2f;

        private Vector3 lastPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            lastPosition = transform.position;
        }

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new LeapDashExit());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!liftedOff)
            {
                return;
            }

            if (isAuthority)
            {
                Vector3 newPosition;
                if (characterMotor.Motor.GroundingStatus.IsStableOnGround)
                {
                    newPosition = characterBody.footPosition;
                } else if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 10000f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    newPosition = hitInfo.point;
                } else
                {
                    newPosition = transform.position;
                }

                if (Vector3.Distance(newPosition, lastPosition) > distanceBetweenProjectiles)
                {

                    var projectileInfo = new FireProjectileInfo()
                    {
                        crit = RollCrit(),
                        owner = base.gameObject,
                        position = newPosition,
                        projectilePrefab = projectilePrefab,
                        rotation = Quaternion.identity,
                        damage = damageStat * projectileDamage
                    };

                    ProjectileManager.instance.FireProjectile(projectileInfo);
                    lastPosition = newPosition;
                }
            }
        }
    }
}
