using RoR2;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class LynxTribeSpawner : MonoBehaviour
    {
        private struct SpawnInfo
        {
            public SpawnCard card;
            public EliteDef elite;
            public float directorCreditsScaled;
            public float eliteCostMultiplier;
        }

        public float eliteBias = 1f;

        public SpawnCard[] spawnCards;

        public float expRewardCoefficient = 0.2f; // that's default value of combat director

        public float goldRewardCoefficient = 1f;

        public int minSpawnCount = 3;

        public int maxSpawnCount = 5;

        public bool assignRewards = false;

        public float spawnDistance = 5f;

        public int retrySpawnCount = 3;

        public NetworkSoundEventDef initialTriggerSound;

        public TeamIndex teamIndex = TeamIndex.Monster;

        private int spawnCount;

        private SpawnInfo[] spawnInfos;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            spawnCount = RoR2Application.rng.RangeInt(minSpawnCount, maxSpawnCount + 1);
            spawnInfos = new SpawnInfo[spawnCount];

            // first pass - we select cards to spawn from array and set their scaled "cost" so we have a pool for elites
            for(int i = 0; i < spawnCount; i++)
            {
                spawnInfos[i].card = spawnCards[RoR2Application.rng.RangeInt(0, spawnCards.Length)];
                spawnInfos[i].directorCreditsScaled = spawnInfos[i].card.directorCreditCost * Stage.instance.entryDifficultyCoefficient;
                spawnInfos[i].eliteCostMultiplier = 1f;
            }

            // second pass - we try to add elites to evert spawn card
            // we do this by checking if we have enough credits to spawn current card as elite,
            // if we don't then we send the remaining credits to the next monster
            // TODO: maybe do one last pass on first monster with remaining credits from all monsters in case first monster is not elite?
            float remainingCredits = 0f;
            for(int i = 0; i < spawnCount; i++)
            {
                float currentRemainingCredits = 0f;
                for(int j = 0; j < RoR2.CombatDirector.eliteTiers.Length; j++)
                {
                    var eliteTierDef = RoR2.CombatDirector.eliteTiers[j];
                    if (!eliteTierDef.CanSelect(spawnInfos[i].card.eliteRules))
                    {
                        continue;
                    }

                    float costWithElites = spawnInfos[i].card.directorCreditCost * eliteTierDef.costMultiplier * eliteBias;
                    if(costWithElites < (spawnInfos[i].directorCreditsScaled + remainingCredits))
                    {
                        spawnInfos[i].elite = eliteTierDef.GetRandomAvailableEliteDef(RoR2Application.rng);
                        spawnInfos[i].eliteCostMultiplier = eliteTierDef.costMultiplier;
                        currentRemainingCredits = (spawnInfos[i].directorCreditsScaled + remainingCredits) - costWithElites;
                    }
                }
                remainingCredits += currentRemainingCredits;
            }
        }

        public void SpawnLynxTribesmen(Transform spawnTransform)
        {
            if (!NetworkServer.active)
            {
                Log.Warning("LynxTribeSpawner::SpawnLynxTribesmen has been called on client!");
                return;
            }

            if(spawnInfos == null)
            {
                Log.Warning("spawnInfos in LynxTribeSpawner is null! Spawning nothing...");
                return;
            }

            float angle = 360f / spawnCount;
            for(int i = 0; i < spawnCount; i++)
            {
                int currentRetrySpawnCount = 0;
                var x = spawnDistance * Mathf.Cos(angle * i * Mathf.Deg2Rad);
                var z = spawnDistance * Mathf.Sin(angle * i * Mathf.Deg2Rad);
                var spawnPosition = new Vector3(spawnTransform.position.x + x, spawnTransform.position.y, spawnTransform.position.z + z);

                if (!Spawn(spawnInfos[i].card, spawnInfos[i].elite, spawnPosition, 0f, 0f, spawnInfos[i].eliteCostMultiplier))
                {
                    currentRetrySpawnCount++;
                    while(currentRetrySpawnCount < retrySpawnCount)
                    {
                        if (Spawn(spawnInfos[i].card, spawnInfos[i].elite, spawnPosition, 0f, currentRetrySpawnCount * 10f, spawnInfos[i].eliteCostMultiplier))
                        {
                            break;
                        }
                        currentRetrySpawnCount++;
                    }
                }
            }
        }

        private bool Spawn(SpawnCard card, EliteDef eliteDef, Vector3 position, float minDistance, float maxDistance, float eliteCostMultiplier)
        {
            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(card, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                position = position,
                preventOverhead = true,
                minDistance = minDistance,
                maxDistance = maxDistance
            }, RoR2Application.rng);
            directorSpawnRequest.ignoreTeamMemberLimit = true;
            directorSpawnRequest.onSpawnedServer = OnCardSpawned;
            directorSpawnRequest.teamIndexOverride = teamIndex;
            if (!DirectorCore.instance.TrySpawnObject(directorSpawnRequest))
            {
                Log.Info($"LynxTribeSpawner failed to spawn {card} at {position}.");
                return false;
            }

            return true;

            void OnCardSpawned(SpawnCard.SpawnResult result)
            {
                if (result.success)
                {
                    CharacterMaster master = result.spawnedInstance.GetComponent<CharacterMaster>();
                    GameObject bodyObject = master.GetBodyObject();
                    CharacterBody characterBody = bodyObject.GetComponent<CharacterBody>();
                    if (characterBody)
                    {
                        characterBody.cost = card.directorCreditCost * eliteCostMultiplier;
                    }
                    var healthBoost = eliteDef?.healthBoostCoefficient ?? 1f;
                    var damageBoost = eliteDef?.damageBoostCoefficient ?? 1f;
                    EquipmentIndex equipmentIndex = eliteDef?.eliteEquipmentDef?.equipmentIndex ?? EquipmentIndex.None;
                    if (equipmentIndex != EquipmentIndex.None)
                    {
                        master.inventory.SetEquipmentIndex(equipmentIndex);
                    }
                    master.inventory.GiveItem(RoR2Content.Items.BoostHp, Mathf.RoundToInt((healthBoost - 1f) * 10f));
                    master.inventory.GiveItem(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((damageBoost - 1f) * 10f));
                    if (assignRewards && bodyObject.TryGetComponent<DeathRewards>(out var deathRewards))
                    {
                        float num3 = card.directorCreditCost * eliteCostMultiplier * expRewardCoefficient;
                        deathRewards.spawnValue = (int)Mathf.Max(1f, num3);
                        if (num3 > Mathf.Epsilon)
                        {
                            deathRewards.expReward = (uint)Mathf.Max(1f, num3 * Run.instance.compensatedDifficultyCoefficient);
                            deathRewards.goldReward = (uint)Mathf.Max(1f, num3 * goldRewardCoefficient * 2f * Run.instance.compensatedDifficultyCoefficient); // 2 is magic number from combat director
                        }
                        else
                        {
                            deathRewards.expReward = 0u;
                            deathRewards.goldReward = 0u;
                        }
                    }
                }
            }
        }
    }
}
