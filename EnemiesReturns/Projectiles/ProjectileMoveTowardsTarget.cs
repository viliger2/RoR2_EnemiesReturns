using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileMoveTowardsTarget : MonoBehaviour
    {
        public float speed;

        public bool moveYAxis = false;

        private ProjectileTargetComponent target;

        private void Start()
        {
            if (!NetworkServer.active)
            {
                this.enabled = false;
                return;
            }

            target = GetComponent<ProjectileTargetComponent>();
        }

        private void FixedUpdate()
        {
            if (target.target)
            {
                var newVector = Vector3.MoveTowards(gameObject.transform.position, target.target.position, speed * Time.fixedDeltaTime);
                if (moveYAxis)
                {
                    gameObject.transform.position = newVector;
                }
                else
                {
                    gameObject.transform.position = new Vector3(newVector.x, gameObject.transform.position.y, newVector.z);
                }
            }
        }
    }
}
