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

        public static float baseFireCooldown => 0.5f;

        public static int numberOfBarrages => 4;

        public static float baseSingleShotDuration => 0.6667f;

        public static float spreadBloom => 0.2f;

        public static float projectileDamage => 1f;

        public static float speedOverride => 75f;

        public static float correctionAngle = 2.5f;

        public static GameObject primaryEffect;

        private float fireCooldown;

        private int barragesFired;

        private float timer;

        private float singleShotDuration;

        private List<GameObject> primaryEffects = new List<GameObject>();

        public override void OnEnter()
        {
            base.OnEnter();
            fireCooldown = baseFireCooldown / attackSpeedStat;
            singleShotDuration = baseSingleShotDuration / attackSpeedStat;

            FireBarrage();

            if (primaryEffect)
            {
                SpawnEffect("CannonR");
                SpawnEffect("CannonL");
            }

            void SpawnEffect(string childName)
            {
                var parent = FindModelChild(childName);
                if (parent)
                {
                    var newObject = UnityEngine.Object.Instantiate(primaryEffect, parent.position, parent.rotation);
                    newObject.transform.parent = parent;
                    primaryEffects.Add(newObject);
                }
            }
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
            for (int i = 0; i < primaryEffects.Count; i++)
            {
                if ((bool)primaryEffects[i])
                {
                    Destroy(primaryEffects[i]);
                }
            }
        }

        private void FireBarrage()
        {
            PlayAnimation("Gesture, Additive", "FireBarrage", "dualShot.duration", singleShotDuration);
            if (isAuthority)
            {
                FireSingleProjectile(correctionAngle, "CannonL");
                FireSingleProjectile(-correctionAngle, "CannonR");

                characterBody.AddSpreadBloom(spreadBloom);
            }
            barragesFired++;

            void FireSingleProjectile(float correctionAngle, string originChildName)
            {
                var aimRay = GetAimRay();
                aimRay.direction = Quaternion.AngleAxis(correctionAngle, Vector3.up) * aimRay.direction;
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, characterBody.spreadBloomAngle, 1f, 1f);

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
                    damage = damageStat * projectileDamage,
                    owner = gameObject,
                    position = origin,
                    projectilePrefab = projectilePrefab,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                    speedOverride = speedOverride
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
