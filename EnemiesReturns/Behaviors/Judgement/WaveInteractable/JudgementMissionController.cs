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
                Log.Info($"Selected card: {card.spawnCard.name}");
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
    }
}
