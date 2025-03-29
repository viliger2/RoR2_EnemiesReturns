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

        public float damageCoefficient;

        public GameObject impactEffect;

        public Vector3 forceVector;

        public float procCoefficient;

        public int maximumOverlapTargets = 100;

        private void Start()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();

            var attack = new OverlapAttack
            {
                procChainMask = projectileController.procChainMask,
                procCoefficient = projectileController.procCoefficient * procCoefficient,
                attacker = projectileController.owner,
                inflictor = base.gameObject,
                teamIndex = projectileController.teamFilter.teamIndex,
                damage = damageCoefficient * projectileDamage.damage,
                forceVector = forceVector + projectileDamage.force * base.transform.forward,
                hitEffectPrefab = impactEffect,
                isCrit = projectileDamage.crit,
                damageColorIndex = projectileDamage.damageColorIndex,
                damageType = projectileDamage.damageType
            };
            attack.procChainMask = projectileController.procChainMask;
            attack.maximumOverlapTargets = maximumOverlapTargets;
            attack.hitBoxGroup = GetComponent<HitBoxGroup>();

            attack.Fire();
        }
    }
}
