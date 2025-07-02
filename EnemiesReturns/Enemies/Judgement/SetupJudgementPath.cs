using EnemiesReturns.Behaviors.Judgement.MithrixWeaponDrop;
using EnemiesReturns.Enemies.Judgement.Arraign;
using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.UI;
using RoR2BepInExPack.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Judgement
{
    public static class SetupJudgementPath
    {
        public static GameObject PileOfDirt;

        public static GameObject BrokenTeleporter;

        public static GameObject JudgementInteractable;

        public static MasterCatalog.MasterIndex ArraignP1MasterIndex;

        public static MasterCatalog.MasterIndex ArraignP2MasterIndex;

        public static BodyIndex ArraignP1BodyIndex;

        public static BodyIndex ArraignP2BodyIndex;

        public static Material immuneToAllDamageExceptHammerMaterial;

        public static List<DirectorCard> mixEnemiesDirectorCards = new List<DirectorCard>();

        [SystemInitializer(new Type[] { typeof(MasterCatalog), typeof(BodyCatalog) })]
        private static void Init()
        {
            if (!EnemiesReturns.EnemiesReturnsPlugin.ModIsLoaded)
            {
                return;
            }

            if (!EnemiesReturns.Configuration.Judgement.Judgement.Enabled.Value)
            {
                return;
            }

            ArraignP1MasterIndex = MasterCatalog.FindMasterIndex("ArraignP1Master");
            ArraignP2MasterIndex = MasterCatalog.FindMasterIndex("ArraignP2Master");

            ArraignP1BodyIndex = BodyCatalog.FindBodyIndex("ArraignP1Body");
            ArraignP2BodyIndex = BodyCatalog.FindBodyIndex("ArraignP2Body");

            AddAeonianAnointedItemDisplays();

            ArraignDamageController.AddBodyToArmorBypass(BodyCatalog.FindBodyIndex("BrotherBody"));
            ArraignDamageController.AddBodyToArmorBypass(BodyCatalog.FindBodyIndex("BrotherHurtBody"));
        }

        private static void AddAeonianAnointedItemDisplays()
        {
            var keyEquipmentReference = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteIce.EliteIceEquipment);
            var equipment = Addressables.LoadAssetAsync<EquipmentDef>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_EliteIce.EliteIceEquipment_asset).WaitForCompletion();
            var dictionary = CreateAeonianAnointedDictionary();
            foreach (var body in BodyCatalog.allBodyPrefabs)
            {
                var modelLocator = body.GetComponent<ModelLocator>();
                if (!modelLocator) continue;

                if (!modelLocator.modelTransform) continue;

                var characterModel = modelLocator.modelTransform.GetComponent<CharacterModel>();
                if (!characterModel) continue;

                var bodyIDRS = characterModel.itemDisplayRuleSet;
                if (!bodyIDRS) continue;

                var existingidrs = bodyIDRS.keyAssetRuleGroups.Where(item => item.keyAsset == Content.Equipment.EliteAeonian || item.keyAsset == Content.Items.HiddenAnointed).ToArray();
                if (existingidrs.Length > 0)
                {
                    continue;
                }

                if (dictionary.TryGetValue(body.name, out var value))
                {
                    ArrayUtils.ArrayAppend(ref bodyIDRS.keyAssetRuleGroups, new KeyAssetRuleGroup
                    {
                        displayRuleGroup = value,
                        keyAsset = Content.Equipment.EliteAeonian
                    });

                    ArrayUtils.ArrayAppend(ref bodyIDRS.keyAssetRuleGroups, new KeyAssetRuleGroup
                    {
                        displayRuleGroup = value,
                        keyAsset = Content.Items.HiddenAnointed
                    });

                    continue;
                }

                DisplayRuleGroup displayRuleGroup = default;

                var result = bodyIDRS.keyAssetRuleGroups.Where(item => item.keyAssetAddress == keyEquipmentReference || item.keyAsset == equipment).ToArray();
                if (result.Length != 0)
                {
                    displayRuleGroup = result[0].displayRuleGroup;
                }
                else
                {
                    var result2 = bodyIDRS.namedEquipmentRuleGroups.Where(item => item.name == "EliteIceEquipment").ToArray();
                    if (result2.Length != 0)
                    {
                        displayRuleGroup = result2[0].displayRuleGroup;
                    }
                }

                if (displayRuleGroup.Equals(default))
                {
                    continue;
                }

                var keyAssetRuleGroup = result.ToArray()[0];

                var displayRuleGroupAeonian = new DisplayRuleGroup();
                foreach (var rule in displayRuleGroup.rules)
                {
                    displayRuleGroupAeonian.AddDisplayRule(new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                        followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                        childName = rule.childName,
                        localPos = rule.localPos,
                        localAngles = rule.localAngles,
                        localScale = rule.localScale,
                        limbMask = LimbFlags.None
                    });
                }

                ArrayUtils.ArrayAppend(ref bodyIDRS.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupAeonian,
                    keyAsset = Content.Equipment.EliteAeonian
                });

                ArrayUtils.ArrayAppend(ref bodyIDRS.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupAeonian,
                    keyAsset = Content.Items.HiddenAnointed
                });
            }
            dictionary = null;
        }

        public static void Hooks()
        {
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                On.RoR2.EscapeSequenceController.EscapeSequenceMainState.OnEnter += SpawnBrokenTeleporter2;
                On.RoR2.CharacterModel.UpdateOverlays += AddDamageImmuneOverlay;
                //On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter += SpawnBrokenTeleporter;
                On.EntityStates.BrotherMonster.SpellChannelExitState.OnExit += TalkAboutLunarFlower;
                RoR2.Stage.onServerStageBegin += BazaarAddMessageIfPlayersWithRock;
                RoR2.SceneDirector.onPostPopulateSceneServer += SpawnObjects;
                BossGroup.onBossGroupStartServer += SpawnGoldTitanOnArraign;
                DirectorAPI.MixEnemiesDccsActions += GrabSpawnCardsForJudgement;
            }
        }

        [Obsolete("Please use methods from EnemiesReturns.Enemies.AnointedSkins, this method now does nothing and is only left for backcompat.")]
        public static bool AddBodyToBlacklist(string bodyName)
        {
            return false;
        }

        private static void AddDamageImmuneOverlay(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);
            if (self.body && self.activeOverlayCount < RoR2.CharacterModel.maxOverlays && self.body.HasBuff(Content.Buffs.ImmuneToAllDamageExceptHammer))
            {
                self.currentOverlays[self.activeOverlayCount++] = immuneToAllDamageExceptHammerMaterial;
            }
        }

        private static void SpawnBrokenTeleporter2(On.RoR2.EscapeSequenceController.EscapeSequenceMainState.orig_OnEnter orig, EscapeSequenceController.EscapeSequenceMainState self)
        {
            orig(self);

            if (!NetworkServer.active)
            {
                return;
            }

            var itemFound = false;
            foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
            {
                if (!playerCharacterMaster.isConnected || !playerCharacterMaster.master)
                {
                    continue;
                }

                if (!playerCharacterMaster.master.inventory)
                {
                    continue;
                }

                if (playerCharacterMaster.master.inventory.GetItemCount(Content.Items.LunarFlower) > 0)
                {
                    itemFound = true;
                    break;
                }
            }

            if (itemFound)
            {
                var position = new Vector3(-88.4849f, 491.488f, -0.3325f);
                ChildLocator component = SceneInfo.instance.GetComponent<ChildLocator>();
                if ((bool)component)
                {
                    Transform transform = component.FindChild("CenterOfArena");
                    if ((bool)transform)
                    {
                        position = transform.position;
                    }
                }

                var newTeleporter = UnityEngine.Object.Instantiate(BrokenTeleporter, position, Quaternion.identity);
                NetworkServer.Spawn(newTeleporter);
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "ENEMIES_RETURNS_JUDGEMENT_BROKEN_TELEPORTER_SPAWNED",
                });
            }
        }

        private static void GrabSpawnCardsForJudgement(DirectorCardCategorySelection mixEnemiesDccs)
        {
            foreach (var category in mixEnemiesDccs.categories)
            {
                foreach (var card in category.cards)
                {
                    if (mixEnemiesDirectorCards.Where(item => item.spawnCard == card.spawnCard).Count() == 0)
                    {
                        mixEnemiesDirectorCards.Add(card);
                    }
                }
            }
        }

        private static void SpawnGoldTitanOnArraign(BossGroup bossGroup)
        {
            CombatSquad combatSquad = bossGroup.combatSquad;
            bool flag = false;
            foreach (CharacterMaster readOnlyMembers in combatSquad.readOnlyMembersList)
            {
                if (readOnlyMembers.masterIndex == ArraignP1MasterIndex)
                {
                    flag = true;
                    break;
                }
            }
            float timer;
            if (flag)
            {
                timer = 2f;
                RoR2Application.onFixedUpdate += Check;
            }
            void Check()
            {
                bool flag2 = true;
                Vector3 approximatePosition = Vector3.zero;
                try
                {
                    if ((bool)combatSquad)
                    {
                        ReadOnlyCollection<CharacterMaster> readOnlyMembersList = combatSquad.readOnlyMembersList;
                        for (int i = 0; i < readOnlyMembersList.Count; i++)
                        {
                            CharacterMaster characterMaster = readOnlyMembersList[i];
                            if ((bool)characterMaster)
                            {
                                CharacterBody body = characterMaster.GetBody();
                                if (body)
                                {
                                    approximatePosition = body.transform.position;
                                }
                                if (body.HasBuff(RoR2Content.Buffs.HiddenInvincibility) || body.outOfCombat)
                                {
                                    flag2 = false;
                                }
                                else
                                {
                                    timer -= Time.fixedDeltaTime;
                                    if (timer > 0f)
                                    {
                                        flag2 = false;
                                    }
                                }
                            }
                        }
                        if (flag2)
                        {
                            GoldTitanManager.TryStartChannelingTitansServer(combatSquad, approximatePosition);
                        }
                    }
                }
                catch (Exception)
                {
                }
                if (flag2)
                {
                    RoR2Application.onFixedUpdate -= Check;
                }
            }
        }

        private static void TalkAboutLunarFlower(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnExit orig, EntityStates.BrotherMonster.SpellChannelExitState self)
        {
            if (NetworkServer.active)
            {
                if (self.itemStealController)
                {
                    foreach (var stolenInfo in self.itemStealController.stolenInventoryInfos)
                    {
                        if (stolenInfo != null && stolenInfo.lentItemStacks != null
                            && stolenInfo.lentItemStacks.Length > (int)Content.Items.LunarFlower.itemIndex
                            && stolenInfo.lentItemStacks[(int)Content.Items.LunarFlower.itemIndex] > 0)
                        {
                            Chat.SendBroadcastChat(new Chat.NpcChatMessage
                            {
                                baseToken = "ENEMIES_RETURNS_JUDGEMENT_MITHRIX_LUNAR_FLOWER_COMMENT",
                                formatStringToken = "BROTHER_DIALOGUE_FORMAT",
                                sender = null,
                                sound = null
                            });
                        }
                    }
                }
            }
            orig(self);
        }

        private static void BazaarAddMessageIfPlayersWithRock(Stage stage)
        {
            if (stage.sceneDef.cachedName != "bazaar")
            {
                return;
            }

            var itemFound = false;
            foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
            {
                if (!playerCharacterMaster.isConnected || !playerCharacterMaster.master)
                {
                    continue;
                }

                if (!playerCharacterMaster.master.inventory)
                {
                    continue;
                }

                if (playerCharacterMaster.master.inventory.GetItemCount(Content.Items.TradableRock) > 0)
                {
                    itemFound = true;
                    break;
                }
            }

            if (!itemFound)
            {
                return;
            }

            var shopkeeperTrigger = GameObject.Find("HOLDER: Store/HOLDER: Store Platforms/ShopkeeperPosition/SpawnShopkeeperTrigger");
            if (!shopkeeperTrigger)
            {
                return;
            }

            var gameObject = shopkeeperTrigger.gameObject;
            var chatMessage = gameObject.AddComponent<SendChatMessage>();
            chatMessage.messageToken = "ENEMIES_RETURNS_JUDGEMENT_NEWT_TRADE_OFFER";
            chatMessage.withDelay = true;
            chatMessage.delay = 3f;

            var onPlayerEnterEvent = shopkeeperTrigger.gameObject.GetComponent<OnPlayerEnterEvent>();
            onPlayerEnterEvent.action.AddListener(chatMessage.Send);
        }

        private static void SpawnBrokenTeleporter(On.EntityStates.Missions.BrotherEncounter.BossDeath.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BossDeath self)
        {
            orig(self);

            if (!NetworkServer.active)
            {
                return;
            }

            if (!self.childLocator)
            {
                return;
            }

            var center = self.childLocator.FindChild("CenterOfArena");
            if (!center)
            {
                return;
            }

            var itemFound = false;
            foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
            {
                if (!playerCharacterMaster.isConnected || !playerCharacterMaster.master)
                {
                    continue;
                }

                if (!playerCharacterMaster.master.inventory)
                {
                    continue;
                }

                if (playerCharacterMaster.master.inventory.GetItemCount(Content.Items.LunarFlower) > 0)
                {
                    itemFound = true;
                    break;
                }
            }

            if (itemFound)
            {
                var newTeleporter = UnityEngine.Object.Instantiate(BrokenTeleporter, center.position, Quaternion.identity);
                NetworkServer.Spawn(newTeleporter);
            }
        }

        public static void SpawnObjects(SceneDirector sceneDirector)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!SceneInfo.instance
                || !SceneInfo.instance.sceneDef)
            {
                return;
            }

            if (SceneInfo.instance.sceneDef.baseSceneName == "mysteryspace"
                && PileOfDirt)
            {
                var newPile = UnityEngine.Object.Instantiate(PileOfDirt);
                newPile.transform.position = new Vector3(44.72055f, -55.80222f, 0.2936229f);
                newPile.transform.rotation = Quaternion.Euler(39.434f, 355.6797f, 13.5983f);
                NetworkServer.Spawn(newPile);
            }
        }

        public static void AddInteractabilityToNewt()
        {
            var shopkeeperBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Shopkeeper/ShopkeeperBody.prefab").WaitForCompletion();

            var newtTrader = shopkeeperBody.AddComponent<Behaviors.Judgement.Newt.NewtTrader>();
            newtTrader.contextString = "ENEMIES_RETURNS_JUDGEMENT_NEWT_TRADE_CONTEXT";
            newtTrader.itemToTake = Content.Items.TradableRock;
            newtTrader.itemToGive = Content.Items.LunarFlower;
            newtTrader.available = true;
            newtTrader.chatMessageOnInteraction = "ENEMIES_RETURNS_JUDGEMENT_NEWT_SUCCESSFUL_TRADE";

            var procFilter = shopkeeperBody.AddComponent<InteractionProcFilter>();
            procFilter.shouldAllowOnInteractionBeginProc = false;

            var highlight = shopkeeperBody.AddComponent<Highlight>();
            highlight.targetRenderer = shopkeeperBody.transform.Find("ModelBase/mdlNewtShopkeeper/NewtMesh").gameObject.GetComponent<Renderer>();

            shopkeeperBody.AddComponent<EntityLocator>().entity = shopkeeperBody;
        }

        public static void AddWeaponDropToMithrix()
        {
            var mithrixHurtBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherHurtMaster.prefab").WaitForCompletion();

            var dropEquipment = mithrixHurtBody.AddComponent<DropEquipment>();
            dropEquipment.itemToCheck = Content.Items.LunarFlower;
            dropEquipment.equipmentToDrop = Content.Equipment.MithrixHammer;
            dropEquipment.dropChatToken = "ENEMIES_RETURNS_JUDGEMENT_HAMMER_DROP";
        }

        public static GameObject CloneOptionPickerPanel()
        {
            var panel = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OptionPickup/OptionPickerPanel.prefab").WaitForCompletion().InstantiateClone("JudgementPickerPanel", false);
            panel.transform.Find("MainPanel/Juice/Label").gameObject.GetComponent<LanguageTextMeshController>().token = "ENEMIES_RETURNS_JUDGEMENT_OPTION_PICKUP_HEADER";

            return panel;
        }

        public static GameObject SetupBrokenTeleporter(GameObject prefab)
        {
            prefab.transform.Find("MegaTeleporterPrefab/MegaLunarTeleporter(Clone)/MegaLunarTeleporter").GetComponent<SkinnedMeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon/matMoonRuinsDirtyArena.mat").WaitForCompletion();
            prefab.transform.Find("MegaTeleporterPrefab/TeleporterVessel(Clone)/MegaLunarTeleporter").GetComponent<SkinnedMeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon/matMoonRuinsDirtyArena.mat").WaitForCompletion();

            prefab.transform.Find("MegaTeleporterPrefab/TeleporterVessel(Clone)/PickupLunarFlower/itemJudgeAccess/Sphere").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidoutro/matCapturedPlantOrb.mat").WaitForCompletion();
            prefab.transform.Find("MegaTeleporterPrefab/TeleporterVessel(Clone)/PickupLunarFlower/itemJudgeAccess/MoonGhostPlant1").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidoutro/matCapturedPlantPlant.mat").WaitForCompletion();

            return prefab;
        }

        public static GameObject SetupLunarFlower(GameObject prefab)
        {
            prefab.transform.Find("itemJudgeAccess/Sphere").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidoutro/matCapturedPlantOrb.mat").WaitForCompletion();
            prefab.transform.Find("itemJudgeAccess/MoonGhostPlant1").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidoutro/matCapturedPlantPlant.mat").WaitForCompletion();

            return prefab;
        }

        public static GameObject SetupLunarKey(GameObject prefab)
        {
            prefab.transform.Find("lunarKey/LunarExploderCoreMesh").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarExploder/matLunarExploderCore.mat").WaitForCompletion();
            prefab.transform.Find("lunarKey/meshLunarKeyGlass").gameObject.GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherStib.mat").WaitForCompletion();

            return prefab;
        }

        public static Dictionary<string, DisplayRuleGroup> CreateAeonianAnointedDictionary()
        {
            var dictionary = new Dictionary<string, DisplayRuleGroup>();
#pragma warning disable CS0618 // Type or member is obsolete
            dictionary.Add("ToolbotBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.01192F, 2.33924F, 2.68312F),
                    localAngles = new Vector3(332.3655F, 0.05552F, 177.9755F),
                    localScale = new Vector3(0.29092F, 0.29092F, 0.29092F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("TreebotBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "FlowerBase",
                    localPos = new Vector3(0.01413F, 2.30119F, -0.18182F),
                    localAngles = new Vector3(270.074F, 0F, 0F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("RailgunnerBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.00989F, 0.23731F, -0.11833F),
                    localAngles = new Vector3(289.0599F, 180.9201F, 181.0143F),
                    localScale = new Vector3(0.021F, 0.021F, 0.021F),
                    limbMask = LimbFlags.None
                }
            }));


            dictionary.Add("FalseSonBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.001F, 0.77918F, -0.01667F),
                    localAngles = new Vector3(271.5699F, 0.00085F, -0.00086F),
                    localScale = new Vector3(0.04289F, 0.04289F, 0.04289F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("ChefBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.34146F, -0.09502F, 0.02628F),
                    localAngles = new Vector3(357.3232F, 272.0138F, 198.2701F),
                    localScale = new Vector3(0.04278F, 0.04278F, 0.04278F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("BeetleBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.01962F, 0.38285F, 0.66161F),
                    localAngles = new Vector3(296.7061F, 354.1524F, 186.5402F),
                    localScale = new Vector3(0.1F, 0.1F, 0.1F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("GupBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "MainBody2",
                    localPos = new Vector3(0.05864F, 1.10224F, -0.16438F),
                    localAngles = new Vector3(274.7971F, 180F, 180F),
                    localScale = new Vector3(0.10666F, 0.10652F, 0.1066F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("ParentBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-114.9762F, 140.3186F, 1F),
                    localAngles = new Vector3(315.0002F, 269.9998F, 180.0001F),
                    localScale = new Vector3(10F, 10F, 10F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("ScorchlingBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.27257F, 0.6441F, -0.047F),
                    localAngles = new Vector3(279.0522F, 163.4916F, 286.3138F),
                    localScale = new Vector3(0.16449F, 0.16449F, 0.16449F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("SS2UNemmandoBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0.00006F, 0.00524F, -0.00051F),
                    localAngles = new Vector3(90F, 180F, 0F),
                    localScale = new Vector3(0.0003F, 0.0003F, 0.0003F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("EnforcerBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0.09564F, 0.31238F, 0.00459F),
                    localAngles = new Vector3(270F, 270F, 0F),
                    localScale = new Vector3(0.03F, 0.03F, 0.03F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("RobHunkBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(1.79623F, 28.51547F, 3.92431F),
                    localAngles = new Vector3(271.5699F, 0.00085F, -0.00086F),
                    localScale = new Vector3(1.87715F, 1.87715F, 1.87715F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("RobBelmontBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.09622F, 0.49185F, -0.01094F),
                    localAngles = new Vector3(271.5699F, 0.00085F, -0.00086F),
                    localScale = new Vector3(0.04419F, 0.04419F, 0.04419F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("CadetBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0.01117F, 0.33053F, -0.09775F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.02854F, 0.02854F, 0.02854F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("RobPaladinBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0F, 0.59181F, 0.01854F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.03551F, 0.03849F, 0.03551F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("SS2UExecutionerBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0F, 0.0055F, -0.0008F),
                    localAngles = new Vector3(62.04026F, 0F, 0F),
                    localScale = new Vector3(0.0003F, 0.0003F, 0.0003F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("SS2UCyborgBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "head_end",
                    localPos = new Vector3(0F, 0.39848F, -0.03963F),
                    localAngles = new Vector3(272.2047F, 180F, 180F),
                    localScale = new Vector3(0.03651F, 0.03651F, 0.03651F),
                    limbMask = LimbFlags.None
                }
            }));

            dictionary.Add("GnomeChefBody", CreateItemDisplayRule(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0F, 0.00917F, -0.00314F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.0013F, 0.0013F, 0.0013F),
                    limbMask = LimbFlags.None
            }
            }));
#pragma warning restore CS0618 // Type or member is obsolete
            return dictionary;

            DisplayRuleGroup CreateItemDisplayRule(ItemDisplayRule[] rules)
            {
                var drg = new DisplayRuleGroup();
                foreach (var rule in rules)
                {
                    drg.AddDisplayRule(rule);
                }
                return drg;
            }
        }
    }
}
