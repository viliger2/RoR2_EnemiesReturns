﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    [RegisterEntityState]
    public class FireHellzoneFire : BaseState
    {
        public static float baseDuration = 2f;

        public static float baseChargeTime = 0.5f;

        public static float baseSpawnDoTZoneTime = 1f;

        public static string targetMuzzle = "MuzzleMouth";

        public static GameObject projectilePrefab;

        public static float projectileSpeed = EnemiesReturns.Configuration.Ifrit.HellzoneFireballProjectileSpeed.Value;

        public static float damageCoefficient = EnemiesReturns.Configuration.Ifrit.HellzoneFireballDamage.Value;

        public static float dotZoneDamageCoefficient = EnemiesReturns.Configuration.Ifrit.HellzoneDoTZoneDamage.Value;

        public static float force = EnemiesReturns.Configuration.Ifrit.HellzoneFireballForce.Value;

        public static GameObject dotZoneProjectile;

        public Predictor predictor;

        private float duration;

        private float chargeTime;

        private bool hasFired;

        private bool hasSpawnedDoTZone;

        private float spawnDoTZoneTime => baseSpawnDoTZoneTime - (baseChargeTime - chargeTime);

        private Vector3 predictedTargetPosition;

        private Transform muzzleMouth;

        private Transform fireballAimHelper;

        private Vector3 defaultSpawnPosition;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            chargeTime = baseChargeTime / attackSpeedStat;
            muzzleMouth = FindModelChild("MuzzleMouth");
            fireballAimHelper = FindModelChild("FireballAimHelper");
            PlayCrossfade("Gesture,Override", "FireballFire", "FireFireball.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge <= spawnDoTZoneTime)
            {
                if (predictor != null)
                {
                    predictor.Update();
                    predictor.GetPredictedTargetPosition(baseDuration - baseChargeTime, out predictedTargetPosition);
                }
            }

            if (isAuthority)
            {
                if (!hasFired && fixedAge >= chargeTime)
                {
                    FireProjectile();
                    hasFired = true;
                }

                if (!hasSpawnedDoTZone && fixedAge >= spawnDoTZoneTime)
                {
                    FireDoTZone();
                    hasSpawnedDoTZone = true;
                }
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new FireHellzoneEnd());
            }
        }

        private void FireProjectile()
        {
            var rotation = Quaternion.LookRotation(fireballAimHelper.position - muzzleMouth.position, Vector3.up);
            ProjectileManager.instance.FireProjectile(projectilePrefab, muzzleMouth.position, rotation, gameObject, damageStat * damageCoefficient, force, RollCrit(), RoR2.DamageColorIndex.Default, null, projectileSpeed);
            if (Physics.Raycast(muzzleMouth.position, muzzleMouth.TransformDirection(Vector3.forward), out var result, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
            {
                defaultSpawnPosition = result.point;
            }
        }

        private void FireDoTZone()
        {
            var position = predictedTargetPosition;
            if (position == Vector3.zero)
            {
                position = defaultSpawnPosition;
            }

            var projectileInfo = new FireProjectileInfo();
            projectileInfo.projectilePrefab = dotZoneProjectile;
            projectileInfo.position = position;
            projectileInfo.rotation = Quaternion.identity;
            projectileInfo.owner = base.gameObject;
            projectileInfo.damage = damageStat * dotZoneDamageCoefficient;
            projectileInfo.force = 0f;
            projectileInfo.crit = RollCrit();
            projectileInfo.fuseOverride = baseDuration - baseSpawnDoTZoneTime;
            ProjectileManager.instance.FireProjectile(projectileInfo);
        }

        public override void OnExit()
        {
            if (!hasSpawnedDoTZone && isAuthority)
            {
                FireDoTZone();
            }
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
