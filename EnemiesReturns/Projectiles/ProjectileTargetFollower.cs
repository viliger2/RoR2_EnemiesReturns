using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileTargetComponent))]
    public class ProjectileTargetFollower : MonoBehaviour
    {
        private ProjectileTargetComponent targetComponent;

        private void Awake()
        {
            targetComponent = GetComponent<ProjectileTargetComponent>();
        }

        private void FixedUpdate()
        {
            if(targetComponent && targetComponent.target)
            {
                this.transform.position = targetComponent.target.position;
            }
        }
    }
}
