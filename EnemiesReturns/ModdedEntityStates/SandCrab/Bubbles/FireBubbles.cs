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

        public static float projectileSpread = 135f;

        public static int timesToFire = 4;

        public static int projectilesCount = 1;

        public static float damageCoefficient = 2f;

        public static float force = 0f;

        public static float minSpread = 0f;

        public static float maxSpread = 10f;

        public static float bonusSpread = 3f;

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
            startingDirection = Quaternion.AngleAxis(-projectileSpread * 0.5f, aimRay.direction) * Vector3.up;
            rotation = Quaternion.AngleAxis(angle, new Vector3(aimRay.direction.x, 0f, aimRay.direction.z));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer -= Time.fixedDeltaTime;
            if (timer < 0f)
            {
                PlayAnimation("Gesture, Override, Mask", "FireBubbles", "FireBubbles.playbackRate", singleDuration);
                if (isAuthority)
                {
                    //Ray ray = GetAimRay();
                    for (int i = 0; i < projectilesCount; i++)
                    {
                        //float bonusPitch = UnityEngine.Random.Range(-bonusSpread, bonusSpread);
                        //float bonusYaw = UnityEngine.Random.Range(-bonusSpread, bonusSpread);
                        FireSingleBubble(startingDirection, 0f, 0f);
                    }
                }
                timesFired++;
                timer += singleDuration;
                startingDirection = rotation * startingDirection;
            }

            if (timesFired >= timesToFire && isAuthority) 
            {
                outer.SetNextState(new EndBubbles());
            }
        }

        private void FireSingleBubble(Vector3 direction, float bonusPitch, float bonusYaw)
        {
            //Vector3 forward = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, bonusYaw, bonusPitch);
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
