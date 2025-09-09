using EnemiesReturns.Components;
using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2BepInExPack.Utilities;
using System;
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
            var icon = AnointedSkinIcon;

            List<SkinDef> anointedSkins = new List<SkinDef>();
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
                    }
                    else
                    {
                        Log.Warning($"Body {body.name} has an empty material on baseRendererInfos at index {k}.");
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

                var eliteSkinDef = Utils.CreateHiddenSkinDef($"skin{body.name}EnemiesReturnsAnointed", model.gameObject, skinRenderInfos, true, defaultSkin);
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

                anointedSkins.Add(eliteSkinDef);
            }
            foreach (var skin in anointedSkins)
            {
                AnointedSkinsOverlayHashSet.Add(skin);
                AnointedSkinsItemHashSet.Add(skin);
            }
            foreach(var unlockable in skinUnlockables)
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
