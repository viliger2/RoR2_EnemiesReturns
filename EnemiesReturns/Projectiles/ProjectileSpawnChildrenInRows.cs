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
    public class ProjectileSpawnChildrenInRows : MonoBehaviour, IProjectileImpactBehavior
    {
        public float radius = 9f;

        public int numberOfRows = 3;

        public float childrenDamageCoefficient;

        public float delayEachRow = 0.5f;

        public GameObject childPrefab;

        protected ProjectileController projectileController;

        protected ProjectileDamage projectileDamage;

        protected TeamFilter teamFilter;

        protected bool alive = true;

        private void Awake()
        {
            projectileController = GetComponent<ProjectileController>();
            projectileDamage = GetComponent<ProjectileDamage>();
            teamFilter = GetComponent<TeamFilter>();
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if(!NetworkServer.active)
            {
                return;
            }

            var smallRadius = (radius / ((numberOfRows - 1) + 0.5f));
            var childScale = new Vector3(smallRadius, smallRadius, smallRadius);
            //var mainSphere = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            //mainSphere.transform.parent = gameObject.transform;
            //mainSphere.transform.localScale = new Vector3(radius * 2, 0.1f, radius * 2);
            //mainSphere.transform.localPosition = Vector3.zero;
            for (int i = 0; i < numberOfRows; i++)
            {
                float delay = delayEachRow + delayEachRow * i;
                float fromCentre = smallRadius * i;
                if (i == 0)
                {
                    SpawnChild(base.transform.position, childScale, delay);
                }
                else
                {
                    var angleCos = (Mathf.Pow(fromCentre, 2f) + Mathf.Pow(fromCentre, 2f) - Mathf.Pow(smallRadius, 2f)) / (2 * fromCentre * fromCentre);
                    var angle = Mathf.Acos(angleCos) / (MathF.PI / 180);
                    int rockCount = (int)(360f / angle);
                    float newAngle = 360f / rockCount;
                    //Debug.Log("angle: " + angle.ToString() + ", rockCount: " + rockCount.ToString() + ", newAngle: " + newAngle.ToString());
                    for (int k = 0; k < rockCount; k++)
                    {
                        var x = fromCentre * Mathf.Cos(newAngle * k * Mathf.Deg2Rad);
                        var z = fromCentre * Mathf.Sin(newAngle * k * Mathf.Deg2Rad);

                        if (Physics.Raycast(base.transform.position + new Vector3(x, 5f, x), Vector3.down, out var hit, 100f, LayerIndex.world.intVal))
                        {
                            SpawnChild(hit.point, childScale, delay);
                        }
                        else
                        {
                            SpawnChild(base.transform.position + new Vector3(x, 0f, z), childScale, delay);
                        }
                    }
                }

            }
        }

        private void SpawnChild(Vector3 position, Vector3 scale, float delay)
        {
            var newObject = UnityEngine.Object.Instantiate(childPrefab, position, Quaternion.identity);
            newObject.transform.localScale = scale;

            var newController = newObject.GetComponent<ProjectileController>();
            if(newController)
            {
                newController.procChainMask = projectileController.procChainMask;
                newController.procCoefficient = projectileController.procCoefficient;
                newController.Networkowner = projectileController.Networkowner;

                if(newController.ghostPrefab)
                {
                    newController.ghostPrefab = UnityEngine.Object.Instantiate(newController.ghostPrefab);
                    if (newController.ghostPrefab.TryGetComponent<InvokeDelayedEventOnStart>(out var component))
                    {
                        component.timer = delay;
                        component.enabled = true;
                    }
                }
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

            var enabler = newObject.GetComponent<ComponentStateSwitcher>();
            enabler.delay = delay;
            enabler.enabled = true;

            NetworkServer.Spawn(newObject);
        }
    }
}
