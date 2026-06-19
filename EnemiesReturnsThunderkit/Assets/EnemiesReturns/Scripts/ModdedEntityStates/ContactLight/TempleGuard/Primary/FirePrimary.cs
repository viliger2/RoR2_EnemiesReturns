using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard.Primary
{
    [RegisterEntityState]
    public class FirePrimary : BaseState
    {
        public static GameObject projectilePrefab;

        public static float baseFireCooldown => 1f;

        public static int numberOfBarrages => 4;

        public static float baseSingleShotDuration => 0.6667f;

        public static float spreadBloom => 0.2f;

        public static float projectileDamage => 1f;

        private float fireCooldown;

        private int barragesFired;

        private float timer;

        private float singleShotDuration;

        public override void OnEnter()
        {
            base.OnEnter();
            fireCooldown = baseFireCooldown / attackSpeedStat;
            singleShotDuration = baseSingleShotDuration / attackSpeedStat;

            FireBarrage();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += Time.fixedDeltaTime;
            if (timer > fireCooldown)
            {
                FireBarrage();

                timer -= fireCooldown;
            }

            if (barragesFired >= numberOfBarrages && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
        }

        private void FireBarrage()
        {
            PlayAnimation("Gesture", "FireBarrage", "dualShot.duration", singleShotDuration);
            if (isAuthority)
            {
                var aimRay = GetAimRay();
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, characterBody.spreadBloomAngle, 1f, 1f);

                FireSingleProjectile(aimRay, "CannonL");
                FireSingleProjectile(aimRay, "CannonR");

                characterBody.AddSpreadBloom(spreadBloom);
            }
            barragesFired++;

            void FireSingleProjectile(Ray aimRay, string originChildName)
            {
                Vector3 origin = Vector3.zero;
                var cannon = FindModelChild(originChildName);
                if (cannon)
                {
                    origin = cannon.position;
                }
                if (origin == Vector3.zero)
                {
                    origin = aimRay.origin;
                }

                FireProjectileInfo info = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    damage = projectileDamage,
                    owner = gameObject,
                    position = origin,
                    projectilePrefab = projectilePrefab,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
