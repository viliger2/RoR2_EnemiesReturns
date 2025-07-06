using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.HeadLaserBarrage
{
    [RegisterEntityState]
    public class HeadLaserBarrageAttack : BaseState
    {
        public static float baseDuration => EnemiesReturns.Configuration.Colossus.LaserBarrageDuration.Value;

        public static float projectileSpeed => EnemiesReturns.Configuration.Colossus.LaserBarrageProjectileSpeed.Value;

        public static float baseFireFrequency => EnemiesReturns.Configuration.Colossus.LaserBarrageFrequency.Value;

        public static int projectilesPerShot => EnemiesReturns.Configuration.Colossus.LaserBarrageProjectileCount.Value;

        public static float damageCoefficient => EnemiesReturns.Configuration.Colossus.LaserBarrageDamage.Value;

        public static float forceMagnitude => EnemiesReturns.Configuration.Colossus.LaserBarrageForce.Value;

        public static float spread => EnemiesReturns.Configuration.Colossus.LaserBarrageSpread.Value;

        public static float desiredEmission = ColossusBody.MAX_BARRAGE_EMISSION; // max total emmision, we jump from 3.5 to 7 with intencityGraph

        public static float desiredLightRange = ColossusBody.MAX_EYE_LIGHT_RANGE;

        public static AnimationCurve intencityGraph;

        public static GameObject projectilePrefab;

        public static float moveSpeedMultiplier => EnemiesReturns.Configuration.Colossus.LaserBarrageMoveSpeedMultiplier.Value;

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

        private float initialLightRange = ColossusBody.NORMAL_EYE_LIGHT_RANGE;

        private AimAnimator aimAnimator;

        private AimAnimator.DirectionOverrideRequest animatorDirectionOverrideRequest;

        private Vector3 aimDirection;

        private bool aimAnimatorWasDisabled;

        private Vector3 targetMoveVector;


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

            aimAnimator = modelLocator.modelTransform.gameObject.GetComponent<AimAnimator>();
            if (aimAnimator)
            {
                if (!aimAnimator.enabled)
                {
                    aimAnimatorWasDisabled = true;
                    aimAnimator.enabled = true;
                }
                animatorDirectionOverrideRequest = aimAnimator.RequestDirectionOverride(GetAimDirection);
            }
            aimDirection = GetTargetDirection();

            PlayAnimation("Body", "LasetBeamWalkF");
            PlayAnimation("Gesture, Additive", "LaserBeamLoop");
            Fire();
        }

        public Vector3 GetAimDirection()
        {
            return aimDirection;
        }

        private Vector3 GetTargetDirection()
        {
            if (inputBank)
            {
                return inputBank.aimDirection;
            }
            return transform.forward;
        }

        public override void Update()
        {
            base.Update();

            headLight.range = initialLightRange + (additionalLight * intencityGraph.Evaluate(fireStopwatch * attackSpeedStat));
            eyePropertyBlock.SetFloat("_EmPower", initialEmmision + (additionalEmmision * intencityGraph.Evaluate(fireStopwatch * attackSpeedStat))); // multiply by attackspeedstat so we can get back to baseFireFrequency 
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

        }

        public override void FixedUpdate()
        {
            aimDirection = Vector3.RotateTowards(aimDirection, GetTargetDirection(), 100f * (Mathf.PI / 180f) * GetDeltaTime(), float.PositiveInfinity);

            Vector3 targetMoveVelocity = Vector3.zero;
            targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(targetMoveVector, base.inputBank.aimDirection, ref targetMoveVelocity, 0.01f, 45f), Vector3.up).normalized;
            base.characterDirection.moveVector = targetMoveVector;
            Vector3 forward = base.characterDirection.forward;
            float value = moveSpeedStat * moveSpeedMultiplier;
            base.characterMotor.moveDirection = forward * moveSpeedMultiplier;
            modelAnimator.SetFloat(AnimationParameters.forwardSpeed, value);

            base.FixedUpdate();

            fireStopwatch += Time.fixedDeltaTime;
            if (fireStopwatch > fireFrequency)
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
                    ProjectileManager.instance.FireProjectile(projectilePrefab, projectilesSpawnPoint.position, rotation, base.gameObject, damageStat * damageCoefficient, forceMagnitude, RollCrit(), RoR2.DamageColorIndex.Default, null, projectileSpeed, DamageSource.Utility);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Additive", "BufferEmpty");
            base.characterMotor.moveDirection = Vector3.zero;
            if (aimAnimator && animatorDirectionOverrideRequest != null)
            {
                aimAnimator.RemoveRequest(animatorDirectionOverrideRequest);
                if (aimAnimatorWasDisabled)
                {
                    aimAnimator.enabled = false;
                }
                animatorDirectionOverrideRequest.Dispose();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
