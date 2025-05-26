using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.WaveInteractable
{
    public class JudgementMissionController : NetworkBehaviour
    {
        [Serializable]
        public struct WavesInformation
        {
            public int minCreditCost;
            public int maxCreditCost;
            public float totalAvailableCredits;
            public uint maxSquadCount;
        }

        public WavesInformation[] wavesInformation;

        public static float rewardMultiplier = 6f; // TODO: config

        public int maxWaves => wavesInformation.Length;

        public CombatDirector[] combatDirectors;

        public BossGroup[] bossGroups;

        public Inventory inventory;

        public int maximumNumberToSpawnBeforeSkipping = 4;

        public float spawnDistanceMultiplier = 2f;

        public EliteDef eliteDef;

        public bool missionClear { private set; get; }

        public int currentRound { private set; get; } = 0;

        private float playerDifficultyCoefficient;

        public static JudgementMissionController instance;

        private bool roundActive;

        private void Start()
        {
            if (NetworkServer.active)
            {
                InitCombatDirectors();
            }
        }

        private void OnEnable()
        {
            instance = SingletonHelper.Assign(instance, this);
            playerDifficultyCoefficient = 0.7f + Run.instance.participatingPlayerCount * 0.3f;
        }

        private void OnDisable()
        {
            instance = SingletonHelper.Unassign(instance, this);
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (roundActive)
            {
                var endRound = true;
                for (int i = 0; i < combatDirectors.Length; i++)
                {
                    var director = combatDirectors[i];
                    endRound = endRound && director.combatSquad.defeatedServer;
                }
                if (endRound)
                {
                    EndRound();
                    roundActive = false;
                }
            }
        }

        [Server]
        public void EndRound()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            missionClear = currentRound >= wavesInformation.Length;
        }

        [Server]
        public void BeginRound()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            for (int i = 0; i < bossGroups.Length; i++)
            {
                var bossGroup = bossGroups[i];
                bossGroup.bestObservedName = "";
                bossGroup.bestObservedSubtitle = "";
                bossGroup.bossMemoryCount = 0;
            }

            RpcClientBeginRound();

            if (currentRound > wavesInformation.Length - 1)
            {
                currentRound = wavesInformation.Length - 1;
            }

            var information = wavesInformation[currentRound];

            WeightedSelection<DirectorCard> cardSelection = new WeightedSelection<DirectorCard>();

            var monsterSelection = ClassicStageInfo.instance.monsterSelection;
            for (int i = 0; i < monsterSelection.Count; i++)
            {
                var card = monsterSelection.choices[i].value;
                if (card == null) 
                {
                    continue;
                }

                if (card.cost >= information.minCreditCost && card.cost <= information.maxCreditCost)
                {
                    cardSelection.AddChoice(card, 1);
                }
            }

            // ensuring variety
            List<DirectorCard> selectedCard = new List<DirectorCard>();

            for (int i = 0; i < combatDirectors.Length; i++)
            {
                var combatDirector = combatDirectors[i];
                combatDirector.monsterCredit += information.totalAvailableCredits * playerDifficultyCoefficient;
                var card = cardSelection.Evaluate(RoR2Application.rng.nextNormalizedFloat);
                // failsafe in case of monster count being less than director count, like in cases of not having dlcs
                if (cardSelection.Count >= combatDirectors.Length)
                {
                    while (selectedCard.Contains(card))
                    {
                        card = cardSelection.Evaluate(RoR2Application.rng.nextNormalizedFloat);
                    }
                    selectedCard.Add(card);
                }
                combatDirector.OverrideCurrentMonsterCard(card);
                combatDirector.monsterSpawnTimer = 0f;
                combatDirector.gameObject.SetActive(true);
                combatDirector.maxSquadCount = information.maxSquadCount;
                if (combatDirector.combatSquad)
                {
                    combatDirector.combatSquad.memberHistory.Clear();
                    combatDirector.combatSquad.defeatedServer = false;
                }
            }

            // dropping warbanners
            if (currentRound == 0)
            {
                ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
                for (int j = 0; j < teamMembers.Count; j++)
                {
                    TeamComponent teamComponent = teamMembers[j];
                    CharacterBody body = teamComponent.body;
                    if (!body)
                    {
                        continue;
                    }
                    CharacterMaster master = teamComponent.body.master;
                    if ((bool)master)
                    {
                        int itemCount = master.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                        if (itemCount > 0)
                        {
                            GameObject obj = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard"), body.transform.position, Quaternion.identity);
                            obj.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
                            obj.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount;
                            NetworkServer.Spawn(obj);
                        }
                    }
                }
            }

            roundActive = true;

            currentRound++;
        }

        private void InitCombatDirectors()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            for (int i = 0; i < combatDirectors.Length; i++)
            {
                var combatDirector = combatDirectors[i];
                combatDirector.maximumNumberToSpawnBeforeSkipping = maximumNumberToSpawnBeforeSkipping;
                combatDirector.spawnDistanceMultiplier = spawnDistanceMultiplier;
                combatDirector.eliteBias = 999f; // adding insane elite bias so elites are never chosen, we'll set elites ourselves
                combatDirector.onSpawnedServer.AddListener(ModifySpawnedMonsters);
            }
        }

        [ClientRpc]
        private void RpcClientBeginRound()
        {
            for (int i = 0; i < bossGroups.Length; i++)
            {
                var bossGroup = bossGroups[i];
                bossGroup.bestObservedName = "";
                bossGroup.bestObservedSubtitle = "";
                bossGroup.bossMemoryCount = 0;
            }
        }

        public void ModifySpawnedMonsters(GameObject spawnedMonster)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            CharacterMaster component = spawnedMonster.GetComponent<CharacterMaster>();
            BaseAI ai = component.GetComponent<BaseAI>();
            if ((bool)ai)
            {
                ai.onBodyDiscovered += OnBodyDiscovered;
            }

            component.inventory.AddItemsFrom(inventory);

            var healthBoost = eliteDef?.healthBoostCoefficient ?? 1f;
            var damageBoost = eliteDef?.damageBoostCoefficient ?? 1f;

            var equipmentIndex = eliteDef?.eliteEquipmentDef?.equipmentIndex ?? EquipmentIndex.None;
            if (equipmentIndex != EquipmentIndex.None)
            {
                component.inventory.SetEquipmentIndex(equipmentIndex);
            }

            component.inventory.GiveItem(RoR2Content.Items.BoostHp, Mathf.RoundToInt((healthBoost - 1) * 10f));
            component.inventory.GiveItem(RoR2Content.Items.BoostDamage, Mathf.RoundToInt((damageBoost - 1) * 10f));

            void OnBodyDiscovered(CharacterBody newBody)
            {
                ai.ForceAcquireNearestEnemyIfNoCurrentEnemy();

                if (newBody.gameObject.TryGetComponent<DeathRewards>(out var deathRewards))
                {
                    float num3 = newBody.cost * rewardMultiplier * 0.2f;
                    deathRewards.spawnValue = (int)Mathf.Max(1f, num3);
                    if (num3 > Mathf.Epsilon)
                    {
                        deathRewards.expReward = (uint)Mathf.Max(1f, num3 * Run.instance.compensatedDifficultyCoefficient);
                        deathRewards.goldReward = (uint)Mathf.Max(1f, num3 * 2f * Run.instance.compensatedDifficultyCoefficient); // 2 is magic number from combat director
                    }
                    else
                    {
                        deathRewards.expReward = 0u;
                        deathRewards.goldReward = 0u;
                    }
                }

                ai.onBodyDiscovered -= OnBodyDiscovered;
            }
        }
    }
}
