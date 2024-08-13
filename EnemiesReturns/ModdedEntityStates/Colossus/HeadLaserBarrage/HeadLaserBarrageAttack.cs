using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;
using UnityEngine.UIElements;
using RoR2.Projectile;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaserBarrage
{
    public class HeadLaserBarrageAttack: BaseState
    {
        public static float baseDuration = EnemiesReturnsConfiguration.Colossus.LaserBarrageDuration.Value;

        public static GameObject projectilePrefab;

        public static float projectileSpeed = EnemiesReturnsConfiguration.Colossus.LaserBarrageProjectileSpeed.Value;

        public static float baseFireFrequency = EnemiesReturnsConfiguration.Colossus.LaserBarrageFrequency.Value;

        public static int projectilesPerShot = EnemiesReturnsConfiguration.Colossus.LaserBarrageProjectileCount.Value;

        public static float damageCoefficient = EnemiesReturnsConfiguration.Colossus.LaserBarrageDamage.Value;

        public static float forceMagnitude = EnemiesReturnsConfiguration.Colossus.LaserBarrageForce.Value;

        public static float pitch = EnemiesReturnsConfiguration.Colossus.LaserBarrageHeadPitch.Value;

        public static float spread = EnemiesReturnsConfiguration.Colossus.LaserBarrageSpread.Value;

        private float duration;

        private float fireFrequency;

        private Animator modelAnimator;

        private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

        private static readonly int aimPitchCycleHash = Animator.StringToHash("aimPitchCycle");

        private float stopwatch;

        private Transform initialPoint;

        private Transform bulletSpawnHelperPoint;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireFrequency = baseFireFrequency / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            initialPoint = FindModelChild("LaserInitialPoint");
            bulletSpawnHelperPoint = FindModelChild("BulletAttackHelper");
            PlayAnimation("Body", "LaserBeamLoop");
            Fire();
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                // math is fun
                modelAnimator.SetFloat(aimYawCycleHash, Mathf.Clamp(age / baseDuration, 0f, 0.99f));
                modelAnimator.SetFloat(aimPitchCycleHash, pitch);
                //modelAnimator.SetFloat(aimPitchCycleHash, Mathf.Clamp(pitchStart + pitchStep * Mathf.Min(age / (duration / totalTurnCount), totalTurnCount - 1), 0f, 0.99f));
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if(stopwatch > fireFrequency)
            {
                Fire();
                stopwatch -= fireFrequency;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new HeadLaserBarrageEnd());
            }
        }

        private void Fire()
        {
            if (isAuthority)
            {
                for (int i = 0; i < projectilesPerShot; i++)
                {
                    bulletSpawnHelperPoint.localPosition = new Vector3(UnityEngine.Random.Range(-spread, spread), UnityEngine.Random.Range(-spread, spread), 0.2f);
                    var rotation = Quaternion.LookRotation(bulletSpawnHelperPoint.position - initialPoint.position, Vector3.up);
                    ProjectileManager.instance.FireProjectile(projectilePrefab, initialPoint.position, rotation, base.gameObject, damageStat * damageCoefficient, forceMagnitude, RollCrit(), RoR2.DamageColorIndex.Default, null, projectileSpeed);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
