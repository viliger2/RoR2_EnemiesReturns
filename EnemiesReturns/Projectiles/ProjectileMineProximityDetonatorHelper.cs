using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(MineProximityDetonator))]
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
