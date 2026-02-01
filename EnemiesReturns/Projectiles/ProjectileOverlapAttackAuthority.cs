using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(HitBoxGroup))]
    public class ProjectileOverlapAttackAuthority : MonoBehaviour, IProjectileImpactBehavior
    {
        private ProjectileController projectileController;

        private ProjectileDamage projectileDamage;

        public float damageCoefficient;

        public GameObject impactEffect;

        public Vector3 forceVector;

        public float pushAwayForce;

        public float overlapProcCoefficient = 1f;

        public int maximumOverlapTargets = 100;

        private OverlapAttackAuthority attack;

        public float fireFrequency = 60f;

        [Tooltip("If non-negative, the attack clears its hit memory at the specified interval.")]
        public float resetInterval = -1f;

        private float resetTimer;

        public bool requireDelayOnePhysicsFrame;

        public bool isOverrideTeam;

        public TeamIndex OverrideTeamIndex;

        [Tooltip("If artifact of chaos is active, then this overlap attack can hurt the owner")]
        public bool canHitOwner;

        private float fireTimer;

        private void Start()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            attack = new OverlapAttackAuthority();
            UpdateAttackValues();
            projectileDamage.DamageInfoChanged += UpdateAttackValues;
            attack.hitBoxGroup = GetComponent<HitBoxGroup>();
            int num = attack.hitBoxGroup.hitBoxes.Length;
        }

        private void OnEnable()
        {
            if ((bool)projectileDamage)
            {
                projectileDamage.DamageInfoChanged += UpdateAttackValues;
            }
        }

        private void OnDisable()
        {
            if ((bool)projectileDamage)
            {
                projectileDamage.DamageInfoChanged -= UpdateAttackValues;
            }
        }

        public void UpdateAttackValues()
        {
            attack.procChainMask = projectileController.procChainMask;
            attack.procCoefficient = projectileController.procCoefficient * overlapProcCoefficient;
            attack.attacker = projectileController.owner;
            attack.inflictor = base.gameObject;
            attack.teamIndex = (isOverrideTeam ? OverrideTeamIndex : projectileController.teamFilter.teamIndex);
            attack.damage = damageCoefficient * projectileDamage.damage;
            attack.forceVector = forceVector + projectileDamage.force * base.transform.forward;
            attack.pushAwayForce = pushAwayForce;
            attack.hitEffectPrefab = impactEffect;
            attack.isCrit = projectileDamage.crit;
            attack.damageColorIndex = projectileDamage.damageColorIndex;
            attack.damageType = projectileDamage.damageType;
            attack.procChainMask = projectileController.procChainMask;
            attack.maximumOverlapTargets = maximumOverlapTargets;
            attack.attackerFiltering = (canHitOwner ? AttackerFiltering.AlwaysHit : AttackerFiltering.NeverHitSelf);
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
        }

        private void FixedUpdate()
        {
            MyFixedUpdate(Time.fixedDeltaTime);
        }

        public void MyFixedUpdate(float deltaTime)
        {
            if (resetInterval >= 0f)
            {
                resetTimer -= deltaTime;
                if (resetTimer <= 0f)
                {
                    resetTimer = resetInterval;
                    ResetOverlapAttack();
                }
            }
            fireTimer -= deltaTime;
            if (!(fireTimer <= 0f))
            {
                return;
            }
            fireTimer = 1f / fireFrequency;
            attack.damage = damageCoefficient * projectileDamage.damage;
            attack.Fire();
        }

        public void ResetOverlapAttack()
        {
            attack.damageType = projectileDamage.damageType;
            attack.ResetIgnoredHealthComponents();
        }

        public void SetDamageCoefficient(float newDamageCoefficient)
        {
            damageCoefficient = newDamageCoefficient;
        }

        public void AddToDamageCoefficient(float bonusDamageCoefficient)
        {
            damageCoefficient += bonusDamageCoefficient;
        }

        public void AddIgnoredHitList(HealthComponent healthComponent)
        {
            if (attack != null)
            {
                attack.addIgnoredHitList(healthComponent);
            }
        }
    }
}
