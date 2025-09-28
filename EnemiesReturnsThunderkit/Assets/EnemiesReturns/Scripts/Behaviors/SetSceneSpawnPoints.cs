using RoR2;
using RoR2.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.Navigation.NodeGraph;

namespace EnemiesReturns.Behaviors
{
    public class SetSceneSpawnPoints : MonoBehaviour
    {
        public Transform[] spawnPoints;

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

            for(int i = 0; i < spawnPoints.Length; i++)
            {
                var spawnNode = groundNodes.FindClosestNode(spawnPoints[i].position, HullClassification.Human);
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
