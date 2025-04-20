using BepInEx.Configuration;
using R2API;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxStuff : IConfiguration
    {
        public static ConfigEntry<bool> LynxShrineEnabled;

        public static ConfigEntry<int> LynxShrineDirectorCost;
        public static ConfigEntry<int> LynxShrineSelectionWeight;
        public static ConfigEntry<DirectorAPI.InteractableCategory> LynxShrineSpawnCategory;
        public static ConfigEntry<int> LynxShrineMaxSpawnPerStage;

        public static ConfigEntry<bool> LynxShrineMultiplayerScaling;
        public static ConfigEntry<float> LynxShrineEscapeTimer;
        public static ConfigEntry<float> LynxShrineTimerDisplayDistance;

        public static ConfigEntry<float> LynxShrineTier1Weight;
        public static ConfigEntry<int> LynxShrineTier1MinSpawns;
        public static ConfigEntry<int> LynxShrineTier1MaxSpawns;
        public static ConfigEntry<float> LynxShrineTier1EliteBias;

        public static ConfigEntry<float> LynxShrineTier2Weight;
        public static ConfigEntry<int> LynxShrineTier2MinSpawns;
        public static ConfigEntry<int> LynxShrineTier2MaxSpawns;
        public static ConfigEntry<float> LynxShrineTier2EliteBias;

        public static ConfigEntry<float> LynxShrineTier3Weight;
        public static ConfigEntry<int> LynxShrineTier3MinSpawns;
        public static ConfigEntry<int> LynxShrineTier3MaxSpawns;
        public static ConfigEntry<float> LynxShrineTier3EliteBias;

        public static ConfigEntry<float> LynxShrineTierBossWeight;
        public static ConfigEntry<int> LynxShrineTierBossMinSpawns;
        public static ConfigEntry<int> LynxShrineTierBossMaxSpawns;
        public static ConfigEntry<float> LynxShrineTierBossEliteBias;

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

        public void PopulateConfig(ConfigFile config)
        {
            LynxShrineEnabled = config.Bind("Lynx Shrine Spawn", "Enable Lynx Shrine", true, "Enables Lynx Shrine. Has no effect if Lynx Totem is disabled.");
            LynxShrineDirectorCost = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Director Cost", 20, "Lynx Shrine's director cost. The same as other shrines by default.");
            LynxShrineSelectionWeight = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Selection Weight", 2, "Lynx Shrine's selection weight. The same as Combat shrine by default.");
            LynxShrineSpawnCategory = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Spawn Category", DirectorAPI.InteractableCategory.Shrines, "Lynx Shrine's spawn category.");
            LynxShrineMaxSpawnPerStage = config.Bind("Lynx Shrine Spawn", "Lynx Shrine Max Spawn Per Stage", 2, "Max spawns of Lynx Shrine per stage.");

            LynxShrineMultiplayerScaling = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Multiplayer Scaling", false, "Enables multiplayer scaling for Lynx Shrine. Enabling this will drop items for every player in the lobby, but spawned enemies will have multiplayer health scaling.");
            LynxShrineEscapeTimer = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Escape Timer", 25f, "How much time players have to kill the enemies before they escape and shrine encounter is failed.");
            LynxShrineTimerDisplayDistance = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Timer Display Distance", 100f, "How far the timer renders when you walk away from the shrine.");

            LynxShrineTier1Weight = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 1 Weight", 0.55f, "Weight of Tier 1 items. The higher the value, the higher the chance of this tier being selected. Weight is relative to weight of other tiers.");
            LynxShrineTier1MinSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 1 Min Spawns", 2, "Minimum number of enemies that are spawned when Tier 1 item is selected.");
            LynxShrineTier1MaxSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 1 Max Spawns", 3, "Maximum number of enemies that are spawned when Tier 1 item is selected.");
            LynxShrineTier1EliteBias = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 1 Elite Bias", 1f, "Elite bias of Tier 1 item. Basically, the lower the value the cheaper elites are to spawn, which means there will be more of them.");

            LynxShrineTier2Weight = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 2 Weight", 0.3f, "Weight of Tier 2 items. The higher the value, the higher the chance of this tier being selected. Weight is relative to weight of other tiers.");
            LynxShrineTier2MinSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 2 Min Spawns", 3, "Minimum number of enemies that are spawned when Tier 2 item is selected.");
            LynxShrineTier2MaxSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 2 Max Spawns", 4, "Maximum number of enemies that are spawned when Tier 2 item is selected.");
            LynxShrineTier2EliteBias = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 2 Elite Bias", 0.75f, "Elite bias of Tier 2 item. Basically, the lower the value the cheaper elites are to spawn, which means there will be more of them.");

            LynxShrineTier3Weight = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 3 Weight", 0.05f, "Weight of Tier 3 items. The higher the value, the higher the chance of this tier being selected. Weight is relative to weight of other tiers.");
            LynxShrineTier3MinSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 3 Min Spawns", 5, "Minimum number of enemies that are spawned when Tier 3 item is selected.");
            LynxShrineTier3MaxSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 3 Max Spawns", 6, "Maximum number of enemies that are spawned when Tier 3 item is selected.");
            LynxShrineTier3EliteBias = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier 3 Elite Bias", 0.4f, "Elite bias of Tier 3 item. Basically, the lower the value the cheaper elites are to spawn, which means there will be more of them.");

            LynxShrineTierBossWeight = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier Boss Weight", 0.1f, "Weight of Boss Tier items. The higher the value, the higher the chance of this tier being selected. Weight is relative to weight of other tiers.");
            LynxShrineTierBossMinSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier Boss Min Spawns", 4, "Minimum number of enemies that are spawned when Boss Tier item is selected.");
            LynxShrineTierBossMaxSpawns = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier Boss Max Spawns", 5, "Maximum number of enemies that are spawned when Boss Tier item is selected.");
            LynxShrineTierBossEliteBias = config.Bind("Lynx Shrine Behaviour", "Lynx Shrine Tier Boss Elite Bias", 0.5f, "Elite bias of Boss Tier item. Basically, the lower the value the cheaper elites are to spawn, which means there will be more of them.");

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
