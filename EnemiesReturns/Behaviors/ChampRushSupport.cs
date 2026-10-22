using EnemiesReturns.ScriptableObjects;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors
{
    public static class ChampRushSupport
    {
        public static void Hooks()
        {
            RoR2.Stage.onServerStageBegin += AddChampRushMonsters;
            On.RoR2.ChampRushQuadrantManager.OnEnable += ChampRushQuadrantManager_OnEnable;
        }

        private static void ChampRushQuadrantManager_OnEnable(On.RoR2.ChampRushQuadrantManager.orig_OnEnable orig, ChampRushQuadrantManager self)
        {
            orig(self);

            AddQuadrants(self);
        }

        private static void AddQuadrants(ChampRushQuadrantManager self)
        {
            if (Configuration.General.EnableColossus.Value)
            {
                ChampRushQuadrantEncounter encounter = CreateColossusEncounter();
#if DEBUG || NOWEAVER
                AddEncounter(self.challengeAreas, encounter, 0, 1, 2, 3);
#else
                AddEncounter(self.challengeAreas, encounter, 1, 3);
#endif
            }

            if (Configuration.General.EnableIfrit.Value)
            {
                var encounter = CreateIfritEncounter();

#if DEBUG || NOWEAVER
                AddEncounter(self.challengeAreas, encounter, 0, 1, 2, 3);
#else
                AddEncounter(self.challengeAreas, encounter, 0, 2);
#endif            
            }

            void AddEncounter(List<ChampRushQuadrantManager.QuadrantChallenge> challenges, ChampRushQuadrantEncounter encounter, params int[] whereToAdd)
            {
                for (int i = 0; i < whereToAdd.Length; i++)
                {
                    int challengeIndex = whereToAdd[i];
                    if (challenges.Count <= challengeIndex)
                    {
                        challengeIndex = challenges.Count - 1;
                    }
                    challenges[challengeIndex].encounters.Add(encounter);
                }
            }
        }

        private static ChampRushQuadrantEncounter CreateColossusEncounter()
        {
            var targetHealth = 3500f;

            int itemCount = Mathf.CeilToInt(Mathf.Max((1 - (targetHealth / Configuration.Colossus.BaseMaxHealth.Value)), 0f) / Items.ReduceHP.ReduceHP.healthReductionFraction);

            var encounter = ScriptableObject.CreateInstance<ChampRushQuadrantEncounter>();
            (encounter as ScriptableObject).name = "ER_MisterFister";
            encounter.nameToken = "Stone Fisting, Fun Sized";

            var spawnRequest = new ChampRushSpawnRequest();
            spawnRequest.spawnCard = Enemies.Colossus.ColossusBody.SpawnCards.cscColossusDefault;
            spawnRequest.total = 2;
            spawnRequest.shouldBeStatue = true;
            spawnRequest.startingLoadout = new ChampRushLoadoutPackage()
            {
                noEliteStats = false,
                startingItems = new ItemOrder[]
                {
                    new ItemOrder()
                    {
                        amount = itemCount,
                        item = Content.Items.ReduceHP
                    },
                    new ItemOrder()
                    {
                        amount = Mathf.CeilToInt(itemCount * 2.5f), // reduceHp reduces HP by 5%, scale reduces scale by 1%, we want half the scale for each reduce health, so we multiply item count by 2.5
                        item = DLC4Content.Items.ReduceCharacterScale
                    }
                }
            };

            var spawnRequest2 = new ChampRushSpawnRequest();
            spawnRequest2.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Titan.cscTitanGolemPlains_asset).WaitForCompletion();
            spawnRequest2.total = 1;
            spawnRequest2.shouldBeStatue = false;
            spawnRequest2.startingLoadout = new ChampRushLoadoutPackage();

            var spawnRequest3 = new ChampRushSpawnRequest();
            spawnRequest3.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Golem.cscGolemNature_asset).WaitForCompletion();
            spawnRequest3.total = 6;
            spawnRequest3.shouldBeStatue = false;
            spawnRequest3.startingLoadout = new ChampRushLoadoutPackage();

            var spawnRequest4 = new ChampRushSpawnRequest();
            if (Configuration.General.EnableSwift.Value)
            {
                spawnRequest4.spawnCard = Enemies.Swift.SwiftBody.SpawnCards.cscSwiftDefault;
                spawnRequest4.total = 6;
                spawnRequest4.shouldBeStatue = false;
                spawnRequest4.startingLoadout = new ChampRushLoadoutPackage();
            } else
            {
                spawnRequest4.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_GreaterWisp.cscGreaterWisp_asset).WaitForCompletion();
                spawnRequest4.total = 3;
                spawnRequest4.shouldBeStatue = false;
                spawnRequest4.startingLoadout = new ChampRushLoadoutPackage();
            }

            encounter.spawnRequests = new ChampRushSpawnRequest[] { spawnRequest, spawnRequest2, spawnRequest3, spawnRequest4 };
            return encounter;
        }

        private static ChampRushQuadrantEncounter CreateIfritEncounter()
        {
            var encounter = ScriptableObject.CreateInstance<ChampRushQuadrantEncounter>();
            (encounter as ScriptableObject).name = "ER_Burning";
            encounter.nameToken = "The survivor is burning, the burning of survivor";

            var spawnRequestIfrit = new ChampRushSpawnRequest();
            spawnRequestIfrit.spawnCard = Enemies.Ifrit.IfritBody.SpawnCards.cscIfritDefault;
            spawnRequestIfrit.total = 1;
            spawnRequestIfrit.shouldBeStatue = true;
            spawnRequestIfrit.startingLoadout = new ChampRushLoadoutPackage();

            var spawnRequestBruiser = new ChampRushSpawnRequest();
            spawnRequestBruiser.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_LemurianBruiser.cscLemurianBruiser_asset).WaitForCompletion();
            spawnRequestBruiser.total = 2;
            spawnRequestBruiser.shouldBeStatue = true;
            spawnRequestBruiser.startingLoadout = new ChampRushLoadoutPackage()
            {
                equipment = RoR2Content.Equipment.AffixRed,
                noEliteStats = true
            };

            var spawnRequestLemurian = new ChampRushSpawnRequest();
            spawnRequestLemurian.spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Lemurian.cscLemurian_asset).WaitForCompletion();
            spawnRequestLemurian.total = 6;
            spawnRequestLemurian.shouldBeStatue = true;
            spawnRequestLemurian.startingLoadout = new ChampRushLoadoutPackage()
            {
                equipment = RoR2Content.Equipment.AffixRed,
            };

            var spawnRequests = new ChampRushSpawnRequest[] { spawnRequestIfrit, spawnRequestBruiser, spawnRequestLemurian };

            if (Configuration.General.EnableSpitter.Value)
            {
                var spawnRequestSpitter = new ChampRushSpawnRequest();
                spawnRequestSpitter.spawnCard = Enemies.Spitter.SpitterBody.SpawnCards.cscSpitterDepths;
                spawnRequestSpitter.total = 4;
                spawnRequestSpitter.shouldBeStatue = false;
                spawnRequestSpitter.startingLoadout = new ChampRushLoadoutPackage()
                {
                    equipment = RoR2Content.Equipment.AffixRed,
                };
                HG.ArrayUtils.ArrayAppend(ref spawnRequests, spawnRequestSpitter);
            } else
            {
                spawnRequestLemurian.total = 10;
            }

            if (Configuration.General.EnableSandCrab.Value)
            {
                var spawnRequestCrab = new ChampRushSpawnRequest();

                spawnRequestCrab.spawnCard = Enemies.SandCrab.SandCrabBody.SpawnCards.cscSandCrabSandy;
                spawnRequestCrab.total = 2;
                spawnRequestCrab.shouldBeStatue = false;
                spawnRequestCrab.startingLoadout = new ChampRushLoadoutPackage()
                {
                    equipment = RoR2Content.Equipment.AffixRed,
                    noEliteStats = true
                };

                HG.ArrayUtils.ArrayAppend(ref spawnRequests, spawnRequestCrab);
            }
            else
            {
                spawnRequestBruiser.total = 3;
            }

            encounter.spawnRequests = spawnRequests;


            return encounter;
        }

        // these aren't actually used to spawn waves but we'll keep them just in case, maybe these are used by the anima, who knows
        private static void AddChampRushMonsters(Stage stage)
        {
            if (stage.sceneDef.cachedName != "champrush")
            {
                return;
            }

            var waveGameObject = GameObject.Find("HOLDER: MissionObjects/ChampRushWaveController");
            if (waveGameObject && waveGameObject.TryGetComponent<RoR2.ChampRushWaveController>(out var component))
            {
                if (Configuration.General.EnableColossus.Value)
                {
                    AddColossus(component);
                }
                if (Configuration.General.EnableIfrit.Value)
                {
                    AddIfrit(component);
                }
            }

            // too late
            //var quadrantObject = GameObject.Find("Holder: Quadrant System/QuadrantManager");
            //if(quadrantObject  && quadrantObject.TryGetComponent<RoR2.ChampRushQuadrantManager>(out var manager))
            //{
            //    AddQuadrants(manager);
            //}
        }

        private static void AddColossus(RoR2.ChampRushWaveController component)
        {
            var mainBoss = ScriptableObject.CreateInstance<NormalizedHealthCharacterSpawnCard>().FillFromCharacterSpawnCard(Enemies.Colossus.ColossusBody.SpawnCards.cscColossusGrassy);
            mainBoss.targetHealth = 2800f;

            var additionalBosses = CreateDCCS("dccsChampRushAdditionalBossesColossus", champions: new CharacterSpawnCard[] {
                Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Vagrant.cscVagrant_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ImpBoss.cscImpBoss_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Titan.cscTitanGolemPlains_asset).WaitForCompletion()
            });

            List<CharacterSpawnCard> minibosses = new List<CharacterSpawnCard>();
            minibosses.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Golem.cscGolem_asset).WaitForCompletion());
            if (Configuration.General.EnableSpitter.Value)
            {
                minibosses.Add(Enemies.Spitter.SpitterBody.SpawnCards.cscSpitterDefault);
            }

            List<CharacterSpawnCard> basicMonsters = new List<CharacterSpawnCard>();
            if (Configuration.General.EnableSwift.Value)
            {
                basicMonsters.Add(Enemies.Swift.SwiftBody.SpawnCards.cscSwiftDefault);
            }

            var familyPool = CreateDccsPool("dpChampRushColossus", minibosses: minibosses.ToArray(), basicMonsters: basicMonsters.ToArray());

            component.champRushChampions.Add(new ChampRushChampion()
            {
                mainBoss = mainBoss,
                additionalBosses = additionalBosses,
                familyPool = familyPool
            });
        }

        private static void AddIfrit(RoR2.ChampRushWaveController component)
        {
            var mainBoss = Enemies.Ifrit.IfritBody.SpawnCards.cscIfritDefault;

            var additionalBosses = CreateDCCS("dccsChampRushAdditionalBossesIfrit", champions: new CharacterSpawnCard[] {
                Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Vagrant.cscVagrant_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_ImpBoss.cscImpBoss_asset).WaitForCompletion(),
                Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Titan.cscTitanGolemPlains_asset).WaitForCompletion()
            });

            List<CharacterSpawnCard> minibosses = new List<CharacterSpawnCard>();
            minibosses.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_LemurianBruiser.cscLemurianBruiser_asset).WaitForCompletion());
            if (Configuration.General.EnableSandCrab.Value)
            {
                minibosses.Add(Enemies.SandCrab.SandCrabBody.SpawnCards.cscSandCrabGrassy);
            }

            List<CharacterSpawnCard> basicMonsters = new List<CharacterSpawnCard>();
            basicMonsters.Add(Addressables.LoadAssetAsync<CharacterSpawnCard>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Lemurian.cscLemurian_asset).WaitForCompletion());
            if (Configuration.General.EnableArcherBug.Value)
            {
                basicMonsters.Add(Enemies.ArcherBug.ArcherBugBody.SpawnCards.cscArcherBugDefault);
            }

            var familyPool = CreateDccsPool("dpChampRushIfrit", minibosses: minibosses.ToArray(), basicMonsters: basicMonsters.ToArray());

            component.champRushChampions.Add(new ChampRushChampion()
            {
                mainBoss = mainBoss,
                additionalBosses = additionalBosses,
                familyPool = familyPool
            });
        }

        // we never add dlc selections, so only include monsters that are actually available in vanilla
        private static DccsPool CreateDccsPool(string name, CharacterSpawnCard[] champions = null, CharacterSpawnCard[] minibosses = null, CharacterSpawnCard[] basicMonsters = null)
        {
            var pool = ScriptableObject.CreateInstance<DccsPool>();
            (pool as ScriptableObject).name = "ER_" + name;

            var poolEntry = new DccsPool.PoolEntry();
            poolEntry.dccs = CreateDCCS(name + "Pool", champions, minibosses, basicMonsters);

            var category = new DccsPool.Category();
            category.name = "Standard";
            category.categoryWeight = 1f;
            category.alwaysIncluded = new DccsPool.PoolEntry[] { poolEntry };
            category.includedIfConditionsMet = Array.Empty<DccsPool.ConditionalPoolEntry>();
            category.includedIfNoConditionsMet = Array.Empty<DccsPool.ConditionalPoolEntry>();


            pool.poolCategories = new DccsPool.Category[] { };

            return pool;
        }

        private static DirectorCardCategorySelection CreateDCCS(string name, CharacterSpawnCard[] champions = null, CharacterSpawnCard[] minibosses = null, CharacterSpawnCard[] basicMonsters = null)
        {
            var dccs = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
            (dccs as ScriptableObject).name = "ER_" + name;

            dccs.categories = new DirectorCardCategorySelection.Category[] { 
                CreateCategory(champions, DirectorAPI.MonsterCategory.Champions),
                CreateCategory(minibosses, DirectorAPI.MonsterCategory.Minibosses),
                CreateCategory(basicMonsters, DirectorAPI.MonsterCategory.BasicMonsters),
            };

            return dccs;

            DirectorCardCategorySelection.Category CreateCategory(CharacterSpawnCard[] cards, DirectorAPI.MonsterCategory category)
            {
                DirectorCardCategorySelection.Category result = new DirectorCardCategorySelection.Category();
                result.name = DirectorAPI.Helpers.GetVanillaMonsterCategoryName(category);
                result.selectionWeight = 1f;
                if (cards != null)
                {
                    result.cards = Array.ConvertAll(cards, item => new DirectorCard()
                    {
                        spawnCardReference = new UnityEngine.AddressableAssets.AssetReferenceT<SpawnCard>(""),
                        spawnCard = item,
                        selectionWeight = 1,
                        spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
                    });
                }

                return result;
            }
        }
    }
}
