using EnemiesReturns.Helpers;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileSpawnChildrenInRowsWithDelay : MonoBehaviour
    {
        public float radius;

        public int numberOfRows;

        public float childrenDamageCoefficient;

        public float delayEachRow;

        public GameObject childPrefab;

        protected ProjectileController projectileController;

        protected ProjectileDamage projectileDamage;

        protected TeamFilter teamFilter;

        protected bool alive = true;

        private float timer;

        private int currentRow;

        private float smallRadius;

        private Vector3 childScale;

        private void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            teamFilter = GetComponent<TeamFilter>();
            smallRadius = (radius / ((numberOfRows - 1) + 0.5f));
            childScale = new Vector3(smallRadius, smallRadius, smallRadius);
            currentRow = 0;
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            timer += Time.fixedDeltaTime;
            if(timer >= delayEachRow)
            {
                if(currentRow == 0)
                {
                    SpawnChild(base.transform.position, childScale);
                } else
                {
                    float fromCentre = smallRadius * currentRow;
                    var angleCos = (Mathf.Pow(fromCentre, 2f) + Mathf.Pow(fromCentre, 2f) - Mathf.Pow(smallRadius, 2f)) / (2 * fromCentre * fromCentre);
                    var angle = Mathf.Acos(angleCos) / (MathF.PI / 180);
                    int rockCount = (int)(360f / angle);
                    float newAngle = 360f / rockCount;
                    //Debug.Log("angle: " + angle.ToString() + ", rockCount: " + rockCount.ToString() + ", newAngle: " + newAngle.ToString());
                    for (int k = 0; k < rockCount; k++)
                    {
                        var x = fromCentre * Mathf.Cos(newAngle * k * Mathf.Deg2Rad);
                        var z = fromCentre * Mathf.Sin(newAngle * k * Mathf.Deg2Rad);

                        if (Physics.Raycast(base.transform.position + new Vector3(x, 5f, z), Vector3.down, out var hit, 100f, LayerIndex.world.mask))
                        {
                            SpawnChild(hit.point, childScale);
                        }
                        else
                        {
                            SpawnChild(base.transform.position + new Vector3(x, 0f, z), childScale);
                        }
                    }
                }
                currentRow++;
                timer -= delayEachRow;
            }

            if(currentRow >= numberOfRows)
            {
                UnityEngine.Object.Destroy(this);
            }
        }

        private void SpawnChild(Vector3 position, Vector3 scale)
        {
            var newObject = UnityEngine.Object.Instantiate(childPrefab, position, Quaternion.identity);
            newObject.transform.localScale = scale;

            var newController = newObject.GetComponent<ProjectileController>();
            if(newController)
            {
                newController.procChainMask = projectileController.procChainMask;
                newController.procCoefficient = projectileController.procCoefficient;
                newController.Networkowner = projectileController.Networkowner;

                //if(newController.ghostPrefab)
                //{
                //    newController.ghostPrefab = UnityEngine.Object.Instantiate(newController.ghostPrefab);
                //    if (newController.ghostPrefab.TryGetComponent<InvokeDelayedEventOnStart>(out var component))
                //    {
                //        component.timer = delay;
                //        component.enabled = true;
                //    }
                //}
            }

            var teamFilter = newObject.GetComponent<TeamFilter>();
            if(teamFilter)
            {
                teamFilter.teamIndex = this.teamFilter.teamIndex;
            }

            var newDamage = newObject.GetComponent<ProjectileDamage>();
            if (newDamage)
            {
                newDamage.damage = projectileDamage.damage * childrenDamageCoefficient;
                newDamage.crit = projectileDamage.crit;
                newDamage.force = projectileDamage.force;
                newDamage.damageColorIndex = projectileDamage.damageColorIndex;
            }

            //var enabler = newObject.GetComponent<ComponentStateSwitcher>();
            //enabler.delay = delay;
            //enabler.enabled = true;

            NetworkServer.Spawn(newObject);
        }
    }
}
