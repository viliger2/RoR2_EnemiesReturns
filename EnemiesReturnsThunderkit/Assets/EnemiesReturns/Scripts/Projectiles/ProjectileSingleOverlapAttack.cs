using RoR2;
using RoR2.Projectile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileDamage))]
    [RequireComponent(typeof(HitBoxGroup))]
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileSingleOverlapAttack : MonoBehaviour
    {
        public ProjectileController projectileController;

        public ProjectileDamage projectileDamage;

        public float delay = 0f;

        public float damageCoefficient;

        public GameObject impactEffect;

        public Vector3 forceVector;

        public float procCoefficient;

        public int maximumOverlapTargets = 100;

        private void Start()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            if(delay <= 0f)
            {
                FireAttack();
            } else
            {
                Invoke("FireAttack", delay);
            }
        }

        public void FireAttack()
        {
            var attack = new OverlapAttack();
            attack.procChainMask = projectileController.procChainMask;
            attack.procCoefficient = projectileController.procCoefficient * procCoefficient;
            attack.attacker = projectileController.owner;
            attack.inflictor = base.gameObject;
            attack.teamIndex = projectileController.teamFilter.teamIndex;
            attack.damage = damageCoefficient * projectileDamage.damage;
            attack.forceVector = forceVector + projectileDamage.force * base.transform.forward;
            attack.hitEffectPrefab = impactEffect;
            attack.isCrit = projectileDamage.crit;
            attack.damageColorIndex = projectileDamage.damageColorIndex;
            attack.damageType = projectileDamage.damageType;
            attack.procChainMask = projectileController.procChainMask;
            attack.maximumOverlapTargets = maximumOverlapTargets;
            attack.hitBoxGroup = GetComponent<HitBoxGroup>();

            attack.Fire();
        }
    }
}
