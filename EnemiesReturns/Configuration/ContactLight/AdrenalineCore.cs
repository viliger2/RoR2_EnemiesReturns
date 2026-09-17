using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight
{
    public class AdrenalineCore : IConfiguration
    {
        public static ConfigEntry<bool> EnableUI;

        public static ConfigEntry<bool> TransendanceSupport;

        public static ConfigEntry<int> MaxLevel;
        public static ConfigEntry<int> MaxLevelPerStack;
        public static ConfigEntry<float> PointPerLevelReduction;
        public static ConfigEntry<float> ChampionReward;
        public static ConfigEntry<float> NormalReward;
        public static ConfigEntry<float> T1EliteModifier;
        public static ConfigEntry<float> T2EliteModifier;
        public static ConfigEntry<float> PointsPerLevel;

        public static ConfigEntry<float> AttackSpeed;
        public static ConfigEntry<float> MovementSpeed;
        public static ConfigEntry<float> CritChance;
        public static ConfigEntry<float> CritDamage;

        public static ConfigEntry<float> HealthFreqCheck;
        public static ConfigEntry<float> HealthThreshold;

        public void PopulateConfig(ConfigFile config)
        {
            EnableUI = config.Bind("Main Functionality", "Enable UI", true, "Enables UI (level bar under health bar) of Adrenaline Core.");
            TransendanceSupport = config.Bind("Main Functionality", "Transendance Support", true, "Transendance support. Swaps HP check from health to shields if player has Transendance.");

            MaxLevel = config.Bind("Leveling", "Max Level", 5, "Default max level. Also level at which crit bonuses are given.");
            MaxLevelPerStack = config.Bind("Leveling", "Max Level Per Stack", 2, "Additional levels per item stack");
            PointsPerLevel = config.Bind("Leveling", "Points Per Level", 25f, "Number of points required for leveling up.");
            PointPerLevelReduction = config.Bind("Leveling", "Poins Per Level Reduction Per Stack", 10f, "Percent value of points needed per level per item stack. Stacks hyperbolically.");

            ChampionReward = config.Bind("Leveling", "Champion Monster Reward", 5f, "How many points champion monster rewards.");
            NormalReward = config.Bind("Leveling", "Normal Monster Reward", 1f, "How many points normal monster rewards.");
            T1EliteModifier = config.Bind("Leveling", "Tier 1 Elite Modifier", 2f, "Multiplier that is added to other point rewards if killed monster is Tier 1 elite.");
            T2EliteModifier = config.Bind("Leveling", "Tier 2 Elite Modifier", 3f, "Multiplier that is added to other point rewards if killed monster is Tier 2 elite.");

            AttackSpeed = config.Bind("Stats", "Attack Speed", 8f, "Attack speed bonus per level.");
            MovementSpeed = config.Bind("Stats", "Movement Speed", 6f, "Movement speed bonus per level.");
            CritChance = config.Bind("Stats", "Critical Chance", 20f, "Critical chance bonus at MaxLevel.");
            CritDamage = config.Bind("Stats", "Critical Damage", 20f, "Critical damage bonus at MaxLevel.");

            HealthFreqCheck = config.Bind("Deleveling", "Health Frequency Check", 0.1f, "How frequently, in seconds, game checks for health updates. Lower values might decrease performance, higher values will lump damage taken over the course of the value together.");
            HealthThreshold = config.Bind("Deleveling", "Health Threshold", 10f, "Health threshold, taking this much percent health in HealthFrequencyCheck time will delevel the user.");

        }

    }
}
