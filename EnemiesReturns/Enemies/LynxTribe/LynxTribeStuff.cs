using EnemiesReturns.Behaviors;
using EnemiesReturns.ModCompats.PrefabAPICompat;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe
{
    public class LynxTribeStuff
    {
        // TODO: config this shit
        public GameObject CreateTrapPrefab(GameObject trapPrefab)
        {
            trapPrefab.AddComponent<NetworkIdentity>();

            var disabler = trapPrefab.AddComponent<DestroyOnTimerNetwork>();
            disabler.duration = 15f;
            disabler.enabled = false;

            var spawner = trapPrefab.AddComponent<LynxTribeSpawner>();
            spawner.eliteBias = 1f;
            spawner.spawnCards = new RoR2.SpawnCard[]
            {
                Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Lemurian/cscLemurian.asset").WaitForCompletion() // TODO: replace with lynx tribe cards
            };
            spawner.minSpawnCount = 3;
            spawner.maxSpawnCount = 5;
            spawner.assignRewards = true;
            spawner.spawnDistance = 5f;
            spawner.retrySpawnCount = 3;
            spawner.teamIndex = TeamIndex.Monster;

            var trap = trapPrefab.AddComponent<LynxTribeTrap>(); // TODO: add sound effects
            trap.checkInterval = 0.25f;
            trap.spawnAfterTriggerInterval = 0.5f;
            trap.spawner = spawner;
            trap.destroyOnTimer = disabler;
            trap.hitBox = trapPrefab.transform.Find("Hitbox");
            var teamMask = new TeamMask();
            teamMask.AddTeam(TeamIndex.Player);
            trap.teamFilter = teamMask;

            trapPrefab.RegisterNetworkPrefab();

            return trapPrefab;
        }

    }
}
