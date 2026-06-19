using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class ExplicitMonsterCombatSpawner : ShrineBehavior
    {
        public Color shrineEffectColor;

        public int maxPurchaseCount;

        public int baseMonsterCredit;

        public float monsterCreditCoefficientPerPurchase;

        public SpawnCard spawnCard;

        private CombatDirector combatDirector;

        private PurchaseInteraction purchaseInteraction;

        private int purchaseCount;

        private float refreshTimer;

        private const float refreshDuration = 2f;

        private bool waitingForRefresh;

        private float monsterCredit => (float)baseMonsterCredit * Stage.instance.entryDifficultyCoefficient * (1f + (float)purchaseCount * (monsterCreditCoefficientPerPurchase - 1f));

        public static event Action onDefeatedServerGlobal;

        private void Awake()
        {
            if (NetworkServer.active)
            {
                purchaseInteraction = GetComponent<PurchaseInteraction>();
                combatDirector = GetComponent<CombatDirector>();
                combatDirector.combatSquad.onDefeatedServer += CombatSquad_onDefeatedServer;
            }
        }

        private void Start()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!spawnCard)
            {
                purchaseInteraction.SetAvailable(false);
            }
        }


        public void FixedUpdate()
        {
            if (waitingForRefresh)
            {
                refreshTimer -= Time.fixedDeltaTime;
                if (refreshTimer <= 0f && purchaseCount < maxPurchaseCount)
                {
                    purchaseInteraction.SetAvailable(newAvailable: true);
                    waitingForRefresh = false;
                }
            }
        }

        public void AddShrineStack(Interactor interactor)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            waitingForRefresh = true;

            combatDirector.enabled = true;
            combatDirector.monsterCredit += monsterCredit;
            combatDirector.OverrideCurrentMonsterCard(new DirectorCard()
            {
                spawnCard = spawnCard,
                selectionWeight = 1,
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
            });
            combatDirector.monsterSpawnTimer = 0f;

            purchaseCount++;
            refreshTimer = refreshDuration;
            if(purchaseCount >= maxPurchaseCount)
            {
                RpcSetPingable(false);
            }
        }

        private void CombatSquad_onDefeatedServer()
        {
            onDefeatedServerGlobal?.Invoke();
        }
    }
}
