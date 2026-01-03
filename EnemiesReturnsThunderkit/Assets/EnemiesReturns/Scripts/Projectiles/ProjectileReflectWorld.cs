using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileReflectWorld : MonoBehaviour, IProjectileImpactBehavior
    {
        //public int maxBounces = 10;

        //public GameObject projectilePrefab;

        //private ProjectileController controller;

        //private ProjectileDamage projectileDamage;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            //controller = GetComponent<ProjectileController>();
            //projectileDamage = GetComponent<ProjectileDamage>();
        }

        private void FixedUpdate()
        {
            rb.angularVelocity = Vector3.zero;
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            transform.forward = Vector3.Reflect(transform.forward, impactInfo.estimatedImpactNormal);

            //if (!NetworkServer.active)
            //{
            //    return;
            //}

            //if(controller.combo < maxBounces)
            //{
            //    UnityEngine.Object.Destroy(gameObject);
            //    return;
            //}

            //var newForward = Vector3.Reflect(transform.forward, impactInfo.estimatedImpactNormal);

            //var projectileInfo = new FireProjectileInfo()
            //{
            //    crit = projectileDamage.crit,
            //    owner = controller.owner,
            //    position = transform.position,
            //    projectilePrefab = projectilePrefab,
            //    rotation = Util.QuaternionSafeLookRotation(newForward),
            //    damage = projectileDamage.damage,
            //    damageTypeOverride = DamageTypeCombo.Generic,
            //    comboNumber = (byte)(controller.combo + 1)
            //};

            //ProjectileManager.instance.FireProjectile(projectileInfo);

            //UnityEngine.Object.Destroy(gameObject);
        }
    }
}
