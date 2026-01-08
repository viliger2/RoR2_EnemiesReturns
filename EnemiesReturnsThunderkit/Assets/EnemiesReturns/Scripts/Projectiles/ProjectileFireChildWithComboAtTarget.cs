using EnemiesReturns.EditorHelpers;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileTargetComponent))]
    [RequireComponent(typeof(ProjectileController))]
    [RequireComponent(typeof(ProjectileDamage))]
    public class ProjectileFireChildWithComboAtTarget : MonoBehaviour
    {
        public PrefabDef childToFire;

        private ProjectileTargetComponent targetComponent;

        private ProjectileDamage damage;

        private ProjectileController controller;

        public void Awake()
        {
            targetComponent = GetComponent<ProjectileTargetComponent>();
            controller = GetComponent<ProjectileController>();
            damage = GetComponent<ProjectileDamage>();
        }

        public void FireProjectileServer()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if(!controller || !damage)
            {
                return;
            }

            if(controller.combo == 0)
            {
                return;
            }

            var rotation = transform.forward.normalized;
            if(targetComponent && targetComponent.target)
            {
                rotation = (targetComponent.target.position - transform.position).normalized;
            }

            var info = new FireProjectileInfo()
            {
                comboNumber = (byte)(controller.combo - 1),
                crit = damage.crit,
                damage = damage.damage,
                damageColorIndex = damage.damageColorIndex,
                damageTypeOverride = damage.damageType,
                force = damage.force,
                owner = controller.owner,
                position = transform.position,
                procChainMask = controller.procChainMask,
                projectilePrefab = childToFire.prefab,
                rotation = Util.QuaternionSafeLookRotation(rotation),
                fuseOverride = 2f
            };

            ProjectileManager.instance.FireProjectile(info);
        }
    }
}
