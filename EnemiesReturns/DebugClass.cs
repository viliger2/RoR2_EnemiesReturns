using EnemiesReturns.Quests;
using RoR2;
using RoR2.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace EnemiesReturns
{
#if DEBUG || NOWEAVER
    public class DebugClass
    {
        public static void DebugHooks()
        {
            //On.RoR2.Quests.QuestRunner.OnDeserialize += QuestRunner_OnDeserialize;
            //On.RoR2.Quests.QuestRunner.OnSerialize += QuestRunner_OnSerialize;
            //On.RoR2.RoR2Application.OnLoad += RoR2Application_OnLoad;
            FunTimeWithWhatevers();
        }

        private static bool QuestRunner_OnSerialize(On.RoR2.Quests.QuestRunner.orig_OnSerialize orig, QuestRunner self, UnityEngine.Networking.NetworkWriter writer, bool initialState)
        {
            //System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
            //Log.Info(t.ToString());
            Log.Info("started quest serialization");
            uint num = self.syncVarDirtyBits;
            if (initialState)
            {
                num = 7u;
            }
            bool num2 = (num & 1) != 0;
            bool flag = (num & 2) != 0;
            bool flag2 = (num & 4) != 0;
            writer.Write((byte)num);
            if (num2)
            {
                Log.Info("we are at num2, sending quest index");
                writer.Write(self.quest?.questIndex ?? QuestIndex.None);
                Log.Info("quest index " + (int)(self.quest?.questIndex ?? QuestIndex.None));
            }
            if (flag)
            {
                Log.Info("flag, serializing quest");
                self.quest?.OnSerialize(writer);
            }
            if (flag2)
            {
                Log.Info("flag2, next quest info");
                bool flag3 = self.hasNextQuest;
                Log.Info("has next quest " + flag3);
                writer.Write(flag3);
                if (flag3)
                {
                    Log.Info("questCompletedSuccesfully " + self.questCompletedSuccesfully);
                    writer.Write(self.questCompletedSuccesfully);
                    Log.Info("quest index " + (int)(self.nextQuest?.questIndex ?? QuestIndex.None));
                    writer.Write(self.nextQuest?.questIndex ?? QuestIndex.None);
                    Log.Info("serializing quest");
                    self.nextQuest?.OnSerialize(writer);
                }
            }
            Log.Info("ended quest serialization");
            if (!initialState)
            {
                return num != 0;
            }
            return false;
        }

        private static System.Collections.IEnumerator RoR2Application_OnLoad(On.RoR2.RoR2Application.orig_OnLoad orig, RoR2Application self)
        {
            yield return orig(self);
            Log.Info("quest index" + QuestCatalog.FindQuestIndex(typeof(CompulsionQuestSurviveIfritPillars)));
        }

        private static void QuestRunner_OnDeserialize(On.RoR2.Quests.QuestRunner.orig_OnDeserialize orig, RoR2.Quests.QuestRunner self, UnityEngine.Networking.NetworkReader reader, bool initialState)
        {
            Log.Info("started deserializing quest");
            byte num = reader.ReadByte();
            bool flag = (num & 1) != 0; // 1 is quest
            bool flag2 = (num & 2) != 0; // 2 is quest data
            bool num2 = (num & 4) != 0; // 4 is next quest
            if (flag)
            {
                Log.Info("flag, has quest, does quest.onexit and new quest instance");
                self.quest?.OnExit();
                self.quest = QuestCatalog.CreateQuestInstance(reader.ReadQuestIndex());
                if (self.quest != null)
                {
                    Log.Info("quest is not null, setting the runner");
                    self.quest.runner = self;
                }
            }
            if (flag2)
            {
                Log.Info("flag2, has quest data, does deserialization and on enter");
                self.quest?.OnDeserialize(reader);
                if (flag)
                {
                    Log.Info("we also have quest so we enter it");
                    self.quest?.OnEnter();
                }
            }
            if (num2 && reader.ReadBoolean())
            {
                Log.Info("num2, has next quest or something");
                self.hasNextQuest = true;
                self.hasChangedQuest = false;
                self.questCompletedSuccesfully = reader.ReadBoolean();
                self.nextQuest = QuestCatalog.CreateQuestInstance(reader.ReadQuestIndex());
                self.nextQuest?.OnDeserialize(reader);
            }
            Log.Info("ended deserializing quest");

        }

        private static void FunTimeWithWhatevers()
        {
            var locator = Addressables.ResourceLocators.FirstOrDefault(l => l.LocatorId == "AddressablesMainContentCatalog") as ResourceLocationMap;

            if (locator == null)
            {
                Log.Error("Couldn't find game's locator");
                return;
            }

            var guidRegex = new Regex("^[0-9a-f]{32}$", RegexOptions.Compiled);

            foreach (var (key, locations) in locator.Locations)
            {

                if(!(key is string guid))
                {
                    continue;
                }

                if(guid.Length != 32 || !guidRegex.IsMatch(guid))
                {
                    continue;
                }

                foreach (var location in locations)
                {
                    var type = typeof(ChampRushQuadrantEncounter);

                    if(location.ResourceType == type)
                    {
                        Log.Info(guid);
                        var encounter = Addressables.LoadAssetAsync<ChampRushQuadrantEncounter>(guid).WaitForCompletion();
                        PrintEncounterInfo(encounter);
                    }
                }
            }
        }

        private static void PrintEncounterInfo(ChampRushQuadrantEncounter encounter)
        {
            //Log.Info("Begin Encounter Info " + encounter.name);
            Log.Info("Encounter name: " + encounter.nameToken);
            //Log.Info("Spawn Requests: ");
            Log.Info("Card\tHealth\tCount\tisElite\teliteStats\thasItems\tTotalHealthWithCountAndStats");
            var allSpawnsHealth = 0f;
            var allSpawnsHealthWithElite = 0f;
            foreach(var spawnRequest in encounter.spawnRequests)
            {
                var health = spawnRequest.spawnCard.prefab.GetComponent<CharacterMaster>().bodyPrefab.GetComponent<CharacterBody>().baseMaxHealth;
                var totalHealth = health * spawnRequest.total;
                var totalHealthWithElite = totalHealth;
                var eliteStats = 0f;
                if (spawnRequest.startingLoadout.equipment && !spawnRequest.startingLoadout.noEliteStats)
                {
                    eliteStats = spawnRequest.startingLoadout.equipment.passiveBuffDef.eliteDef.healthBoostCoefficient;
                    totalHealthWithElite *= eliteStats;
                }

                Log.Info($"{spawnRequest.spawnCard}\t{health}\t{spawnRequest.total}\t{(bool)spawnRequest.startingLoadout.equipment}\t{eliteStats}\t{spawnRequest.startingLoadout.startingItems.Length > 0}\t{totalHealthWithElite}");

                allSpawnsHealth += totalHealth;
                allSpawnsHealthWithElite += totalHealthWithElite;
            }
            Log.Info($"Total Health: {allSpawnsHealth}, Total Health With Elite Bonuses: {allSpawnsHealthWithElite}");
            //Log.Info("End Encounter Info " + encounter.name);
        }
    }
#endif
}
