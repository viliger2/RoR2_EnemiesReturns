//using EnemiesReturns.Components;
using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    public class SummonSkyLasers : BaseState
    {
        public static float baseDuration = 6.8f;

        public static CharacterSpawnCard cscSkyLaser;

        public static int baseLaserCount = 3;

        public static float additionalLaserPerPlayer = 0.5f;

        private static float minDistance = 30f;

        private static float maxDistance = 70f;

        private int totalLaserCount;

        private ChildLocator laserPointLocator;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;

            PlayCrossfade("Gesture, Override", "SummonSkyLaser", "SkyLaser.playbackRate", duration, 0.1f);

            // var bodies = Utils.GetActiveAndAlivePlayerBodies();
            // if(bodies.Count == 0)
            // {
            //     outer.SetNextStateToMain();
            //     return;
            // }

            //totalLaserCount = baseLaserCount + (int)Math.Round(additionalLaserPerPlayer * (bodies.Count - 1), MidpointRounding.ToEven);

            var sceneChildLocator = SceneInfo.instance.gameObject.GetComponent<ChildLocator>();
            if (sceneChildLocator)
            {
                var laserPoints = sceneChildLocator.FindChild("LaserSpawnPoints");
                if (laserPoints)
                {
                    laserPointLocator = laserPoints.gameObject.GetComponent<ChildLocator>();
                }
            }

            if (laserPointLocator)
            {
                var staringIndex = UnityEngine.Random.Range(0, laserPointLocator.Count);
                for (int i = 0; i < totalLaserCount; i++)
                {
                    SummonSkyLaser(laserPointLocator.FindChild((staringIndex + i) % laserPointLocator.Count));
                }
            } else
            {
                for (int i = 0; i < totalLaserCount; i++)
                {
                    SummonSkyLaser(null);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(isAuthority && fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        private GameObject SummonSkyLaser(Transform transform)
        {
            var placementRule = new DirectorPlacementRule();
            if (transform)
            {
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.Direct;
                placementRule.spawnOnTarget = transform;
            } else
            {
                placementRule.placementMode = DirectorPlacementRule.PlacementMode.Approximate;
                placementRule.minDistance = minDistance;
                placementRule.maxDistance = maxDistance;
                placementRule.spawnOnTarget = this.transform;
            }

            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(cscSkyLaser, placementRule, RoR2Application.rng);
            directorSpawnRequest.summonerBodyObject = gameObject;
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = (spawnResult) =>
            {
                if (spawnResult.success && spawnResult.spawnedInstance && characterBody)
                {
                    var aiownership = spawnResult.spawnedInstance.GetComponent<AIOwnership>();
                    if (aiownership)
                    {
                        aiownership.ownerMaster = characterBody.master;
                    }

                    var baseAI = spawnResult.spawnedInstance.GetComponent<BaseAI>();
                    if (baseAI && baseAI.body)
                    {
                        baseAI.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                    }
                }
            };

            return DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
        }

    }
}
