using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileExplosionComboLimit : ProjectileExplosion
    {
        [Header("Combo Limit Additions")]
        public bool adjustSpawnPoint;

        public override void FireChild()
        {
            if(projectileController.combo == 0)
            {
                return;
            }
            base.FireChild();
        }

        public override void InitializeChild(GameObject childProjectileInstance)
        {
            base.InitializeChild(childProjectileInstance);
            var controller = childProjectileInstance.GetComponent<ProjectileController>();
            if (controller)
            {
                controller.Networkcombo = (byte)(projectileController.combo - 1);
            }
            if (adjustSpawnPoint)
            {
                var sphereCollider = childProjectileInstance.GetComponent<SphereCollider>();
                if (sphereCollider)
                {
                    childProjectileInstance.transform.position = childProjectileInstance.transform.position + Vector3.up * (sphereCollider.radius * 2);
                }
            }
        }
    }
}
