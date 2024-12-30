using EntityStates;
using RoR2;
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

        public static float baseEffectSpawnTime = 0.5f;

        public static GameObject summonEffect;

        public static int projectileCount => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesCount.Value;

        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesDamage.Value;

        public static float baseInitialDelay = 2.8f;

        //public static float initialSummonEffectScale = 1.5f;

        public static AnimationCurve effectScaling;

        private float duration;

        private float initialDelay;

        private float effectSpawnTime;

        private bool isEffectSpawned;

        private bool isShot;

        private Transform spawnPoint;

        private ChildLocator childLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            initialDelay = baseInitialDelay / attackSpeedStat;
            effectSpawnTime = baseEffectSpawnTime / attackSpeedStat;
            PlayCrossfade("Gesture", "SummonStorm", "SummonStorm.playbackRate", duration, 0.1f);
            childLocator = GetModelChildLocator();
            spawnPoint = childLocator.FindChild("StaffUpperPoint");
            Util.PlayAttackSpeedSound(EnemiesReturns.Configuration.General.LynxVoices.Value ? "ER_Shaman_SummonProjectiles_Play" : "ER_Shaman_SummonProjectiles_No_Voice_Play", base.gameObject, attackSpeedStat);
            if (!spawnPoint)
            {
                spawnPoint = transform;
            }
            isShot = false;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > effectSpawnTime && !isEffectSpawned)
            {
                var effectData = new EffectData()
                {
                    rootObject = this.gameObject,
                    modelChildIndex = (short)childLocator.FindChildIndex(spawnPoint),
                    origin = spawnPoint.position
                };
                EffectManager.SpawnEffect(summonEffect, effectData, false);
                // spawn effect
                isEffectSpawned = true;
            }
            if (fixedAge > initialDelay && !isShot)
            {
                if (isAuthority)
                {
                    for (int i = 0; i < projectileCount; i++)
                    {
                        var spawnDirection = new Vector3(0f, (360f / projectileCount) * i, 0);
                        ProjectileManager.instance.FireProjectile(trackingProjectilePrefab, spawnPoint.position, Quaternion.Euler(spawnDirection), gameObject, damageStat * damageCoefficient, 0f, RollCrit(), RoR2.DamageColorIndex.Poison);
                    }
                }
                //Util.PlaySound("ER_Shaman_SummonProjectiles_Stop", base.gameObject);
                Util.PlaySound("ER_Shaman_FireProjectiles_Play", base.gameObject);
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
