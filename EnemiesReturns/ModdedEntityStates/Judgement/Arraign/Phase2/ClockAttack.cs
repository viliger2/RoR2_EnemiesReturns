using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    public class ClockAttack : BaseState
    {
        private class ClockFiringLine
        {
            private Vector3 targetPoint;

            public GameObject owner;

            public float damageStat;

            public bool Finished { get; private set; }

            private float timer;

            private GameObject currentPointGameObject;

            public ClockFiringLine(Vector3 originPoint, Vector3 targetPoint)
            {
                this.targetPoint = targetPoint;

                currentPointGameObject = new GameObject();
                currentPointGameObject.name = "ClockFiringLine";
                currentPointGameObject.transform.position = originPoint;
                currentPointGameObject.transform.LookAt(this.targetPoint);

                var firstProjectileOrigin = new GameObject();
                firstProjectileOrigin.name = "Projectile0";
                firstProjectileOrigin.transform.parent = currentPointGameObject.transform;
                firstProjectileOrigin.transform.localPosition = Vector3.zero;
                firstProjectileOrigin.transform.localRotation = Quaternion.identity;

                if(projectileCountPerRow % 2 == 0)
                {
                    firstProjectileOrigin.transform.localPosition = -Vector3.right * (projectileSize / 2 + projectileSize * (projectileCountPerRow / 2 - 1));
                } else
                {
                    firstProjectileOrigin.transform.localPosition = -Vector3.right * (projectileSize * (projectileCountPerRow / 2 - 1));
                }
                var position = firstProjectileOrigin.transform.localPosition;
                for (int i = 1; i < projectileCountPerRow; i++)
                {
                    position += Vector3.right * projectileSize;
                    var projectileOrigin = new GameObject();
                    projectileOrigin.name = "Projectile" + i;
                    projectileOrigin.transform.parent = currentPointGameObject.transform;
                    projectileOrigin.transform.localPosition = position;
                    projectileOrigin.transform.localRotation = Quaternion.identity;
                }
            }

            ~ClockFiringLine()
            {
                if (currentPointGameObject)
                {
                    UnityEngine.Object.Destroy(currentPointGameObject);
                }
            }

            public void FixedUpdate(float deltaTime)
            {
                timer += deltaTime;

                if(Vector3.Distance(targetPoint, currentPointGameObject.transform.position) < projectileSize / 2)
                {
                    Finished = true;
                    return;
                }

                if(timer > delayBetweenSpawns)
                {
                    currentPointGameObject.transform.position = Vector3.MoveTowards(currentPointGameObject.transform.position, targetPoint, projectileSize);
                    foreach(Transform projectileOrigin in currentPointGameObject.transform)
                    {
                        var projectileInfo = new FireProjectileInfo()
                        {
                            crit = false,
                            owner = owner,
                            position = projectileOrigin.position,
                            projectilePrefab = projectilePrefab,
                            rotation = Quaternion.identity,
                            damage = damageStat * damageCoefficient
                        };

                        ProjectileManager.instance.FireProjectile(projectileInfo);
                    }
                    timer -= delayBetweenSpawns;
                }
            }

        }

        public static GameObject projectilePrefab;

        public static float delayBetweenSpawns = 0.1f;

        public static int projectileCountPerRow = 4;

        public static float projectileSize = 7f;

        public static float damageCoefficient = 3f;

        public static int additionalPairs = 1;

        public static int pairsDistanceFromOrigin = 2;

        private ClockFiringLine[] lines = Array.Empty<ClockFiringLine>();

        private ChildLocator clockChildLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            if(isAuthority && projectileCountPerRow == 0)
            {
                outer.SetNextStateToMain();
            }

            var sceneChildLocator = SceneInfo.instance.gameObject.GetComponent<ChildLocator>();
            if (sceneChildLocator)
            {
                var clock = sceneChildLocator.FindChild("Clock");
                if (clock)
                {
                    clockChildLocator = clock.gameObject.GetComponent<ChildLocator>();
                }
            }

            if (isAuthority)
            {
                var staringIndex = UnityEngine.Random.Range(0, clockChildLocator.Count);

                HG.ArrayUtils.ArrayAppend(ref lines, CreateLine(clockChildLocator, staringIndex));

                for(int i = 0; i < additionalPairs; i++)
                {
                    HG.ArrayUtils.ArrayAppend(ref lines, CreateLine(clockChildLocator, (staringIndex + pairsDistanceFromOrigin * i + 1) % 12));
                    HG.ArrayUtils.ArrayAppend(ref lines, CreateLine(clockChildLocator, (12 + (staringIndex - (pairsDistanceFromOrigin * i + 1))) % 12));
                }
            }
        }

        private ClockFiringLine CreateLine(ChildLocator childLocator, int childIndex)
        {
            Vector3 originPoint = base.transform.position;
            if (childLocator)
            {
                var child = childLocator.FindChild(childIndex);
                if (child)
                {
                    originPoint = child.transform.position;
                }
            }

            Vector3 targetPoint = Vector3.zero;
            if (childLocator)
            {
                var child = childLocator.FindChild((childIndex + 6) % 12);
                if (child)
                {
                    targetPoint = child.transform.position;
                }
            } else
            {
                var forward = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up) * Vector3.ProjectOnPlane(originPoint, Vector3.up).normalized;
                targetPoint = base.transform.position + forward * 1000f;
            }

            var firingLine = new ClockFiringLine(originPoint, targetPoint)
            {
                damageStat = damageStat,
                owner = base.gameObject
            };

            return firingLine;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                bool finished = true;
                foreach (var firingLine in lines)
                {
                    firingLine.FixedUpdate(GetDeltaTime());
                    finished = finished && firingLine.Finished;
                }

                if (finished)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            for(int i = lines.Length - 1; i >= 0; i--)
            {
                lines[i] = null;
            }
            lines = null;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
