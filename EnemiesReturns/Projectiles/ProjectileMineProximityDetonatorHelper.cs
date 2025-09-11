using EnemiesReturns.Behaviors;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(MineProximityDetonatorWithGameObjectCheck))]
    public class ProjectileMineProximityDetonatorHelper : MonoBehaviour
    {
        public ProjectileImpactExplosion impactExplosion;

        private void Awake()
        {
            var proximityDetonator = GetComponent<MineProximityDetonator>();
            proximityDetonator.triggerEvents.AddListener(Detonate);
        }

        private void Detonate()
        {
            if (impactExplosion)
            {
                impactExplosion.SetAlive(false);
            }
        }
    }
}
