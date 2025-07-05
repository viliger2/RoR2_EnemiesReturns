﻿using BepInEx.Configuration;
using R2API;
using UnityEngine;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxHunter : IConfiguration
    {
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

        public static ConfigEntry<float> StabCooldown;
        public static ConfigEntry<float> StabDamage;
        public static ConfigEntry<float> StabProcCoefficient;

        public static ConfigEntry<KeyCode> SingEmoteKey;

        public void PopulateConfig(ConfigFile config)
        {
            SelectionWeight = config.Bind("Lynx Hunter Director", "Selection Weight", 1, "Selection weight of Lynx Hunter.");
            DirectorCost = config.Bind("Lynx Hunter Director", "Director Cost", 27, "Director cost of Lynx Hunter.");
            MinimumStageCompletion = config.Bind("Lynx Hunter Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DefaultStageList = config.Bind("Lynx Hunter Director", "Default Variant Stage List",
                string.Join(",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    "snowtime_gmconstruct",
                    "snowtime_gmflatgrass"
                ),
                "Stages that Default Lynx Hunter appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Lynx Hunter Character Stats", "Base Max Health", 140f, "Lynx Hunter' base health.");
            BaseMoveSpeed = config.Bind("Lynx Hunter Character Stats", "Base Movement Speed", 10f, "Lynx Hunter' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Hunter Character Stats", "Base Jump Power", 18f, "Lynx Hunter' base jump power.");
            BaseDamage = config.Bind("Lynx Hunter Character Stats", "Base Damage", 12f, "Lynx Hunter' base damage.");
            BaseArmor = config.Bind("Lynx Hunter Character Stats", "Base Armor", 0f, "Lynx Hunter' base armor.");

            LevelMaxHealth = config.Bind("Lynx Hunter Character Stats", "Health per Level", 54f, "Lynx Hunter' health increase per level.");
            LevelDamage = config.Bind("Lynx Hunter Character Stats", "Damage per Level", 2.4f, "Lynx Hunter' damage increase per level.");
            LevelArmor = config.Bind("Lynx Hunter Character Stats", "Armor per Level", 0f, "Lynx Hunter' armor increase per level.");

            StabCooldown = config.Bind("Lynx Hunter Stab", "Stab Cooldown", 3f, "Lynx Hunter's Stab cooldown.");
            StabDamage = config.Bind("Lynx Hunter Stab", "Stab Damage", 2.5f, "Lynx Hunter's Stab damage.");
            StabProcCoefficient = config.Bind("Lynx Hunter Stab", "Stab Proc Coefficient", 1f, "Lynx Hunter's Stab proc coefficient.");

            SingEmoteKey = config.Bind("Lynx Hunter Emotes", "Sing Emote", KeyCode.Alpha1, "Key used to Sing.");
        }
    }
}
