﻿using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using System;
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

        public NetworkSoundEventDef soundEventDef;

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
            childPrefab.transform.localScale = childScale;
            currentRow = 0;
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            timer += Time.fixedDeltaTime;
            if (timer >= delayEachRow)
            {
                if (currentRow == 0)
                {
                    SpawnChild(base.transform.position, childScale);
                }
                else
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
                if (soundEventDef)
                {
                    EntitySoundManager.EmitSoundServer(soundEventDef.akId, this.gameObject);
                }
                currentRow++;
                timer -= delayEachRow;
            }

            if (currentRow >= numberOfRows)
            {
                UnityEngine.Object.Destroy(this);
            }
        }

        private void SpawnChild(Vector3 position, Vector3 scale)
        {
            var newObject = UnityEngine.Object.Instantiate(childPrefab, position, Quaternion.identity);
            //newObject.transform.localScale = scale;

            var newController = newObject.GetComponent<ProjectileController>();
            if (newController)
            {
                newController.procChainMask = projectileController.procChainMask;
                newController.procCoefficient = projectileController.procCoefficient;
                newController.Networkowner = projectileController.Networkowner;
            }

            var teamFilter = newObject.GetComponent<TeamFilter>();
            if (teamFilter)
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

            NetworkServer.Spawn(newObject);
        }
    }
}
