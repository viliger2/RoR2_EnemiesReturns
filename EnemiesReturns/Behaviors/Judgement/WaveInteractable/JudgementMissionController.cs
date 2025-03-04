using RoR2;
using RoR2.CharacterAI;
using System;
using System.Collections.Generic;
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
        }

        public WavesInformation[] wavesInformation;

        public int maxWaves => wavesInformation.Length;

        public CombatDirector[] combatDirectors;

        public BossGroup[] bossGroups;

        public Inventory inventory;

        public int maximumNumberToSpawnBeforeSkipping = 4;

        public float spawnDistanceMultiplier = 2f;

        public EliteDef eliteDef;

        public int currentRound { private set; get; } = 0;

        private float playerDifficultyCoefficient;

        public static JudgementMissionController instance;

        private bool roundActive;

        private void OnEnable()
        {
            instance = SingletonHelper.Assign(instance, this);
            playerDifficultyCoefficient = 0.7f + Run.instance.participatingPlayerCount * 0.3f;
            // adding hermit crab just to be safe
            var hermitCrab = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/HermitCrab/cscHermitCrab.asset").WaitForCompletion();
            if (ClassicStageInfo.instance.monsterSelection.choices.Where(choice => choice.value != null && choice.value.spawnCard == hermitCrab).ToArray().Length == 0)
            {
                ClassicStageInfo.instance.monsterSelection.AddChoice(
                    new DirectorCard()
                    {
                        spawnCard = hermitCrab,
                        selectionWeight = 1,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Far,
                        preventOverhead = false
                    },
                    1);
            }
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

            if (currentRound >= wavesInformation.Length)
            {
                return;
            }
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
                if (combatDirector.combatSquad)
                {
                    combatDirector.combatSquad.memberHistory.Clear();
                    combatDirector.combatSquad.defeatedServer = false;
                }
            }

            roundActive = true;

            currentRound++;
        }

        [Server]
        public override void OnStartServer()
        {
            base.OnStartServer();
            InitCombatDireactors();
        }

        [Server]
        private void InitCombatDireactors()
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
                combatDirector.eliteBias = 99f; // adding insane elite bias so elites are never chosen, we'll set elites ourselves
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
                ai.onBodyDiscovered -= OnBodyDiscovered;
            }
        }
    }
}
