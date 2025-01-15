using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxStuff 
    {
        public static ConfigEntry<bool> LynxShrineEnabled;

        public static ConfigEntry<int> LynxShrineDirectorCost;
        public static ConfigEntry<int> LynxShrineSelectionWeight;
        public static ConfigEntry<DirectorAPI.InteractableCategory> LynxShrineSpawnCategory;

        public static ConfigEntry<bool> LynxTrapEnabled;

        public static ConfigEntry<int> LynxTrapDirectorCost;
        public static ConfigEntry<int> LynxTrapSelectionWeight;
        public static ConfigEntry<DirectorAPI.InteractableCategory> LynxTrapSpawnCategory;
        public static ConfigEntry<int> LynxTrapMaxSpawnPerStage;

        public static ConfigEntry<float> LynxTrapEliteBias;
        public static ConfigEntry<int> LynxTrapMinSpawnCount;
        public static ConfigEntry<int> LynxTrapMaxSpawnCount;
        public static ConfigEntry<bool> LynxTrapAssignRewards;
        public static ConfigEntry<float> LynxTrapCheckInterval;

        public static void PopulateConfig(ConfigFile config)
        {
            LynxShrineEnabled = config.Bind("Lynx Shrine Spawn", "Enable Lynx Shrine", true, "Enables Lynx Shrine. Has no effect if Lynx Totem is disabled.");
            LynxShrineDirectorCost = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Director Cost", 20, "Lynx Shrine's director cost. The same as other shrines by default.");
            LynxShrineSelectionWeight = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Selection Weight", 2, "Lynx Shrine's selection weight. The same as Combat shrine by default.");
            LynxShrineSpawnCategory = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Spawn Category", DirectorAPI.InteractableCategory.Shrines, "Lynx Shrine's spawn category.");

            LynxTrapEnabled = config.Bind("Lynx Trap Spawn", "Enable Lynx Trap", true, "Enables Lynx Trap. Has no effect is Lynx Totem is disabled.");
            LynxTrapDirectorCost = config.Bind("Lynx Trap Spawn", "Lynx Trap Director Cost", 2, "Lynx Trap's director cost. The same as other shrines by default.");
            LynxTrapSelectionWeight = config.Bind("Lynx Trap Spawn", "Lynx Trap Selection Weight", 1, "Lynx Trap's selection weight. The same as Combat shrine by default.");
            LynxTrapSpawnCategory = config.Bind("Lynx Trap Spawn", "Lynx Trap Spawn Category", DirectorAPI.InteractableCategory.Barrels, "Lynx Trap's spawn category.");
            LynxTrapMaxSpawnPerStage = config.Bind("Lynx Trap Spawn", "Lynx Trap Max Spawn Per Stage", 3, "Max spawns of Lynx Trap per stage.");

            LynxTrapEliteBias = config.Bind("Lynx Trap Spawns", "Lynx Trap Elite Bias", 1f, "Controls elite bias, basically the lower the value the cheaper are elites when their credit cost is calculated. tldr the lower the value the more elites spawned.");
            LynxTrapMinSpawnCount = config.Bind("Lynx Trap Spawns", "Lynx Trap Min Spawn Count", 3, "Minimum number of enemies that get spawned once trap is triggered.");
            LynxTrapMaxSpawnCount = config.Bind("Lynx Trap Spawns", "Lynx Trap Man Spawn Count", 5, "Maximum number of enemies that get spawned once trap is triggered.");
            LynxTrapAssignRewards = config.Bind("Lynx Trap Spawns", "Lynx Trap Assign Rewards", true, "Whether or not enemies spawned by trap reward gold or exp.");
            LynxTrapCheckInterval = config.Bind("Lynx Trap Spawns", "Lynx Trap Check Interval", 0.15f, "How frequently game checks for trap collision. Lower values give better collision but worse performance.");
        }
    }
}
