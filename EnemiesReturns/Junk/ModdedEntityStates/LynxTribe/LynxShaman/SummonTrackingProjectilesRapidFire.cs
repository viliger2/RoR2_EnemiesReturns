using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.LynxShaman
{
    public class SummonTrackingProjectilesRapidFire : BaseState
    {
        public static GameObject trackingProjectilePrefab;

        public static float baseDuration = 3.3f;

        public static int projectileCount => Configuration.LynxTribe.LynxShaman.SummonProjectilesCount.Value;

        public static float baseBetweenDelay = 0.5f;

        public static float damageCoefficient => Configuration.LynxTribe.LynxShaman.SummonProjectilesDamage.Value;

        public static float baseInitialDelay = 1f;

        public static Vector3 spawnDirection = new Vector3(-90f, 0f, 0f);

        private float duration;

        private float initialDelay;

        private float betweenDelay;

        private float delayTimer;

        private int projectilesShot;

        private Transform spawnPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            betweenDelay = baseBetweenDelay / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            initialDelay = baseInitialDelay / attackSpeedStat;
            PlayCrossfade("Gesture", "SummonStorm", "SummonStorm.playbackRate", duration, 0.1f);
            spawnPoint = FindModelChild("StaffUpperPoint");
            if (!spawnPoint)
            {
                spawnPoint = transform;
            }
            projectilesShot = 0;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            delayTimer += GetDeltaTime();
            if (fixedAge < initialDelay)
            {
                return;
            }
            if (delayTimer > initialDelay && isAuthority && projectilesShot < projectileCount)
            {
                ProjectileManager.instance.FireProjectile(trackingProjectilePrefab, spawnPoint.position, Quaternion.Euler(spawnDirection), gameObject, damageStat * damageCoefficient, 0f, RollCrit(), RoR2.DamageColorIndex.Poison);
                delayTimer -= betweenDelay;
                projectilesShot++;
            }
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
