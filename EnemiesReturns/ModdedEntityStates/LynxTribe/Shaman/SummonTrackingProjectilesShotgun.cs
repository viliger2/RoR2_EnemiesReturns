using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class SummonTrackingProjectilesShotgun : GenericCharacterMain
    {
        public static GameObject trackingProjectilePrefab;

        public static float baseDuration = 3.3f;

        public static int projectileCount => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesCount.Value;

        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesDamage.Value;

        public static float baseInitialDelay => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesDelay.Value;

        private float duration;

        private float initialDelay;

        private bool isShot;

        private Transform spawnPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            initialDelay = baseInitialDelay / attackSpeedStat;
            PlayCrossfade("Gesture", "SummonStorm", "SummonStorm.playbackRate", duration, 0.1f);
            spawnPoint = FindModelChild("StaffUpperPoint");
            if (!spawnPoint)
            {
                spawnPoint = transform;
            }
            isShot = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge < initialDelay)
            {
                return;
            }
            if (isAuthority && !isShot)
            {
                for (int i = 0; i < projectileCount; i++)
                {
                    var spawnDirection = new Vector3(0f, (360f / projectileCount) * i, 0);
                    ProjectileManager.instance.FireProjectile(trackingProjectilePrefab, spawnPoint.position, Quaternion.Euler(spawnDirection), gameObject, damageStat * damageCoefficient, 0f, RollCrit(), RoR2.DamageColorIndex.Poison);
                }
                isShot = true;
            }
            if(fixedAge >= duration && isAuthority)
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
