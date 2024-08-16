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
using Rewired.HID;
using EnemiesReturns.Enemies.Colossus;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaserBarrage
{
    public class HeadLaserBarrageAttack: BaseState
    {
        public static float baseDuration => EnemiesReturnsConfiguration.Colossus.LaserBarrageDuration.Value;

        public static float projectileSpeed => EnemiesReturnsConfiguration.Colossus.LaserBarrageProjectileSpeed.Value;

        public static float baseFireFrequency => EnemiesReturnsConfiguration.Colossus.LaserBarrageFrequency.Value;

        public static int projectilesPerShot => EnemiesReturnsConfiguration.Colossus.LaserBarrageProjectileCount.Value;

        public static float damageCoefficient => EnemiesReturnsConfiguration.Colossus.LaserBarrageDamage.Value;

        public static float forceMagnitude => EnemiesReturnsConfiguration.Colossus.LaserBarrageForce.Value;

        public static float pitch => EnemiesReturnsConfiguration.Colossus.LaserBarrageHeadPitch.Value;

        public static float spread => EnemiesReturnsConfiguration.Colossus.LaserBarrageSpread.Value;

        public static float desiredEmission = ColossusFactory.MAX_BARRAGE_EMISSION; // max total emmision, we jump from 3.5 to 7 with intencityGraph

        public static float desiredLightRange = ColossusFactory.MAX_EYE_LIGHT_RANGE;

        public static AnimationCurve intencityGraph;

        public static GameObject projectilePrefab;

        private float additionalEmmision => desiredEmission - initialEmmision;

        private float additionalLight => desiredLightRange - initialLightRange;

        private float duration;

        private float fireFrequency;

        private Animator modelAnimator;

        private float fireStopwatch;

        private Transform projectilesSpawnPoint;

        private Transform bulletSpawnHelperPoint;

        private Renderer eyeRenderer;

        private MaterialPropertyBlock eyePropertyBlock;

        private float initialEmmision;

        private Light headLight;

        private float initialLightRange = ColossusFactory.normalEyeLightRange;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireFrequency = baseFireFrequency / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            projectilesSpawnPoint = FindModelChild("LaserInitialPoint");
            bulletSpawnHelperPoint = FindModelChild("BulletAttackHelper");

            var childLocator = GetModelChildLocator();

            eyeRenderer = childLocator.FindChildComponent<Renderer>("EyeModel");
            eyePropertyBlock = new MaterialPropertyBlock();
            initialEmmision = eyeRenderer.material.GetFloat("_EmPower");
            eyePropertyBlock.SetFloat("_EmPower", initialEmmision + additionalEmmision); // initial emmision is max since we shoot on enter
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

            headLight = childLocator.FindChildComponent<Light>("HeadLight");
            headLight.range = desiredLightRange;

            PlayAnimation("Body", "LaserBeamLoop");
            Fire();
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                // math is fun
                modelAnimator.SetFloat(MissingAnimationParameters.aimYawCycle, Mathf.Clamp(age / baseDuration, 0f, 0.99f));
                modelAnimator.SetFloat(MissingAnimationParameters.aimPitchCycle, pitch);
                //modelAnimator.SetFloat(aimPitchCycle, Mathf.Clamp(pitchStart + pitchStep * Mathf.Min(age / (duration / totalTurnCount), totalTurnCount - 1), 0f, 0.99f));
            }

            headLight.range = initialLightRange + (additionalLight * intencityGraph.Evaluate(fireStopwatch * attackSpeedStat));
            eyePropertyBlock.SetFloat("_EmPower", initialEmmision + (additionalEmmision * intencityGraph.Evaluate(fireStopwatch * attackSpeedStat))); // multiply by attackspeedstat so we can get back to baseFireFrequency 
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            fireStopwatch += Time.fixedDeltaTime;
            if(fireStopwatch > fireFrequency)
            {
                Fire();
                fireStopwatch -= fireFrequency;
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
                    var rotation = Quaternion.LookRotation(bulletSpawnHelperPoint.position - projectilesSpawnPoint.position, Vector3.up);
                    ProjectileManager.instance.FireProjectile(projectilePrefab, projectilesSpawnPoint.position, rotation, base.gameObject, damageStat * damageCoefficient, forceMagnitude, RollCrit(), RoR2.DamageColorIndex.Default, null, projectileSpeed);
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
