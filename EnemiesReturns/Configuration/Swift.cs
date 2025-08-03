using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public class Swift : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;
        public static ConfigEntry<bool> HelminthroostReplaceMushrum;

        public static ConfigEntry<string> DefaultStageList;

        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseJumpPower;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> DiveCooldown;
        public static ConfigEntry<float> DiveDamage;
        public static ConfigEntry<float> DiveMaxDuration;
        public static ConfigEntry<float> DiveTurnSpeed;
        public static ConfigEntry<float> DiveSpeedCoefficient;

        public static ConfigEntry<KeyCode> EmoteKey;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Swift Director", "Enable Swift", true, "Enables Swift.");
            SelectionWeight = config.Bind("Swift Director", "Selection Weight", 1, "Selection weight of Swift.");
            MinimumStageCompletion = config.Bind("Swift Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning."); // TODO: fix to 1 after gameplay test
            DirectorCost = config.Bind("Swift Director", "Director Cost", 32, "Director cost of Swift.");
            DefaultStageList = config.Bind("Swift Director", "Default Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DistantRoost),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDelta),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.RallypointDeltaSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.GildedCoast),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AphelianSanctuary),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AphelianSanctuarySimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SiphonedForest),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ArtifactReliquary),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ArtifactReliquary_AphelianSanctuary_Theme),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.PrimeMeridian),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),

                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ShatteredAbodes), // TODO: potentially remove
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DisturbedImpact)
                ),
                "Stages that Default Swift appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Swift Character Stats", "Base Max Health", 200f, "Swift's base health.");
            BaseMoveSpeed = config.Bind("Swift Character Stats", "Base Movement Speed", 7f, "Swift's base movement speed.");
            BaseJumpPower = config.Bind("Swift Character Stats", "Base Jump Power", 15f, "Swift's base jump power.");
            BaseDamage = config.Bind("Swift Character Stats", "Base Damage", 15f, "Swift's base damage.");
            BaseArmor = config.Bind("Swift Character Stats", "Base Armor", 0f, "Swift's base armor.");

            LevelMaxHealth = config.Bind("Swift Character Stats", "Health per Level", 60f, "Swift's health increase per level.");
            LevelDamage = config.Bind("Swift Character Stats", "Damage per Level", 3f, "Swift's damage increase per level.");
            LevelArmor = config.Bind("Swift Character Stats", "Armor per Level", 0f, "Swift's armor increase per level.");

            DiveCooldown = config.Bind("Swift Dive", "Dive Cooldown", 5f, "Swift's Dive cooldown.");
            DiveDamage = config.Bind("Swift Dive", "Dive Damage", 3f, "Swift's Dive damage.");
            DiveMaxDuration = config.Bind("Swift Dive", "Dive Maximum Duration", 3.5f, "Swift's Dive maximum duration after which it will stop diving if no targets or ground are hit.");
            DiveTurnSpeed = config.Bind("Swift Dive", "Dive Turn Speed", 200f, "Swift's Dive turn speed, the higher the value the faster it will turn following the aim vector.");
            DiveSpeedCoefficient = config.Bind("Swift Dive", "Dive Speed Coefficient", 9f, "Swift's Dive speed coefficient, multiplies base speed.");

            EmoteKey = config.Bind("Swift Emotes", "Duck Dance Emote", KeyCode.Alpha1, "Key used to do the Duck Dance.");
        }
    }
}
