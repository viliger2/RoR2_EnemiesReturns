using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.SandCrab.Bubbles
{
    [RegisterEntityState]
    public class FireBubbles : BaseState
    {
        public static float baseSingleDuration = 0.45f;

        public static float projectileSpread => Configuration.SandCrab.BubbleProjectileSpread.Value;

        public static int timesToFire => Configuration.SandCrab.BubbleShotCount.Value;

        public static int projectilesCount = 1;

        public static float damageCoefficient => Configuration.SandCrab.BubbleDamage.Value;

        public static float force => Configuration.SandCrab.BubbleForce.Value;

        public static float degrees = 20f;

        public static GameObject projectilePrefab;

        private Transform projectileOrigin;

        private float singleDuration;

        private float timer;

        private int timesFired;

        private Vector3 startingDirection;

        private Quaternion rotation;

        public override void OnEnter()
        {
            base.OnEnter();
            singleDuration = baseSingleDuration / attackSpeedStat;
            timer = 0f;

            projectileOrigin = FindModelChild("BubblesFireOrigin");
            if (!projectileOrigin)
            {
                projectileOrigin = transform;
            }
            characterBody.SetAimTimer(singleDuration * timesToFire);

            var angle = projectileSpread / (timesToFire - 1);
            var aimRay = GetAimRay();
            var angleFromForward = Vector3.SignedAngle(Vector3.forward, new Vector3(aimRay.direction.x, 0, aimRay.direction.z), Vector3.up); // we find how far are we from forward ignoring y axis, so it doesn't affect the angle from forward
            var newRight = Quaternion.AngleAxis(angleFromForward, Vector3.up) * Vector3.right; // using the angle we find our new right to our aim direction
            var newVector = (Quaternion.AngleAxis(-degrees, newRight) * aimRay.direction).normalized; // here we angle aim direction 30 degrees towards the sky
            var rotationVector = Vector3.Cross(newRight, newVector); // and finally we find the vector that we use as a vector to rotate bubbles around

            startingDirection = Quaternion.AngleAxis(projectileSpread * 0.5f, rotationVector) * newVector;
            rotation = Quaternion.AngleAxis(-angle, rotationVector);
            Util.PlaySound("ER_SandCrab_FireBubbles_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.moveDirection = Vector3.zero;
            inputBank.moveVector = Vector3.zero;
            timer -= Time.fixedDeltaTime;
            if (timer < 0f)
            {
                PlayAnimation("Gesture, Override, Mask", "FireBubbles", "FireBubbles.playbackRate", singleDuration);
                if (isAuthority)
                {
                    for (int i = 0; i < projectilesCount; i++)
                    {
                        FireSingleBubble(startingDirection);
                    }
                }
                Util.PlaySound("ER_SandCrab_Bubbles_Spawn_Play", base.gameObject);
                timesFired++;
                timer += singleDuration;
                startingDirection = rotation * startingDirection;
            }

            if (timesFired >= timesToFire && isAuthority) 
            {
                outer.SetNextState(new EndBubbles());
            }
        }

        private void FireSingleBubble(Vector3 direction)
        {
            var fireProjectileInfo = new FireProjectileInfo()
            {
                crit = RollCrit(),
                damage = damageStat * damageCoefficient,
                damageTypeOverride = new DamageTypeCombo(DamageType.SlowOnHit, DamageTypeExtended.Generic, DamageSource.Secondary),
                force = force,
                position = projectileOrigin.position,
                rotation = Util.QuaternionSafeLookRotation(direction),
                projectilePrefab = projectilePrefab,
                owner = gameObject,
            };

            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override, Mask", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
