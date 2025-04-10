using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.Colossus.HeadLaser
{
    [RegisterEntityState]
    public class HeadLaserAttack : BaseState
    {
        // unfuck transforms, too many unncessesary points
        // maybe add a hit\filter callback so enemies only get hit every x seconds\only once per swipe
        public static float baseDuration = 20f;

        public static float baseFireFrequency = 0.06f;

        public static float laserDamage = 0.5f;

        public static float laserForce = 0f;

        public static float laserRadius = 7.5f;

        public static GameObject beamPrefab;

        public static int totalTurnCount = 4;

        public static float pitchStart = 0.05f;

        public static float pitchStep = 0.15f;

        private static float laserMaxDistance = 2000f;

        private static int bulletCount = 6;

        private float duration;

        private Animator modelAnimator;

        private static readonly int aimYawCycleHash = Animator.StringToHash("aimYawCycle");

        private static readonly int aimPitchCycleHash = Animator.StringToHash("aimPitchCycle");

        private GameObject beamInstance;

        private Transform effectPoint;

        private Transform initialPoint;

        private Transform bulletSpawnHelperPoint;

        private float fireFrequency;

        private float stopwatch;

        private float angleAttackSpeedMult; // angle adjustment to attackspeed, so we turn faster 

        private float anglePerSecond; // how much we turn per second, used for sin() calculation

        private BulletAttack bulletAttack;

        private float angleBetweenBullets;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            angleAttackSpeedMult = baseDuration / duration;
            anglePerSecond = totalTurnCount * 90 / baseDuration; // 90 is for sin()
            fireFrequency = baseFireFrequency / attackSpeedStat;

            effectPoint = FindModelChild("LaserInitialPoint");
            initialPoint = FindModelChild("LaserInitialPoint");
            bulletSpawnHelperPoint = FindModelChild("BulletAttackHelper");

            bulletAttack = CreateBulletAttack();

            modelAnimator = GetModelAnimator();

            angleBetweenBullets = 360 / bulletCount;

            beamInstance = UnityEngine.Object.Instantiate(beamPrefab);
            beamInstance.transform.SetParent(effectPoint, worldPositionStays: true);

            PlayAnimation("Body", "LaserBeamLoop");
            UpdateBeamTransforms();
        }

        public override void Update()
        {
            base.Update();
            if (modelAnimator)
            {
                // math is fun
                modelAnimator.SetFloat(aimYawCycleHash, Mathf.Clamp(Mathf.Abs(Mathf.Sin(age * anglePerSecond * angleAttackSpeedMult * Mathf.Deg2Rad)), 0f, 0.99f));
                modelAnimator.SetFloat(aimPitchCycleHash, Mathf.Clamp(pitchStart + pitchStep * Mathf.Min(age / (duration / totalTurnCount), totalTurnCount - 1), 0f, 0.99f));
            }
            UpdateBeamTransforms();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (isAuthority && stopwatch >= fireFrequency)
            {
                if (bulletSpawnHelperPoint)
                {
                    for (int i = 0; i < bulletCount; i++)
                    {
                        var x = laserRadius / 2 / 14 * Mathf.Cos(angleBetweenBullets * i * Mathf.Deg2Rad); // 14 is colossus scale
                        var y = laserRadius / 2 / 14 * Mathf.Sin(angleBetweenBullets * i * Mathf.Deg2Rad);
                        bulletSpawnHelperPoint.localPosition = new Vector3(x, y, 0f);
                        bulletAttack.origin = bulletSpawnHelperPoint.position;
                        bulletAttack.aimVector = new Ray(bulletSpawnHelperPoint.position, bulletSpawnHelperPoint.forward).direction;
                        bulletAttack.radius = laserRadius / 4;
                        //bulletAttack.filterCallback
                        //bulletAttack.hitCallback
                        bulletAttack.Fire();
                    }
                }

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
            bulletAttack.origin = initialPoint.position;
            bulletAttack.aimVector = new Ray(initialPoint.position, initialPoint.forward).direction;
            bulletAttack.minSpread = 0f;
            bulletAttack.maxSpread = 0f;
            bulletAttack.bulletCount = 1;
            bulletAttack.damage = laserDamage * damageStat;
            bulletAttack.force = laserForce;
            bulletAttack.tracerEffectPrefab = null;
            bulletAttack.muzzleName = "";
            bulletAttack.hitEffectPrefab = null;
            bulletAttack.isCrit = false;
            bulletAttack.radius = laserRadius;
            bulletAttack.smartCollision = false;
            bulletAttack.damageType = DamageSource.Special;
            bulletAttack.maxDistance = laserMaxDistance;
            bulletAttack.procChainMask = default;
            bulletAttack.damageColorIndex = DamageColorIndex.Default;
            bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
            return bulletAttack;
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
            base.OnExit();
            //RoR2Application.onLateUpdate -= UpdateBeamTransformsInLateUpdate;
            UnityEngine.Object.Destroy(beamInstance);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
