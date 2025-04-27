using EnemiesReturns.Components;
using EnemiesReturns.Enemies.Judgement;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ArraignSkinExample
{
    public static class EnemiesReturnsModCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(EnemiesReturns.EnemiesReturnsPlugin.GUID);
                }
                return (bool)_enabled;
            }
        }

        public static bool AddBodyToBlacklist(string body)
        {
            return EnemiesReturns.Enemies.Judgement.SetupJudgementPath.AddBodyToBlacklist(body);
        }

        public static SkinDef CreateHiddenSkinDef()
        {
            return ScriptableObject.CreateInstance<HiddenSkinDef>();
        }

        public static UnlockableDef CreateUnlockable(string body, Sprite icon)
        {
            // this makes your skin respect ForceUnlock config in EnemiesReturns
            // if you don't want that - remove\comment these 3 lines
            if (EnemiesReturns.Configuration.Judgement.ForceUnlock.Value)
            {
                return null;
            }

            var skinUnlockDef = ScriptableObject.CreateInstance<UnlockableDef>();
            (skinUnlockDef as ScriptableObject).name = $"Skins.{body}.ArraignSkinExample";
            skinUnlockDef.cachedName = $"Skins.{body}.ArraignSkinExample";
            skinUnlockDef.nameToken = $"ARRAIGN_SKIN_EXAMPLE_SKIN_{body.ToUpper()}_NAME";
            skinUnlockDef.hidden = false;
            skinUnlockDef.achievementIcon = icon;

            return skinUnlockDef;
        }

        public static void CreateAchievement(UnlockableDef unlockable, string bodyName)
        {
            // this makes your skin respect ForceUnlock config in EnemiesReturns
            // if you don't want that - remove\comment these 3 lines
            if (EnemiesReturns.Configuration.Judgement.ForceUnlock.Value)
            {
                return;
            }

            if (!unlockable)
            {
                return;
            }

            AchievementDef cheevoDef = new AchievementDef
            {
                identifier = $"ArraignSkinExample{bodyName}JudgementCleared",
                unlockableRewardIdentifier = unlockable.cachedName,
                prerequisiteAchievementIdentifier = null,
                nameToken = "ARRAIGN_SKIN_EXAMPLE_" + (bodyName + "JudgementCleared").ToUpper() + "_NAME",
                descriptionToken = "ARRAIGN_SKIN_EXAMPLE_" + (bodyName + "JudgementCleared").ToUpper() + "_DESC",
                type = typeof(Bandit2JudgementClearedAchievement),
                serverTrackerType = typeof(EnemiesReturns.Achievements.JudgementClearedAchievement.JudgementClearedServerAchievement),
                lunarCoinReward = 10u
            };

            if (unlockable.achievementIcon)
            {
                cheevoDef.SetAchievedIcon(unlockable.achievementIcon);
            }

            AchievementManager.achievementIdentifiers.Add(cheevoDef.identifier);
            AchievementManager.achievementNamesToDefs.Add(cheevoDef.identifier, cheevoDef);
            HG.ArrayUtils.ArrayAppend(ref AchievementManager.achievementDefs, cheevoDef);

            if (unlockable)
            {
                // you can change ENEMIES_RETURNS strings here to whatever you want,
                // but since all strings are pretty much the same there is no real reason to do that
                unlockable.getHowToUnlockString = () => RoR2.Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", "???", "???");
                unlockable.getUnlockedString = () => RoR2.Language.GetStringFormatted("UNLOCKED_FORMAT", RoR2.Language.GetString("ENEMIES_RETURNS_JUDGEMENT_SKIN_ANOINTED_NAME"), RoR2.Language.GetString("ENEMIES_RETURNS_JUDGEMENT_ACHIEVEMENT_SURVIVE_JUDGEMENT_DESC"));
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
        }
    }
}
