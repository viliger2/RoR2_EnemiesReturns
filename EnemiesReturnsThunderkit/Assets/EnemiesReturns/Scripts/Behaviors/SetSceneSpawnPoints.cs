using RoR2;
using RoR2.Navigation;
using System;
using UnityEngine;
using static RoR2.Navigation.NodeGraph;

namespace EnemiesReturns.Behaviors
{
    public class SetSceneSpawnPoints : MonoBehaviour
    {
        public Transform[] spawnPoints;

        public bool randomizeOrder = true;

        public bool closestNode = true;

        public float minDistance = 10f;

        public float maxDistance = 40f;

        void OnEnable()
        {
            SceneDirector.onPreGeneratePlayerSpawnPointsServer += SceneDirector_onPreGeneratePlayerSpawnPointsServer;
        }

        private void SceneDirector_onPreGeneratePlayerSpawnPointsServer(SceneDirector sceneDirector, ref Action generationMethod)
        {
            generationMethod = GeneratePlayerSpawnPointsServer;
        }

        private void GeneratePlayerSpawnPointsServer()
        {
            if (!SceneInfo.instance)
            {
                return;
            }

            if(spawnPoints.Length == 0)
            {
                return;
            }

            NodeGraph groundNodes = SceneInfo.instance.groundNodes;
            if (!groundNodes)
            {
                return;
            }

            // if (randomizeOrder)
            // {
            //     RoR2Application.rng.Shuffle(spawnPoints);
            // }

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (!spawnPoints[i].gameObject.activeSelf)
                {
                    return;
                }
                NodeIndex spawnNode = NodeIndex.invalid;
                if (closestNode)
                {
                    spawnNode = groundNodes.FindClosestNode(spawnPoints[i].position, HullClassification.Human);
                } else
                {
                    var allNodes = groundNodes.FindNodesInRange(spawnPoints[i].position, minDistance, maxDistance, HullMask.Human);
                    if(allNodes.Count > 0)
                    {
                        spawnNode = allNodes[UnityEngine.Random.Range(0, allNodes.Count)];
                    }
                }
                if(spawnNode == NodeIndex.invalid)
                {
                    continue;
                }

                if(groundNodes.GetNodePosition(spawnNode, out Vector3 position))
                {
                    SpawnPoint.AddSpawnPoint(position, spawnPoints[i].rotation);
                }
            }
        }

    }
}
