using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Linq;
using UnityEngine;
using static EntityStates.TitanMonster.FireFist;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    public class FireHellzoneFire : BaseState
    {
        public static float baseDuration = 2f;

        public static float baseChargeTime = 0.5f;

        public static float baseSpawnDoTZoneTime = 1f;

        public static string targetMuzzle = "MuzzleMouth";

        public static GameObject projectilePrefab;

        public static float projectileSpeed = EnemiesReturnsConfiguration.Ifrit.HellzoneFireballProjectileSpeed.Value;

        public static float damageCoefficient = EnemiesReturnsConfiguration.Ifrit.HellzoneFireballDamage.Value;

        public static float dotZoneDamageCoefficient = EnemiesReturnsConfiguration.Ifrit.HellzoneDoTZoneDamage.Value;

        public static float force = EnemiesReturnsConfiguration.Ifrit.HellzoneFireballForce.Value;

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
                if(predictor!= null)
                {
                    predictor.Update();
                    predictor.GetPredictedTargetPosition(baseDuration - baseChargeTime, out predictedTargetPosition);
                }
            }

            if (!hasFired && isAuthority && fixedAge >= chargeTime)
            {
                FireProjectile();
                hasFired = true;
            }

            if(!hasSpawnedDoTZone && isAuthority && fixedAge >= spawnDoTZoneTime)
            {
                FireDoTZone();
                hasSpawnedDoTZone = true;
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
        }

        private void FireDoTZone()
        {
            var position = predictedTargetPosition;
            if (position == Vector3.zero)
            {
                if (Physics.Raycast(muzzleMouth.position, muzzleMouth.forward, out var result, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                {
                    position = result.point;
                }
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
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
