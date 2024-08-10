using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaser
{
    public class HeadLaserAttack : BaseState
    {
        public static float baseDuration = EnemiesReturnsConfiguration.Colossus.HeadLaserDuration.Value;

        public static float baseFireFrequency = EnemiesReturnsConfiguration.Colossus.HeadLaserFireFrequency.Value;

        public static float laserDamage = EnemiesReturnsConfiguration.Colossus.HeadLaserDamage.Value;

        public static float laserForce = EnemiesReturnsConfiguration.Colossus.HeadLaserForce.Value;

        public static float laserRadius = EnemiesReturnsConfiguration.Colossus.HeadLaserRadius.Value;

        public static GameObject beamPrefab;

        public static int totalTurnCount = EnemiesReturnsConfiguration.Colossus.HeadLaserTurnCount.Value;

        public static float pitchStart = EnemiesReturnsConfiguration.Colossus.HeadLaserPitchStart.Value;

        public static float pitchStep = EnemiesReturnsConfiguration.Colossus.HeadLaserPitchStep.Value;

        private static float laserMaxDistance = 2000f;

        private float duration;

        private Animator modelAnimator;

        private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

        private static readonly int aimPitchCycleHas = Animator.StringToHash("aimPitchCycle");

        private GameObject beamInstance;

        private Transform effectPoint;

        private Transform bulletAttackPoint;

        private float fireFrequency;

        private float stopwatch;

        private float angleAttackSpeedMult; // angle adjustment to attackspeed, so we turn faster 

        private float anglePerSecond; // how much we turn per second, used for sin() calculation

        private BulletAttack bulletAttack;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            angleAttackSpeedMult = baseDuration / duration;
            anglePerSecond = (totalTurnCount * 90) / baseDuration; // 90 is for sin()
            fireFrequency = baseFireFrequency / attackSpeedStat;

            effectPoint = FindModelChild("LaserEffectPoint");
            bulletAttackPoint = FindModelChild("LaserInitialPoint");

            bulletAttack = CreateBulletAttack();

            modelAnimator = GetModelAnimator();

            beamInstance = UnityEngine.Object.Instantiate(beamPrefab);
            beamInstance.transform.SetParent(effectPoint, worldPositionStays: true);

            PlayAnimation("Body", "LaserBeamLoop");
            UpdateBeamTransforms();
            RoR2Application.onLateUpdate += UpdateBeamTransformsInLateUpdate;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (modelAnimator)
            {
                // math is fun
                modelAnimator.SetFloat(aimYawCycleHash, Mathf.Clamp(Mathf.Abs(Mathf.Sin(fixedAge * anglePerSecond * angleAttackSpeedMult * Mathf.Deg2Rad)), 0f, 0.99f));
                modelAnimator.SetFloat(aimPitchCycleHas, Mathf.Clamp(pitchStart + pitchStep * Mathf.Min((fixedAge / (duration / totalTurnCount)), totalTurnCount - 1), 0f, 0.99f));
            }
            if(isAuthority && stopwatch >= fireFrequency)
            {
                bulletAttack.origin = bulletAttackPoint.position;
                bulletAttack.aimVector = new Ray(bulletAttackPoint.position, bulletAttackPoint.forward).direction;
                bulletAttack.Fire();
                stopwatch -= fireFrequency;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new HeadLaserEnd());
            }
        }

        private BulletAttack CreateBulletAttack()
        {
            var bulletAttack = new BulletAttack();
            bulletAttack.owner = gameObject;
            bulletAttack.weapon = gameObject;
            bulletAttack.origin = bulletAttackPoint.position;
            bulletAttack.aimVector = new Ray(bulletAttackPoint.position, bulletAttackPoint.forward).direction;
            bulletAttack.minSpread = 0f;
            bulletAttack.maxSpread = 0f;
            bulletAttack.bulletCount = 1;
            bulletAttack.damage = laserDamage * damageStat;
            bulletAttack.force = laserForce;
            bulletAttack.tracerEffectPrefab = null;
            bulletAttack.muzzleName = "";
            bulletAttack.hitEffectPrefab = null; // TODO
            bulletAttack.isCrit = false;
            bulletAttack.radius = laserRadius;
            bulletAttack.smartCollision = false;
            bulletAttack.damageType = DamageType.Generic;
            bulletAttack.maxDistance = laserMaxDistance;
            bulletAttack.procChainMask = default(ProcChainMask);
            bulletAttack.damageColorIndex = DamageColorIndex.Default;
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            return bulletAttack;
        }

        private void UpdateBeamTransformsInLateUpdate()
        {
            try
            {
                UpdateBeamTransforms();
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        private void UpdateBeamTransforms()
        {
            Ray beamRay = GetHeadAimRay();
            beamInstance.transform.SetPositionAndRotation(beamRay.origin, Quaternion.LookRotation(beamRay.direction));
        }

        private Ray GetHeadAimRay()
        {
            return new Ray(effectPoint.position, effectPoint.forward);
        }

        public override void OnExit()
        {
            modelAnimator.SetFloat(aimYawCycleHash, 0.5f);
            modelAnimator.SetFloat(aimPitchCycleHas, 0.5f);
            base.OnExit();
            RoR2Application.onLateUpdate -= UpdateBeamTransformsInLateUpdate;
            UnityEngine.Object.Destroy(beamInstance);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
