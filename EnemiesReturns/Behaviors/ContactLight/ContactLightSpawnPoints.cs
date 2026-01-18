using RoR2;
using RoR2.Navigation;
using System;
using UnityEngine;

namespace EnemiesReturns.Behaviors.ContactLight
{
    public class ContactLightSpawnPoints : MonoBehaviour
    {
        public Transform[] searchOrigins;

        public float minDistance = 10f;

        public float maxDistance = 40f;

        void OnEnable()
        {
            SceneDirector.onPreGeneratePlayerSpawnPointsServer += SceneDirector_onPreGeneratePlayerSpawnPointsServer;
        }

        private void OnDisable()
        {
            SceneDirector.onPreGeneratePlayerSpawnPointsServer -= SceneDirector_onPreGeneratePlayerSpawnPointsServer;
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

            if (searchOrigins.Length == 0)
            {
                return;
            }

            NodeGraph groundNodes = SceneInfo.instance.groundNodes;
            if (!groundNodes)
            {
                return;
            }

            var origin = searchOrigins[RoR2.Run.instance.runRNG.RangeInt(0, searchOrigins.Length)];
            ContactLightGateSetter.instance.SetSpawnPointGate(origin.name);
            var allNodes = groundNodes.FindNodesInRange(origin.position, minDistance, maxDistance, HullMask.Human);

            for (int i = 0; i < 16; i++)
            {
                var spawnNode = allNodes[RoR2.Run.instance.runRNG.RangeInt(0, allNodes.Count)];
                if (groundNodes.GetNodePosition(spawnNode, out Vector3 position))
                {
                    SpawnPoint.AddSpawnPoint(position, Quaternion.identity);
                }
                allNodes.Remove(spawnNode);
            }
        }
    }
}
