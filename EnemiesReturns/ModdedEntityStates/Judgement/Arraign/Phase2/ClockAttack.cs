﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
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

                if (projectileCountPerRow % 2 == 0)
                {
                    firstProjectileOrigin.transform.localPosition = -Vector3.right * (projectileSize / 2 + projectileSize * (projectileCountPerRow / 2 - 1));
                }
                else
                {
                    firstProjectileOrigin.transform.localPosition = -Vector3.right * (projectileSize * (projectileCountPerRow / 2));
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

                EffectManager.SimpleEffect(effectPrefab, originPoint, Quaternion.LookRotation(targetPoint - originPoint, Vector3.up), true);
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

                if (Vector3.Distance(targetPoint, currentPointGameObject.transform.position) < projectileSize / 2)
                {
                    Finished = true;
                    return;
                }

                if (timer > delayBetweenSpawns)
                {
                    currentPointGameObject.transform.position = Vector3.MoveTowards(currentPointGameObject.transform.position, targetPoint, projectileSize);
                    foreach (Transform projectileOrigin in currentPointGameObject.transform)
                    {
                        var projectileInfo = new FireProjectileInfo()
                        {
                            crit = false,
                            owner = owner,
                            position = projectileOrigin.position,
                            projectilePrefab = projectilePrefab,
                            rotation = projectileOrigin.rotation,
                            damage = damageStat * damageCoefficient,
                            damageTypeOverride = DamageTypeCombo.GenericSecondary
                        };

                        ProjectileManager.instance.FireProjectile(projectileInfo);
                    }
                    timer -= delayBetweenSpawns;
                }
            }

        }

        public static GameObject projectilePrefab;

        public static GameObject effectPrefab;

        public static float delayBetweenSpawns => Configuration.Judgement.ArraignP2.ClockAttackDelayBetweenSpawns.Value;

        public static int projectileCountPerRow = 1;

        public static float projectileSize => Configuration.Judgement.ArraignP2.ClockAttackProjectileDistance.Value;

        public static float damageCoefficient => Configuration.Judgement.ArraignP2.ClockAttackProjectileDamage.Value;

        public static int additionalPairs => Configuration.Judgement.ArraignP2.ClockAttackAdditionalPairs.Value;

        public static int pairsDistanceFromOrigin => Configuration.Judgement.ArraignP2.ClockAttackDistanceFromStart.Value;

        private ClockFiringLine[] lines = Array.Empty<ClockFiringLine>();

        private ChildLocator clockChildLocator;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture", "Thundercall", 0.1f);
            if (isAuthority && projectileCountPerRow == 0)
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

            if (isAuthority && clockChildLocator)
            {
                List<ClockFiringLine> linesList = new List<ClockFiringLine>();
                var staringIndex = UnityEngine.Random.Range(0, clockChildLocator.Count);

                linesList.Add(CreateLine(clockChildLocator, staringIndex));

                for (int i = 0; i < additionalPairs; i++)
                {
                    linesList.Add(CreateLine(clockChildLocator, (staringIndex + pairsDistanceFromOrigin * i + 1) % 12));
                    linesList.Add(CreateLine(clockChildLocator, (12 + (staringIndex - (pairsDistanceFromOrigin * i + 1))) % 12));
                }
                lines = linesList.ToArray();
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
            }
            else
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

            if (isAuthority && lines.Length == 0)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
            if (isAuthority)
            {
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    lines[i] = null;
                }
                lines = null;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
