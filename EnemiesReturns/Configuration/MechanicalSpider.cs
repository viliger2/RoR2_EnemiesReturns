using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public static class MechanicalSpider
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;

        public static ConfigEntry<string> DefaultStageList;

        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseJumpPower;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> DoubleShotCooldown;
        public static ConfigEntry<float> DoubleShotDamage;
        public static ConfigEntry<int> DoubleShotShots;
        public static ConfigEntry<float> DoubleShotDelayBetween;
        public static ConfigEntry<float> DoubleShotMinSpread;
        public static ConfigEntry<float> DoubleShotMaxSpread;

        public static ConfigEntry<float> DashCooldown;
        public static ConfigEntry<float> DashDuration;
        public static ConfigEntry<float> DashHeightCheck;

        public static ConfigEntry<float> DroneSpawnChance;
        public static ConfigEntry<int> DroneCost;
        public static ConfigEntry<float> DroneEliteConstMultiplier;
        public static ConfigEntry<float> DroneBaseRegen;
        public static ConfigEntry<float> DroneLevelRegen;

        public static void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Mechanical Spider Director", "Enable Mechanical Spider", true, "Enables Mechanical Spider.");
            SelectionWeight = config.Bind("Mechanical Spider Director", "Selection Weight", 1, "Selection weight of Mechanical Spider.");
            MinimumStageCompletion = config.Bind("Mechanical Spider Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Mechanical Spider Director", "Director Cost", 28, "Director cost of Mechanical Spider.");

            DefaultStageList = config.Bind("Mechanical Spider Director", "Default Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VerdantFalls),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ViscousFalls),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDelta),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SirensCall),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell)
                ),
                "Stages that Default Mechanical Spider appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Mechanical Spider Character Stats", "Base Max Health", 140f, "Mechanical Spider's base health.");
            BaseMoveSpeed = config.Bind("Mechanical Spider Character Stats", "Base Movement Speed", 9f, "Mechanical Spider's base movement speed.");
            BaseJumpPower = config.Bind("Mechanical Spider Character Stats", "Base Jump Power", 25f, "Mechanical Spider's base jump power.");
            BaseDamage = config.Bind("Mechanical Spider Character Stats", "Base Damage", 15f, "Mechanical Spider's base damage.");
            BaseArmor = config.Bind("Mechanical Spider Character Stats", "Base Armor", 0f, "Mechanical Spider's base armor.");

            LevelMaxHealth = config.Bind("Mechanical Spider Character Stats", "Health per Level", 42f, "Mechanical Spider's health increase per level.");
            LevelDamage = config.Bind("Mechanical Spider Character Stats", "Damage per Level", 3f, "Mechanical Spider's damage increase per level.");
            LevelArmor = config.Bind("Mechanical Spider Character Stats", "Armor per Level", 0f, "Mechanical Spider's armor increase per level.");

            DoubleShotCooldown = config.Bind("Mechanical Spider Double Shot", "Double Shot Cooldown", 2f, "Mechanical Spider's Double Shot cooldown.");
            DoubleShotDamage = config.Bind("Mechanical Spider Double Shot", "Double Shot Damage", 1f, "Mechanical Spider's Double Shot damage.");
            DoubleShotShots = config.Bind("Mechanical Spider Double Shot", "Double Shot Shots", 2, "Mechanical Spider's Double Shot number of shots, making it, surprisingly, not double.");
            DoubleShotDelayBetween = config.Bind("Mechanical Spider Double Shot", "Double Shot Delay Between Shots", 0.15f, "Mechanical Spider's Double Shot delay between shots. First shot always comes out instantly after charging state is done, each one after comes out with this delay.");

            DashCooldown = config.Bind("Mechanical Spider Dash", "Dash Cooldown", 5f, "Mechanical Spider's Dash cooldown.");
            DashDuration = config.Bind("Mechanical Spider Dash", "Dash Duration", 0.75f, "Mechanical Spider's Dash duration. Basically controls how far it will go.");
            DashHeightCheck = config.Bind("Mechanical Spider Dash", "Dash Height Check", 50f, "Checks for falls in front of Mechanical Spider and stops his so it wouldn't yeet itself off cliffs. Set it above 1000 to basically disable the functionality.");

            DroneSpawnChance = config.Bind("Mechanical Spider Drone", "Chance to Spawn Drone", 30f, "Chance to spawn purchasable Mechanical Spider on death.");
            DroneCost = config.Bind("Mechanical Spider Drone", "Drone Cost", 60, "Cost to repair broken Mechanical Spider.");
            DroneEliteConstMultiplier = config.Bind("Mechanical Spider Drone", "Elite Cost Multiplier", 0.5f, "Elite cost multiplier. Multiplies elite director cost to this value and then multiplies gold values to result. T1 elites are 6, T2 elites are 36, honor elites are half of those values.");
            DroneBaseRegen = config.Bind("Mechanical Spider Drone", "Base Regen", 5f, "Base health regeneration of allied Mechanical Spider.");
            DroneLevelRegen = config.Bind("Mechanical Spider Drone", "Regen Per Level", 1f, "Per level health regeneration of allied Mechanical Spider.");
        }
    }
}
