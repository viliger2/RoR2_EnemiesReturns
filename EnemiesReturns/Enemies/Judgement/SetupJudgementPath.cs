using EnemiesReturns.Behaviors;
using EnemiesReturns.Behaviors.Judgement.MithrixWeaponDrop;
using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Judgement
{
    public static class SetupJudgementPath
    {
        public static GameObject PileOfDirt;

        public static GameObject BrokenTeleporter;

        public static GameObject JudgementInteractable;

        public static Texture2D aeonianEliteRamp;

        public static Dictionary<string, UnlockableDef> AnointedSkinsUnlockables = new Dictionary<string, UnlockableDef>();

        private static HashSet<SkinDef> AnointedSkins;

        private static readonly ConditionalWeakTable<CharacterModel, ModelSkinController> skinControlerDictionary = new ConditionalWeakTable<CharacterModel, ModelSkinController>();

        public static void Hooks()
        {
            if (Configuration.Judgement.Enabled.Value)
            {
                On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter += SpawnBrokenTeleporter;
                RoR2.Stage.onServerStageBegin += BazaarAddMessageIfPlayersWithRock;
                RoR2.SceneDirector.onPostPopulateSceneServer += SpawnObjects;
                if (Configuration.Judgement.EnableAnointedSkins.Value)
                {
                    RoR2.ContentManagement.ContentManager.onContentPacksAssigned += CreateAnointedSkins;
                    IL.RoR2.CharacterModel.UpdateMaterials += SetupAnointedMaterials;
                    On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.ApplyLoadoutToMannequinInstance += AddAnointedOverlay;
                    IL.RoR2.UI.LoadoutPanelController.Row.FromSkin += HideHiddenSkinDefs;
                }
            }
        }

        private static void BazaarAddMessageIfPlayersWithRock(Stage stage)
        {
            if(stage.sceneDef.cachedName != "bazaar")
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

        private static void HideHiddenSkinDefs(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            ILLabel lable = null;
            var jumpMatch = c.TryGotoNext(MoveType.After,
                x => x.MatchBrfalse(out lable));

            if(jumpMatch)
            {
                c.Index -= 6;

                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 5);
                c.EmitDelegate<Func<RoR2.UI.LoadoutPanelController, SkinDef, bool>>(CheckForSpecialSkinDef);
                c.Emit(OpCodes.Brtrue, lable.Target);
            } else
            {
                Log.Error("RoR2.UI.LoadoutPanelController.Row.FromSkin IL hook failed.");
            }

            static bool CheckForSpecialSkinDef(RoR2.UI.LoadoutPanelController owner, SkinDef skinDef)
            {
                if (!skinDef.unlockableDef)
                {
                    return false;
                }

                if(owner == null)
                {
                    return false;
                }

                UserProfile obj = owner.currentDisplayData.userProfile;
                if(obj == null)
                {
                    return false;
                }

                if (obj.HasUnlockable(skinDef.unlockableDef))
                {
                    return false;
                }
                if(skinDef is HiddenSkinDef)
                {
                    return (skinDef as HiddenSkinDef).hideInLobby;
                }
                return false;
            }
        }

        private static void AddAnointedOverlay(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_ApplyLoadoutToMannequinInstance orig, RoR2.SurvivorMannequins.SurvivorMannequinSlotController self)
        {
            orig(self);

            if (self && self.currentSurvivorDef && self.currentSurvivorDef.survivorIndex != SurvivorIndex.None)
            {
                BodyIndex bodyIndexFromSurvivorIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex(self.currentSurvivorDef.survivorIndex);
                int skinIndex = (int)self.currentLoadout.bodyLoadoutManager.GetSkinIndex(bodyIndexFromSurvivorIndex);
                SkinDef safe = ArrayUtils.GetSafe(BodyCatalog.GetBodySkins(bodyIndexFromSurvivorIndex), skinIndex);
                CharacterModel characterModel = self.mannequinInstanceTransform.GetComponentInChildren<CharacterModel>();
                if (characterModel)
                {
                    if (AnointedSkins.Contains(safe))
                    {
                        // this is such a fucking hack holy shit but this is only for the lobby so it should be fine
                        characterModel.inventoryEquipmentIndex = Content.Equipment.EliteAeonian.equipmentIndex;
                    }
                    else
                    {
                        characterModel.inventoryEquipmentIndex = EquipmentIndex.None;
                    }
                }
            }
        }

        // shamelessly copy pasted from EliteAPI
        private static void SetupAnointedMaterials(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            var firstMatchSuccesful = c.TryGotoNext(MoveType.After,
                x => x.MatchLdarg(0),
                x => x.MatchLdfld<CharacterModel>(nameof(CharacterModel.propertyStorage)),
                x => x.MatchLdsfld(typeof(CommonShaderProperties), nameof(CommonShaderProperties._Fade)));

            var secondMatchSuccesful = c.TryGotoNext(MoveType.After,
                x => x.MatchCallOrCallvirt<MaterialPropertyBlock>(nameof(MaterialPropertyBlock.SetFloat)));

            if (firstMatchSuccesful && secondMatchSuccesful)
            {
                c.Emit(OpCodes.Ldarg, 0);
                c.EmitDelegate<Action<CharacterModel>>(UpdateRampProperly);
            }
            else
            {
               Log.Error($"Elite Ramp ILHook failed");
            }

            static void UpdateRampProperly(CharacterModel charModel)
            {
                if (charModel.shaderEliteRampIndex == -1)
                {
                    //var modelSkinController = charModel.gameObject.GetComponent<ModelSkinController>();
                    if (!skinControlerDictionary.TryGetValue(charModel, out var modelSkinController))
                    {
                        modelSkinController = charModel.gameObject.GetComponent<ModelSkinController>();
                        skinControlerDictionary.AddOrUpdate(charModel, modelSkinController);
                    }
                    if (modelSkinController && modelSkinController.currentSkinIndex > 0)
                    {
                        var skin = modelSkinController.skins[modelSkinController.currentSkinIndex];
                        if (AnointedSkins.Contains(skin))
                        {
                            charModel.propertyStorage.SetTexture(Behaviors.SetEliteRampOnShader.EliteRampPropertyID, aeonianEliteRamp);
                            charModel.propertyStorage.SetFloat(CommonShaderProperties._EliteIndex, 1f);
                        }
                    }
                }
            }
        }

        private static void CreateAnointedSkins(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            List<SkinDef> anointedSkins = new List<SkinDef>();
            for(int i = 0; i < RoR2.ContentManagement.ContentManager._survivorDefs.Length; i++)
            {
                var survivorDef = RoR2.ContentManagement.ContentManager._survivorDefs[i];
                if (survivorDef.bodyPrefab)
                {
                    var body = survivorDef.bodyPrefab;
                    var modelLocator = body.GetComponent<ModelLocator>();
                    if (!modelLocator)
                    {
                        Log.Warning($"Survivor {survivorDef.cachedName} doesn't have ModelLocator component.");
                        continue;
                    }
                    var model = modelLocator.modelTransform;
                    if (!model)
                    {
                        Log.Warning($"Survivor {survivorDef.cachedName} doesn't have model (somehow).");
                        continue;
                    }
                    var characterModel = model.GetComponent<CharacterModel>();
                    if (!characterModel)
                    {
                        Log.Warning($"Survivor {survivorDef.cachedName} doesn't have CharacterModel component.");
                        continue;
                    }
                    var modelSkins = model.GetComponent<ModelSkinController>();
                    if (!modelSkins)
                    {
                        Log.Warning($"Survivor {survivorDef.cachedName} doesn't have ModelSkinController component.");
                        continue;
                    }

                    // trying to find baseSkins for survivor
                    SkinDef defaultSkin = null;
                    foreach (var skin in modelSkins.skins)
                    {
                        if (skin.baseSkins != null && skin.baseSkins.Length > 0)
                        {
                            defaultSkin = skin.baseSkins[0];
                            break;
                        }
                    }

                    CharacterModel.RendererInfo[] skinRenderInfos = new CharacterModel.RendererInfo[characterModel.baseRendererInfos.Length];
                    for (int k = 0; k < skinRenderInfos.Length; k++)
                    {
                        var baseRenderInfo = characterModel.baseRendererInfos[k];

                        Material newMaterial = null;
                        if (baseRenderInfo.defaultMaterial)
                        {
                            if (!ContentProvider.MaterialCache.TryGetValue(baseRenderInfo.defaultMaterial.name + "EnemiesReturnsAnointed", out newMaterial))
                            {
                                newMaterial = UnityEngine.Object.Instantiate(baseRenderInfo.defaultMaterial);
                                newMaterial.name = baseRenderInfo.defaultMaterial.name + "EnemiesReturnsAnointed";
                                newMaterial.SetTexture(Behaviors.SetEliteRampOnShader.EliteRampPropertyID, aeonianEliteRamp);
                                newMaterial.SetFloat(CommonShaderProperties._EliteIndex, 1);
                                ContentProvider.MaterialCache.Add(newMaterial.name, newMaterial);
                            }
                        } else
                        {
                            Log.Warning($"Survivor {survivorDef.cachedName} has an empty material on baseRendererInfos at index {k}.");
                        }
                        skinRenderInfos[k] = new CharacterModel.RendererInfo 
                        {
                            renderer = baseRenderInfo.renderer,
                            defaultMaterial = newMaterial,
                            defaultShadowCastingMode = baseRenderInfo.defaultShadowCastingMode,
                            hideOnDeath = baseRenderInfo.hideOnDeath,
                            ignoreOverlays = baseRenderInfo.ignoreOverlays,
                        };
                    }

                    var eliteSkinDef = Utils.CreateHiddenSkinDef($"skin{survivorDef.cachedName}EnemiesReturnsAnointed", model.gameObject, skinRenderInfos, true, defaultSkin);
                    eliteSkinDef.nameToken = "ENEMIES_RETURNS_SKIN_ANOINTED_NAME";
                    //eliteSkinDef.icon = ;

                    if (!Configuration.Judgement.ForceUnlock.Value)
                    {
                        var skinUnlockDef = ScriptableObject.CreateInstance<UnlockableDef>();
                        (skinUnlockDef as ScriptableObject).name = $"Skins.{survivorDef.cachedName}.EnemiesReturnsAnointed";
                        skinUnlockDef.cachedName = $"Skins.{survivorDef.cachedName}.EnemiesReturnsAnointed";
                        skinUnlockDef.nameToken = "ENEMIES_RETURNS_SKIN_ANOINTED_NAME";
                        skinUnlockDef.hidden = false; // it actually does fucking nothing, it only hides it on game finish

                        AnointedSkinsUnlockables.Add(survivorDef.bodyPrefab.name.Trim().ToLower(), skinUnlockDef);

                        eliteSkinDef.unlockableDef = skinUnlockDef;

                        HG.ArrayUtils.ArrayAppend(ref RoR2.ContentManagement.ContentManager._unlockableDefs, skinUnlockDef);
                    }

                    var skinsArray = modelSkins.skins;
                    var index = skinsArray.Length;
                    Array.Resize(ref skinsArray, index + 1);
                    skinsArray[index] = eliteSkinDef;
                    modelSkins.skins = skinsArray;

                    anointedSkins.Add(eliteSkinDef);
                }
            }
            AnointedSkins = new HashSet<SkinDef>(anointedSkins);
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

            if (SceneInfo.instance.sceneDef.baseSceneName == "arena"
                && PileOfDirt)
            {
                var newPile = UnityEngine.Object.Instantiate(PileOfDirt);
                newPile.transform.position = new Vector3(113.1104f, 37.679f, 272.3562f);
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
            newtTrader.available = true; // TODO: eh?
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
    }
}
