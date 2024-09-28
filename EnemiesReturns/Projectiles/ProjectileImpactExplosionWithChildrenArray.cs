using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.Projectile.ProjectileImpactExplosion;

namespace EnemiesReturns.Projectiles
{
    // ORIGINAL PROJECTILE
    // DO NOT STEAL
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileImpactExplosionWithChildrenArray : MonoBehaviour, IProjectileImpactBehavior
    {
        protected ProjectileController projectileController;

        protected ProjectileDamage projectileDamage;

        protected bool alive = true;

        #region MainProperties

        [Header("Main Properties")]
        public BlastAttack.FalloffModel falloffModel = BlastAttack.FalloffModel.Linear;

        public float blastRadius;

        [Tooltip("The percentage of the damage, proc coefficient, and force of the initial projectile. Ranges from 0-1")]
        public float blastDamageCoefficient;

        public float blastProcCoefficient = 1f;

        public AttackerFiltering blastAttackerFiltering;

        public Vector3 bonusBlastForce;

        public bool canRejectForce = true;

        public HealthComponent projectileHealthComponent;

        public GameObject explosionEffect;

        #endregion

        #region ChildProperties

        [Header("Child Properties")]
        [Tooltip("Does this projectile release children on death?")]
        public bool fireChildren;

        public GameObject childrenProjectilePrefab;

        public int childrenCount;

        [Tooltip("What percentage of our damage does the children get?")]
        public float childrenDamageCoefficient;

        public float childredMinRollDegrees;

        public float childrenRangeRollDegrees;

        public float childrenMinPitchDegrees;

        public float childrenRangePitchDegrees;

        #endregion

        #region DoT

        [Tooltip("If true, applies a DoT given the following properties")]
        [Header("DoT Properties")]
        public bool applyDot;

        public DotController.DotIndex dotIndex = DotController.DotIndex.None;

        [Tooltip("Duration in seconds of the DoT.  Unused if calculateTotalDamage is true.")]
        public float dotDuration;

        [Tooltip("Multiplier on the per-tick damage")]
        public float dotDamageMultiplier = 1f;

        [Tooltip("If true, we cap the numer of DoT stacks for this attacker.")]
        public bool applyMaxStacksFromAttacker;

        [Tooltip("The maximum number of stacks that we can apply for this attacker")]
        public uint maxStacksFromAttacker = uint.MaxValue;

        [Tooltip("If true, we disregard the duration and instead specify the total damage.")]
        public bool calculateTotalDamage;

        [Tooltip("totalDamage = totalDamageMultiplier * attacker's damage")]
        public float totalDamageMultiplier;

        private Vector3 impactNormal = Vector3.up;

        #endregion

        #region DoTZone

        public bool fireDoTZone;

        public GameObject dotZoneProjectilePrefab;

        public float dotZoneDamageCoefficient;

        public float dotZoneMinRollDegrees;

        public float dotZoneRangeRollDegrees;

        public float dotZoneMinPitchDegrees;

        public float dotZoneRangePitchDegrees;

        #endregion

        [Header("Impact & Lifetime Properties")]
        public GameObject impactEffect;

        public NetworkSoundEventDef lifetimeExpiredSound;

        public float offsetForLifetimeExpiredSound;

        public bool destroyOnEnemy = true;

        public bool destroyOnWorld;

        public bool impactOnWorld = true;

        public bool timerAfterImpact;

        public float lifetime;

        public float lifetimeAfterImpact;

        public float lifetimeRandomOffset;

        private float stopwatch;

        private float stopwatchAfterImpact;

        private bool hasImpact;

        private bool hasPlayedLifetimeExpiredSound;

        public TransformSpace transformSpace;

        protected void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            lifetime += UnityEngine.Random.Range(0f, lifetimeRandomOffset);
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (!NetworkServer.active && !projectileController.isPrediction)
            {
                return;
            }
            if (timerAfterImpact && hasImpact)
            {
                stopwatchAfterImpact += Time.fixedDeltaTime;
            }
            bool num = stopwatch >= lifetime;
            bool flag = timerAfterImpact && stopwatchAfterImpact > lifetimeAfterImpact;
            bool flag2 = (bool)projectileHealthComponent && !projectileHealthComponent.alive;
            if (num || flag || flag2)
            {
                alive = false;
            }
            if (alive && !hasPlayedLifetimeExpiredSound)
            {
                bool flag3 = stopwatch > lifetime - offsetForLifetimeExpiredSound;
                if (timerAfterImpact)
                {
                    flag3 |= stopwatchAfterImpact > lifetimeAfterImpact - offsetForLifetimeExpiredSound;
                }
                if (flag3)
                {
                    hasPlayedLifetimeExpiredSound = true;
                    if (NetworkServer.active && (bool)lifetimeExpiredSound)
                    {
                        PointSoundManager.EmitSoundServer(lifetimeExpiredSound.index, base.transform.position);
                    }
                }
            }
            if (!alive)
            {
                explosionEffect = impactEffect ?? explosionEffect;
                Detonate();
            }
        }

        public void Detonate()
        {
            if (NetworkServer.active)
            {
                DetonateServer();
            }
            UnityEngine.Object.Destroy(base.gameObject);
        }

        protected void DetonateServer()
        {
            if ((bool)explosionEffect)
            {
                EffectManager.SpawnEffect(explosionEffect, new EffectData
                {
                    origin = base.transform.position,
                    scale = blastRadius
                }, transmit: true);
            }
            if ((bool)projectileDamage)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.position = base.transform.position;
                blastAttack.baseDamage = projectileDamage.damage * blastDamageCoefficient;
                blastAttack.baseForce = projectileDamage.force * blastDamageCoefficient;
                blastAttack.radius = blastRadius;
                blastAttack.attacker = (projectileController.owner ? projectileController.owner.gameObject : null);
                blastAttack.inflictor = base.gameObject;
                blastAttack.teamIndex = projectileController.teamFilter.teamIndex;
                blastAttack.crit = projectileDamage.crit;
                blastAttack.procChainMask = projectileController.procChainMask;
                blastAttack.procCoefficient = projectileController.procCoefficient * blastProcCoefficient;
                blastAttack.bonusForce = bonusBlastForce;
                blastAttack.falloffModel = falloffModel;
                blastAttack.damageColorIndex = projectileDamage.damageColorIndex;
                blastAttack.damageType = projectileDamage.damageType;
                blastAttack.attackerFiltering = blastAttackerFiltering;
                blastAttack.canRejectForce = canRejectForce;
                BlastAttack.Result result = blastAttack.Fire();
                OnBlastAttackResult(blastAttack, result);
            }
            if (fireChildren)
            {
                FireChild();
            }
            if (fireDoTZone)
            {
                FireDoTZone();
            }
        }

        protected Quaternion GetRandomChildRollPitch()
        {
            Quaternion quaternion = Quaternion.AngleAxis(childredMinRollDegrees + UnityEngine.Random.Range(0f, childrenRangeRollDegrees), Vector3.forward);
            Quaternion quaternion2 = Quaternion.AngleAxis(childrenMinPitchDegrees + UnityEngine.Random.Range(0f, childrenRangePitchDegrees), Vector3.left);
            return quaternion * quaternion2;
        }

        protected virtual Quaternion GetRandomDirectionForChild()
        {
            Quaternion randomChildRollPitch = GetRandomChildRollPitch();

            switch (transformSpace)
            {
                case TransformSpace.Local:
                    return base.transform.rotation * randomChildRollPitch;
                case TransformSpace.Normal:
                    return Quaternion.FromToRotation(Vector3.forward, impactNormal) * randomChildRollPitch;
                default:
                    return randomChildRollPitch;
            }
        }

        protected void FireChild()
        {
            float angle = 360f / childrenCount;
            float startAngle = UnityEngine.Random.Range(0, 360f);
            var normalized = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
            var vector = Vector3.RotateTowards(Vector3.up, normalized, 0.436332315f, float.PositiveInfinity);
            var position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
            for (int i = 0; i < childrenCount; i++)
            {
                var forward = Quaternion.AngleAxis(startAngle + angle * i, Vector3.up) * vector;
                GameObject obj = UnityEngine.Object.Instantiate(childrenProjectilePrefab, position, Util.QuaternionSafeLookRotation(forward));
                ProjectileController component = obj.GetComponent<ProjectileController>();
                if ((bool)component)
                {
                    component.procChainMask = projectileController.procChainMask;
                    component.procCoefficient = projectileController.procCoefficient;
                    component.Networkowner = projectileController.owner;
                }
                obj.GetComponent<TeamFilter>().teamIndex = GetComponent<TeamFilter>().teamIndex;
                ProjectileDamage component2 = obj.GetComponent<ProjectileDamage>();
                if ((bool)component2)
                {
                    component2.damage = projectileDamage.damage * childrenDamageCoefficient;
                    component2.crit = projectileDamage.crit;
                    component2.force = projectileDamage.force;
                    component2.damageColorIndex = projectileDamage.damageColorIndex;
                }
                NetworkServer.Spawn(obj);
            }
        }

        protected Quaternion GetRandomDoTZoneRollPitch()
        {
            Quaternion quaternion = Quaternion.AngleAxis(dotZoneMinRollDegrees + UnityEngine.Random.Range(0f, dotZoneRangeRollDegrees), Vector3.forward);
            Quaternion quaternion2 = Quaternion.AngleAxis(dotZoneMinPitchDegrees + UnityEngine.Random.Range(0f, dotZoneRangePitchDegrees), Vector3.left);
            return quaternion * quaternion2;
        }

        protected virtual Quaternion GetRandomDirectionForDoTZone()
        {
            Quaternion randomDoTZoneRollPitch = GetRandomDoTZoneRollPitch();

            switch (transformSpace)
            {
                case TransformSpace.Local:
                    return base.transform.rotation * randomDoTZoneRollPitch;
                case TransformSpace.Normal:
                    return Quaternion.FromToRotation(Vector3.forward, impactNormal) * randomDoTZoneRollPitch;
                default:
                    return randomDoTZoneRollPitch;
            }
        }

        protected void FireDoTZone()
        {
            Quaternion randomDirectionForDoTZone = GetRandomDirectionForDoTZone();
            GameObject obj = UnityEngine.Object.Instantiate(dotZoneProjectilePrefab, base.transform.position, randomDirectionForDoTZone);
            ProjectileController component = obj.GetComponent<ProjectileController>();
            if ((bool)component)
            {
                component.procChainMask = projectileController.procChainMask;
                component.procCoefficient = projectileController.procCoefficient;
                component.Networkowner = projectileController.owner;
            }
            obj.GetComponent<TeamFilter>().teamIndex = GetComponent<TeamFilter>().teamIndex;
            ProjectileDamage component2 = obj.GetComponent<ProjectileDamage>();
            if ((bool)component2)
            {
                component2.damage = projectileDamage.damage * dotZoneDamageCoefficient;
                component2.crit = projectileDamage.crit;
                component2.force = projectileDamage.force;
                component2.damageColorIndex = projectileDamage.damageColorIndex;
            }
            NetworkServer.Spawn(obj);
        }

        public void SetExplosionRadius(float newRadius)
        {
            blastRadius = newRadius;
        }

        public void SetAlive(bool newAlive)
        {
            alive = newAlive;
        }

        public bool GetAlive()
        {
            if (!NetworkServer.active)
            {
                Debug.Log("Cannot get alive state. Returning false.");
                return false;
            }
            return alive;
        }

        protected virtual void OnBlastAttackResult(BlastAttack blastAttack, BlastAttack.Result result)
        {
            if (!applyDot)
            {
                return;
            }
            CharacterBody characterBody = blastAttack.attacker?.GetComponent<CharacterBody>();
            BlastAttack.HitPoint[] hitPoints = result.hitPoints;
            for (int i = 0; i < hitPoints.Length; i++)
            {
                BlastAttack.HitPoint hitPoint = hitPoints[i];
                if ((bool)hitPoint.hurtBox && (bool)hitPoint.hurtBox.healthComponent)
                {
                    InflictDotInfo inflictDotInfo = default(InflictDotInfo);
                    inflictDotInfo.victimObject = hitPoint.hurtBox.healthComponent.gameObject;
                    inflictDotInfo.attackerObject = blastAttack.attacker;
                    inflictDotInfo.dotIndex = dotIndex;
                    inflictDotInfo.damageMultiplier = dotDamageMultiplier;
                    InflictDotInfo dotInfo = inflictDotInfo;
                    if (calculateTotalDamage && (bool)characterBody)
                    {
                        dotInfo.totalDamage = characterBody.damage * totalDamageMultiplier;
                    }
                    else
                    {
                        dotInfo.duration = dotDuration;
                    }
                    if (applyMaxStacksFromAttacker)
                    {
                        dotInfo.maxStacksFromAttacker = maxStacksFromAttacker;
                    }
                    if ((bool)characterBody && (bool)characterBody.inventory)
                    {
                        StrengthenBurnUtils.CheckDotForUpgrade(characterBody.inventory, ref dotInfo);
                    }
                    DotController.InflictDot(ref dotInfo);
                }
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!alive)
            {
                return;
            }
            Collider collider = impactInfo.collider;
            impactNormal = impactInfo.estimatedImpactNormal;
            if (!collider)
            {
                return;
            }
            DamageInfo damageInfo = new DamageInfo();
            if ((bool)projectileDamage)
            {
                damageInfo.damage = projectileDamage.damage;
                damageInfo.crit = projectileDamage.crit;
                damageInfo.attacker = (projectileController.owner ? projectileController.owner.gameObject : null);
                damageInfo.inflictor = base.gameObject;
                damageInfo.position = impactInfo.estimatedPointOfImpact;
                damageInfo.force = projectileDamage.force * base.transform.forward;
                damageInfo.procChainMask = projectileController.procChainMask;
                damageInfo.procCoefficient = projectileController.procCoefficient;
            }
            else
            {
                Debug.Log("No projectile damage component!");
            }
            HurtBox component = collider.GetComponent<HurtBox>();
            if ((bool)component)
            {
                if (destroyOnEnemy)
                {
                    HealthComponent healthComponent = component.healthComponent;
                    if ((bool)healthComponent)
                    {
                        if (healthComponent.gameObject == projectileController.owner || ((bool)projectileHealthComponent && healthComponent == projectileHealthComponent))
                        {
                            return;
                        }
                        alive = false;
                    }
                }
            }
            else if (destroyOnWorld)
            {
                alive = false;
            }
            hasImpact = (bool)component || impactOnWorld;
            if (NetworkServer.active && hasImpact)
            {
                GlobalEventManager.instance.OnHitAll(damageInfo, collider.gameObject);
            }
        }
    }
}

