using EnemiesReturns.Enemies.LynxTribe.Archer;
using EnemiesReturns.Enemies.LynxTribe.Hunter;
using EnemiesReturns.Enemies.LynxTribe.Scout;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
            var blackListMasters = Configuration.Judgement.Judgement.JudgementEnemyBlacklist.Value.Split(",");
            foreach (var banned in blackListMasters)
            {
                string cleanMasterString = string.Join("", banned.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
                var index = MasterCatalog.FindMasterIndex(cleanMasterString);
                if (index != MasterCatalog.MasterIndex.none)
                {
                    blacklist.Add(MasterCatalog.GetMasterPrefab(index));
                }
            }

            bool needToAddScout = true;
            bool needToAddHunter = true;
            bool needToAddArcher = true;

            foreach (var card in Enemies.Judgement.SetupJudgementPath.mixEnemiesDirectorCards)
            {
                if (stageInfo.monsterSelection.choices.Where(choice => choice.value != null && choice.value.spawnCard != null && choice.value.spawnCard.prefab == card.spawnCard.prefab).Count() == 0)
                {
                    // fixing regi asking for nodes that are not TelepoterOK for some reason
                    if (card.spawnCard.name == "cscRegigigas")
                    {
                        card.spawnCard.forbiddenFlags = RoR2.Navigation.NodeFlags.None;
                    }
                    if (Configuration.LynxTribe.LynxTotem.Enabled.Value)
                    {
                        needToAddScout &= card.spawnCard.name != ScoutBody.SpawnCards.cscLynxScoutDefault.name;
                        needToAddHunter &= card.spawnCard.name != Enemies.LynxTribe.Hunter.HunterBody.SpawnCards.cscLynxHunterDefault.name;
                        needToAddArcher &= card.spawnCard.name != Enemies.LynxTribe.Archer.ArcherBody.SpawnCards.cscLynxArcherDefault.name;
                    }
                    stageInfo.monsterSelection.AddChoice(card, 1);
                }
            }

            if (Configuration.LynxTribe.LynxTotem.Enabled.Value)
            {
                if (needToAddScout)
                {
                    stageInfo.monsterSelection.AddChoice(new DirectorCard
                    {
                        spawnCard = ScoutBody.SpawnCards.cscLynxScoutDefault,
                        selectionWeight = Configuration.LynxTribe.LynxScout.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = Configuration.LynxTribe.LynxScout.MinimumStageCompletion.Value
                    }, 1);
                }
                if (needToAddArcher)
                {
                    stageInfo.monsterSelection.AddChoice(new DirectorCard
                    {
                        spawnCard = ArcherBody.SpawnCards.cscLynxArcherDefault,
                        selectionWeight = Configuration.LynxTribe.LynxArcher.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = Configuration.LynxTribe.LynxArcher.MinimumStageCompletion.Value
                    }, 1);
                }
                if (needToAddHunter)
                {
                    stageInfo.monsterSelection.AddChoice(new DirectorCard
                    {
                        spawnCard = HunterBody.SpawnCards.cscLynxHunterDefault,
                        selectionWeight = Configuration.LynxTribe.LynxHunter.SelectionWeight.Value,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                        preventOverhead = true,
                        minimumStageCompletions = Configuration.LynxTribe.LynxHunter.MinimumStageCompletion.Value
                    }, 1);
                }
            }

            for (int i = stageInfo.monsterSelection.Count - 1; i >= 0; i--)
            {
                if (stageInfo.monsterSelection.choices[i].value != null && stageInfo.monsterSelection.choices[i].value.spawnCard && stageInfo.monsterSelection.choices[i].value.spawnCard.prefab)
                {
                    if (blacklist.Contains(stageInfo.monsterSelection.choices[i].value.spawnCard.prefab))
                    {
                        stageInfo.monsterSelection.RemoveChoice(i);
                    }
                }
            }

#if DEBUG || NOWEAVER
            Log.Info("This list is before DCCS is blended, so if you have mods that modify DCCS blend then they will be applied afterwards.");
            for (int i = stageInfo.monsterSelection.Count - 1; i >= 0; i--)
            {
                if (stageInfo.monsterSelection.choices[i].value != null && stageInfo.monsterSelection.choices[i].value.spawnCard && stageInfo.monsterSelection.choices[i].value.spawnCard.prefab)
                {
                    Log.Info(stageInfo.monsterSelection.choices[i].value.spawnCard);
                }
            }
#endif
        }
    }
}
