using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileEnableHomingAfterTargetAquired : MonoBehaviour
    {
        public bool changeSpeedAfterTargetFound;

        public float newSpeed;

        private ProjectileSimple projectileSimple;

        //private ProjectileSteerTowardTarget steerTowardsTarget;

        private ProjectileSphereTargetFinder sphereTargetFinder;

        private void Start()
        {
            if (!NetworkServer.active)
            {
                this.enabled = false;
                return;
            }

            projectileSimple = GetComponent<ProjectileSimple>();
            sphereTargetFinder = GetComponent<ProjectileSphereTargetFinder>();
            if (sphereTargetFinder)
            {
                if (sphereTargetFinder.onNewTargetFound == null)
                {
                    sphereTargetFinder.onNewTargetFound = new UnityEngine.Events.UnityEvent();
                }
                sphereTargetFinder.onNewTargetFound.AddListener(OnTargetFoundChangeSpeed);
            }
        }

        private void OnTargetFoundChangeSpeed()
        {
            if (projectileSimple)
            {
                projectileSimple.updateAfterFiring = true;
                if (changeSpeedAfterTargetFound)
                {
                    projectileSimple.desiredForwardSpeed = newSpeed;
                }
            }
        }

    }
}
