using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileForceVector : MonoBehaviour, IProjectileImpactBehavior
    {
        public Vector3 force;

        private ProjectileController projectileController;

        private bool alive = true;

        private void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!alive)
            {
                return;
            }

            Collider collider = impactInfo.collider;
            if (!collider)
            {
                return;
            }

            var hurtBox = collider.GetComponent<HurtBox>();
            if (!hurtBox)
            {
                return;
            }

            var healthComponent = hurtBox.healthComponent;
            if(healthComponent.gameObject == projectileController.owner)
            {
                return;
            }

            var damageInfo = new DamageInfo
            {
                damage = 0f,
                force = force,
                canRejectForce = false,
                attacker = projectileController.owner,
                inflictor = this.gameObject,
                position = impactInfo.estimatedPointOfImpact
            };
            healthComponent.TakeDamageForce(damageInfo, false, true);
        }
    }
}
