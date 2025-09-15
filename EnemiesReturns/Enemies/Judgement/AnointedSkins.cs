using BepInEx.Configuration;
using EnemiesReturns.Components;
using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2BepInExPack.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Judgement
{
    public class AnointedSkins
    {
        public static Sprite AnointedSkinIcon;

        public static Texture2D aeonianEliteRamp;

        public static GameObject AeonianAnointedItemDisplay;

        internal static Dictionary<string, UnlockableDef> AnointedSkinsUnlockables = new Dictionary<string, UnlockableDef>();

        internal static Dictionary<UnlockableDef, string> AnointedSkinsUnlockablesAchivements = new Dictionary<UnlockableDef, string>();

        private static HashSet<SkinDef> AnointedSkinsOverlayHashSet = new HashSet<SkinDef>();

        private static HashSet<SkinDef> AnointedSkinsItemHashSet = new HashSet<SkinDef>();

        private static List<SkinDef> anointedSkinsSecondIterationList = new List<SkinDef>();

        private static HashSet<string> AnointedBlacklist = new HashSet<string>();

        private static readonly FixedConditionalWeakTable<CharacterModel, ModelSkinController> skinControlerDictionary = new FixedConditionalWeakTable<CharacterModel, ModelSkinController>();

        internal static List<UnlockableDef> skinUnlockables = new List<UnlockableDef>();

        internal static void Hooks()
        {
            if (Configuration.Judgement.Judgement.Enabled.Value && Configuration.Judgement.Judgement.EnableAnointedSkins.Value)
            {
                RoR2.ContentManagement.ContentManager.onContentPacksAssigned += CreateAnointedSkins;
                RoR2.AchievementManager.onAchievementsRegistered += CreateAnointedAchievements;
                IL.RoR2.CharacterModel.UpdateMaterials += SetupAnointedMaterials;
                On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.ApplyLoadoutToMannequinInstance += AddAnointedOverlay;
                IL.RoR2.UI.LoadoutPanelController.Row.FromSkin += HideHiddenSkinDefs;
                RoR2.CharacterBody.onBodyStartGlobal += AddAnointedItem;
            }
        }

        /// <summary>
        /// Creates Anointed skin by cloning existing <see cref="SkinDef"/> and making <see cref="HiddenSkinDef"/> 
        /// out of it with apropriate <see cref="UnlockableDef"/> and <see cref="EnemiesReturns.Achievements.JudgementClearedAchievement"/> tied to it.
        /// You will need to replace all references to your original <see cref="SkinDef"/> (like those in <see cref="ModelSkinController"/>) manually.
        /// This method needs to be called before all catalogs are initialized, otherwise EnemiesReturns will still generate "default" Anointed skin.
        /// Method also respects all of the config options of EnemiesReturns, so if Judgement is disabled no skin will be generated and original <see cref="SkinDef"/> will be returned,
        /// if skins are ForceUnlocked then no <see cref="UnlockableDef"/> will be attached to the skin.
        /// </summary>
        /// <param name="bodyName">Name of the body, used for <see cref="ScriptableObject"/> names and later to properly setup achivement for this specific body.</param>
        /// <param name="fromSkin"><see cref="SkinDef"/> being cloned.</param>
        /// <param name="addEliteRamp">Whether skin uses aenonian elite\anointed ramp, like those generated automatically.</param>
        /// <returns><see cref="HiddenSkinDef"/> as <see cref="SkinDef"/></returns>
        public static SkinDef CreateAnointedSkin(string bodyName, SkinDef fromSkin, bool addEliteRamp)
        {
            if (!Configuration.Judgement.Judgement.EnableAnointedSkins.Value || !Configuration.Judgement.Judgement.Enabled.Value)
            {
                Log.Warning($"Anointed Skins are disabled in EnemiesReturns. Doing nothing with skin {fromSkin} for body {bodyName}.");
                return fromSkin;
            }

            HiddenSkinDef hiddenSkin = HiddenSkinDef.FromSkinDef(fromSkin);
            hiddenSkin.unlockableDef = CreateAnointedUnlockable(bodyName);
            if (hiddenSkin.unlockableDef)
            {
                skinUnlockables.Add(hiddenSkin.unlockableDef);
            }

            AnointedSkinsItemHashSet.Add(hiddenSkin);
            if (addEliteRamp)
            {
                AnointedSkinsOverlayHashSet.Add(hiddenSkin);
            }
            AnointedBlacklist.Add(bodyName);

            return hiddenSkin;
        }

        // second iteration is purely to support modded skins
        [SystemInitializer(new Type[] {typeof(BodyCatalog)})]
        private static IEnumerator CreateAnointedSkinsSecondIteration()
        {
            var judgementConfiguration = Configuration.Judgement.Judgement.JudgementConfig;
            for(int i = 0; i < anointedSkinsSecondIterationList.Count; i++)
            {
                SkinDef skin = anointedSkinsSecondIterationList[i];
                if (!skin.rootObject)
                {
                    continue;
                }

                if(!skin.rootObject.TryGetComponent<CharacterModel>(out var characterModel))
                {
                    continue;
                }

                var body = characterModel.GetComponent<CharacterModel>().body;
                if (!body)
                {
                    continue;
                }

                if (!judgementConfiguration.TryGetEntry<string>("Anointed Skins", body.name, out var entry))
                {
                    continue;
                }

                if(!skin.rootObject.TryGetComponent<ModelSkinController>(out var controller))
                {
                    continue;
                }

                var anointedBaseSkinName = entry.Value;
                var customSkins = controller.skins.Where(skin => skin.name == anointedBaseSkinName).ToArray();
                if(customSkins.Length == 0)
                {
                    Log.Info($"Couldn't find skin with name {anointedBaseSkinName} for {body.name} for Anointed skin creation on second iteration. Default skin will be used.");
                    continue;
                }

                var baseCustomSkin = customSkins[0];
                skin.baseSkins = new SkinDef[] { baseCustomSkin };
                skin.skinDefParams = baseCustomSkin.skinDefParams;
                skin.rendererInfos = baseCustomSkin.rendererInfos;
                skin.gameObjectActivations = baseCustomSkin.gameObjectActivations;
                skin.meshReplacements = baseCustomSkin.meshReplacements;
                skin.projectileGhostReplacements = baseCustomSkin.projectileGhostReplacements;
                skin.minionSkinReplacements = baseCustomSkin.minionSkinReplacements;
                skin._runtimeSkin = baseCustomSkin._runtimeSkin;
            }
            for (int i = 0; i < anointedSkinsSecondIterationList.Count; i++)
            {
                var result = anointedSkinsSecondIterationList[i].BakeAsync();
                while (result.MoveNext())
                {
                    yield return null;
                }
            }
            anointedSkinsSecondIterationList = null;
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
                Log.Warning($"IL Hook Failed - RoR2.CharacterModel.UpdateMaterials: Anointed skins won't have their elite ramp applied.");
            }

            static void UpdateRampProperly(CharacterModel charModel)
            {
                if (charModel.shaderEliteRampIndex == -1)
                {
                    if (!skinControlerDictionary.TryGetValue(charModel, out var modelSkinController))
                    {
                        modelSkinController = charModel.gameObject.GetComponent<ModelSkinController>();
                        skinControlerDictionary.Add(charModel, modelSkinController);
                    }
                    if (modelSkinController && modelSkinController.currentSkinIndex > 0)
                    {
                        var skin = modelSkinController.skins[modelSkinController.currentSkinIndex];
                        if (AnointedSkinsOverlayHashSet.Contains(skin))
                        {
                            charModel.propertyStorage.SetTexture(Behaviors.SetEliteRampOnShader.EliteRampPropertyID, aeonianEliteRamp);
                            charModel.propertyStorage.SetFloat(CommonShaderProperties._EliteIndex, 1f);
                        }
                    }
                }
            }
        }

        // we basically rely on combination of mistfixes and skinapi to fix our shit
        // thanks randy, you made this all possible
        private static void CreateAnointedSkins(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            var judgementConfiguration = Configuration.Judgement.Judgement.JudgementConfig;

            var icon = AnointedSkinIcon;

            List<SkinDef> anointedSkinsLocal = new List<SkinDef>();
            for (int i = 0; i < RoR2.ContentManagement.ContentManager._survivorDefs.Length; i++)
            {
                var survivorDef = RoR2.ContentManagement.ContentManager._survivorDefs[i];
                if (!survivorDef.bodyPrefab)
                {
                    continue;
                }
                var body = survivorDef.bodyPrefab;

                if (AnointedBlacklist.Contains(body.name))
                {
#if DEBUG || NOWEAVER
                    Log.Info($"Survivor {survivorDef.cachedName} with body named {body.name} is in blacklist, skipping skin creation...");
#endif
                    continue;
                }

                var modelLocator = body.GetComponent<ModelLocator>();
                if (!modelLocator)
                {
                    Log.Warning($"Body {body.name} doesn't have ModelLocator component.");
                    continue;
                }
                var model = modelLocator.modelTransform;
                if (!model)
                {
                    Log.Warning($"Body {body.name} doesn't have model (somehow).");
                    continue;
                }
                var characterModel = model.GetComponent<CharacterModel>();
                if (!characterModel)
                {
                    Log.Warning($"Body {body.name} doesn't have CharacterModel component.");
                    continue;
                }
                var modelSkins = model.GetComponent<ModelSkinController>();
                if (!modelSkins)
                {
                    Log.Warning($"Body {body.name} doesn't have ModelSkinController component.");
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

                if (!defaultSkin)
                {
                    Log.Warning($"Couldn't find Default Skin for {body.name}.");
                    continue;
                }

                bool addToList = false;
                var targetSkinConfig = judgementConfiguration.Bind<string>("Anointed Skins", body.name, defaultSkin.name, $"Target skin for {body.name}, use DebugToolkit's \"list_skin\" command to get all available skins. If skin value is not found then default skin will be used. This is technically client side, but hasn't been tested in any real modpacks.");
                var targetSkinArray = modelSkins.skins.Where(skinDef => skinDef.name == targetSkinConfig.Value).ToArray();
                if (targetSkinArray.Length > 0)
                {
                    defaultSkin = targetSkinArray[0];
                }
                else
                {
                    Log.Info($"Couldn't find skin with name {targetSkinConfig.Value} for {body.name} for Anointed skin creation. Will try again on second iteration.");
                    addToList = true;
                }

                var eliteSkinDef = Utils.CreateHiddenSkinDef($"skin{body.name}EnemiesReturnsAnointed", model.gameObject, hideInLobby: true, baseSkin: defaultSkin);
                eliteSkinDef.nameToken = "ENEMIES_RETURNS_JUDGEMENT_SKIN_ANOINTED_NAME";
                eliteSkinDef.icon = icon;
                eliteSkinDef.unlockableDef = CreateAnointedUnlockable(body.name);
                if (eliteSkinDef.unlockableDef)
                {
                    HG.ArrayUtils.ArrayAppend(ref RoR2.ContentManagement.ContentManager._unlockableDefs, eliteSkinDef.unlockableDef);
                }

                var skinsArray = modelSkins.skins;
                var index = skinsArray.Length;
                Array.Resize(ref skinsArray, index + 1);
                skinsArray[index] = eliteSkinDef;
                modelSkins.skins = skinsArray;

                anointedSkinsLocal.Add(eliteSkinDef);
                if (addToList)
                {
                    anointedSkinsSecondIterationList.Add(eliteSkinDef);
                }
            }
            foreach (var skin in anointedSkinsLocal)
            {
                AnointedSkinsOverlayHashSet.Add(skin);
                AnointedSkinsItemHashSet.Add(skin);
            }

            foreach (var unlockable in skinUnlockables)
            {
                HG.ArrayUtils.ArrayAppend(ref RoR2.ContentManagement.ContentManager._unlockableDefs, unlockable);
            }
            skinUnlockables = null;
        }

        private static UnlockableDef CreateAnointedUnlockable(string bodyName)
        {
            UnlockableDef skinUnlockDef = null;
            if (!Configuration.Judgement.Judgement.ForceUnlock.Value)
            {
                skinUnlockDef = ScriptableObject.CreateInstance<UnlockableDef>();
                (skinUnlockDef as ScriptableObject).name = $"Skins.{bodyName}.EnemiesReturnsAnointed";
                skinUnlockDef.cachedName = $"Skins.{bodyName}.EnemiesReturnsAnointed";
                skinUnlockDef.nameToken = "ENEMIES_RETURNS_JUDGEMENT_SKIN_ANOINTED_NAME";
                skinUnlockDef.hidden = false; // it actually does fucking nothing, it only hides it on game finish
                skinUnlockDef.achievementIcon = AnointedSkinIcon;

                AnointedSkinsUnlockables.Add(bodyName, skinUnlockDef);
                AnointedSkinsUnlockablesAchivements.Add(skinUnlockDef, bodyName);
            }

            return skinUnlockDef;
        }

        // but seriously I copy pasted like half of the method, I hope this works
        private static void CreateAnointedAchievements()
        {
            if (Configuration.Judgement.Judgement.ForceUnlock.Value)
            {
                return;
            }

            for (int i = 0; i < RoR2.ContentManagement.ContentManager._survivorDefs.Length; i++)
            {
                var survivorDef = RoR2.ContentManagement.ContentManager._survivorDefs[i];
                if (!survivorDef)
                {
                    continue;
                }

                var bodyName = survivorDef.bodyPrefab.name.Trim();

                if (!AnointedSkinsUnlockables.TryGetValue(bodyName, out UnlockableDef skinUnlockable))
                {
#if DEBUG || NOWEAVER
                    Log.Info($"Couldn't find Anointed UnlockableDef for body {bodyName}, most likely because it is in blacklist.");
#endif
                    continue;
                }

                AchievementDef cheevoDef = new AchievementDef
                {
                    identifier = "EnemiesReturns" + bodyName + "JudgementClearedNew",
                    unlockableRewardIdentifier = skinUnlockable.cachedName,
                    prerequisiteAchievementIdentifier = null,
                    nameToken = "ENEMIES_RETURNS_" + (bodyName + "JudgementCleared").ToUpper() + "_NAME",
                    descriptionToken = "ENEMIES_RETURNS_" + (bodyName + "JudgementCleared").ToUpper() + "_DESC",
                    type = typeof(Achievements.JudgementClearedAchievement),
                    serverTrackerType = typeof(Achievements.JudgementClearedAchievement.JudgementClearedServerAchievement),
                    lunarCoinReward = 10u
                };

                RoR2.Language.currentLanguage.SetStringByToken(cheevoDef.nameToken, RoR2.Language.GetString(survivorDef.displayNameToken) + ": ???");
                RoR2.Language.currentLanguage.SetStringByToken(cheevoDef.descriptionToken, RoR2.Language.GetString("ENEMIES_RETURNS_JUDGEMENT_ACHIEVEMENT_SURVIVE_JUDGEMENT_DESC"));

                if (skinUnlockable.achievementIcon)
                {
                    cheevoDef.SetAchievedIcon(skinUnlockable.achievementIcon);
                }

                AchievementManager.achievementIdentifiers.Add(cheevoDef.identifier);
                AchievementManager.achievementNamesToDefs.Add(cheevoDef.identifier, cheevoDef);
                HG.ArrayUtils.ArrayAppend(ref AchievementManager.achievementDefs, cheevoDef);

                if (skinUnlockable)
                {
                    skinUnlockable.getHowToUnlockString = () => RoR2.Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", "???", "???");
                    skinUnlockable.getUnlockedString = () => RoR2.Language.GetStringFormatted("UNLOCKED_FORMAT", RoR2.Language.GetString("ENEMIES_RETURNS_JUDGEMENT_SKIN_ANOINTED_NAME"), RoR2.Language.GetString("ENEMIES_RETURNS_JUDGEMENT_ACHIEVEMENT_SURVIVE_JUDGEMENT_DESC"));
                }
            }

            AchievementManager.SortAchievements(AchievementManager.achievementDefs);
            AchievementManager.serverAchievementDefs = AchievementManager.achievementDefs.Where((AchievementDef achievementDef) => achievementDef.serverTrackerType != null).ToArray();
            for (int j = 0; j < AchievementManager.achievementDefs.Length; j++)
            {
                AchievementManager.achievementDefs[j].index = new AchievementIndex
                {
                    intValue = j
                };
            }
            for (int k = 0; k < AchievementManager.serverAchievementDefs.Length; k++)
            {
                AchievementManager.serverAchievementDefs[k].serverIndex = new ServerAchievementIndex
                {
                    intValue = k
                };
            }

            for (int l = 0; l < AchievementManager.achievementIdentifiers.Count; l++)
            {
                string currentAchievementIdentifier = AchievementManager.achievementIdentifiers[l];
                AchievementManager.achievementNamesToDefs[currentAchievementIdentifier].childAchievementIdentifiers = AchievementManager.achievementIdentifiers.Where((string v) => AchievementManager.achievementNamesToDefs[v].prerequisiteAchievementIdentifier == currentAchievementIdentifier).ToArray();
            }
            AnointedSkinsUnlockables = null;
        }

        private static void AddAnointedOverlay(On.RoR2.SurvivorMannequins.SurvivorMannequinSlotController.orig_ApplyLoadoutToMannequinInstance orig, RoR2.SurvivorMannequins.SurvivorMannequinSlotController self)
        {
            orig(self);

            if (self && self.currentSurvivorDef && self.currentSurvivorDef.survivorIndex != SurvivorIndex.None)
            {
                BodyIndex bodyIndexFromSurvivorIndex = SurvivorCatalog.GetBodyIndexFromSurvivorIndex(self.currentSurvivorDef.survivorIndex);
                int skinIndex = (int)self.currentLoadout.bodyLoadoutManager.GetSkinIndex(bodyIndexFromSurvivorIndex);
                SkinDef safe = ArrayUtils.GetSafe(SkinCatalog.GetBodySkinDefs(bodyIndexFromSurvivorIndex), skinIndex);
                CharacterModel characterModel = self.mannequinInstanceTransform.GetComponentInChildren<CharacterModel>();
                if (characterModel)
                {
                    if (AnointedSkinsOverlayHashSet.Contains(safe))
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

        private static void HideHiddenSkinDefs(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            ILLabel lable = null;
            var jumpMatch = c.TryGotoNext(MoveType.After,
                x => x.MatchBrfalse(out lable));

            if (jumpMatch)
            {
                c.Index -= 6;

                c.Emit(OpCodes.Ldarg_0);
                c.Emit(OpCodes.Ldloc, 5);
                c.EmitDelegate<Func<RoR2.UI.LoadoutPanelController, SkinDef, bool>>(CheckForSpecialSkinDef);
                c.Emit(OpCodes.Brtrue, lable.Target);
            }
            else
            {
                Log.Warning("IL Hook Failed - RoR2.UI.LoadoutPanelController.Row.FromSkin: Anointed skins won't be hidden in the lobby until they are unlocked.");
            }

            static bool CheckForSpecialSkinDef(RoR2.UI.LoadoutPanelController owner, SkinDef skinDef)
            {
                if (!skinDef.unlockableDef)
                {
                    return false;
                }

                if (owner == null)
                {
                    return false;
                }

                UserProfile obj = owner.currentDisplayData.userProfile;
                if (obj == null)
                {
                    return false;
                }

                if (obj.HasUnlockable(skinDef.unlockableDef))
                {
                    return false;
                }
                if (skinDef is HiddenSkinDef)
                {
                    return (skinDef as HiddenSkinDef).hideInLobby;
                }
                return false;
            }
        }

        private static void AddAnointedItem(CharacterBody body)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!body.isPlayerControlled)
            {
                return;
            }

            if (body.inventory.GetItemCount(Content.Items.HiddenAnointed) > 0)
            {
                return;
            }

            if (body.modelLocator && body.modelLocator.modelTransform)
            {
                var modelSkinController = body.modelLocator.modelTransform.GetComponent<ModelSkinController>();
                if (modelSkinController)
                {
                    if (body.skinIndex < modelSkinController.skins.Length)
                    {
                        var skin = modelSkinController.skins[body.skinIndex];
                        if (AnointedSkinsItemHashSet.Contains(skin))
                        {
                            body.inventory.GiveItem(Content.Items.HiddenAnointed);
                            return;
                        };
                    }
                }
            }
        }
    }
}
