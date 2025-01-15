using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileController))]
    public class ProjectileImpaleOnEnemyFixed : MonoBehaviour, IProjectileImpactBehavior
    {
        public ProjectileController controller;

        public GameObject impalePrefab;

        private bool spawned;

        // TODO: do not attach to allies
        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (spawned)
            {
                return;
            }
            Collider collider = impactInfo.collider;
            if (!collider)
            {
                return;
            }
            HurtBox component = collider.GetComponent<HurtBox>();
            if (component)
            {
                Vector3 estimatedPointOfImpact = impactInfo.estimatedPointOfImpact;
                GameObject obj = UnityEngine.Object.Instantiate(impalePrefab);
                obj.transform.position = estimatedPointOfImpact;
                obj.transform.rotation = base.transform.rotation;
                if (controller && controller.ghost)
                {
                    obj.transform.rotation = controller.ghost.transform.rotation;
                }
                obj.transform.parent = component.transform;
                spawned = true;
            }
        }
    }

}
