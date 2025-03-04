using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileGhostController))]
    public class ProjectileGhostRotateTowardsMoveDirection : MonoBehaviour
    {
        public ProjectileGhostController ghostController;

        private Transform authorityTransform;

        private Rigidbody rigidbody;

        private void OnEnable()
        {
            if (!ghostController)
            {
                ghostController = GetComponent<ProjectileGhostController>();
            }
            authorityTransform = ghostController.authorityTransform ? ghostController.authorityTransform : ghostController.predictionTransform;
            if (authorityTransform)
            {
                rigidbody = authorityTransform.GetComponent<Rigidbody>();
            }
        }

        private void FixedUpdate()
        {
            if (!authorityTransform)
            {
                authorityTransform = ghostController.authorityTransform ? ghostController.authorityTransform : ghostController.predictionTransform;
            }
            if (authorityTransform && !rigidbody)
            {
                rigidbody = authorityTransform.GetComponent<Rigidbody>();
            }
        }

        private void LateUpdate()
        {
            if (rigidbody)
            {
                transform.LookAt(rigidbody.position + rigidbody.velocity);
            }
        }
    }
}
