using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.DirectorAPI;
using static RoR2.UI.CarouselController;

namespace EnemiesReturns.Behaviors.Judgement
{
    public class AdjustMonsterCards : MonoBehaviour
    {
        public ClassicStageInfo stageInfo;

        public string dccsPoolAddressableAddress;

        private void Awake()
        {
            if (!stageInfo)
            {
                stageInfo = GetComponent<ClassicStageInfo>();
            }

            if (!stageInfo.monsterDccsPool)
            {
                stageInfo.monsterDccsPool = Addressables.LoadAssetAsync<DccsPool>(dccsPoolAddressableAddress).WaitForCompletion();
            }
        }

        private void Start()
        {
            HashSet<GameObject> blacklist = new HashSet<GameObject>();
            var blackListMasters = Configuration.Judgement.JudgementEnemyBlacklist.Value.Split(",");
            foreach (var banned in blackListMasters)
            {
                string cleanMasterString = string.Join("", banned.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                var index = MasterCatalog.FindMasterIndex(cleanMasterString);
                if (index != MasterCatalog.MasterIndex.none)
                {
                    blacklist.Add(MasterCatalog.GetMasterPrefab(index));
                }
            }

            foreach (var card in Enemies.Judgement.SetupJudgementPath.mixEnemiesDirectorCards) 
            {
                if (stageInfo.monsterSelection.choices.Where(choice => choice.value != null && choice.value.spawnCard != null && choice.value.spawnCard.prefab == card.spawnCard.prefab).Count() == 0)
                {
                    stageInfo.monsterSelection.AddChoice(card, 1);
                }
            }

            // adding hermit crab just to be safe
            var hermitCrab = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/HermitCrab/cscHermitCrab.asset").WaitForCompletion();
            if (stageInfo.monsterSelection.choices.Where(choice => choice.value != null && choice.value.spawnCard == hermitCrab).Count() == 0)
            {
                stageInfo.monsterSelection.AddChoice(
                    new DirectorCard()
                    {
                        spawnCard = hermitCrab,
                        selectionWeight = 1,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Far,
                        preventOverhead = false
                    },
                    1);
            }

            for(int i = stageInfo.monsterSelection.Count - 1; i >= 0; i--)
            {
                if (stageInfo.monsterSelection.choices[i].value != null && stageInfo.monsterSelection.choices[i].value.spawnCard && stageInfo.monsterSelection.choices[i].value.spawnCard.prefab)
                {
                    if (blacklist.Contains(stageInfo.monsterSelection.choices[i].value.spawnCard.prefab))
                    {
                        stageInfo.monsterSelection.RemoveChoice(i);
                    }
                }
            }

            for(int i = 0; i < stageInfo.monsterSelection.Count; i++)
            {
                Log.Info(stageInfo.monsterSelection.GetChoice(i).value.spawnCard);
            }
        }
    }
}
