﻿using BepInEx.Configuration;
using R2API;
using UnityEngine;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxArcher : IConfiguration
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

        public static ConfigEntry<float> FireArrowCooldown;
        public static ConfigEntry<float> FireArrowDamage;
        public static ConfigEntry<float> FireArrowProcCoefficient;
        public static ConfigEntry<float> FireArrowTimeToTarget;
        public static ConfigEntry<float> FireArrowForce;

        public static ConfigEntry<KeyCode> SingEmoteKey;

        public void PopulateConfig(ConfigFile config)
        {
            SelectionWeight = config.Bind("Lynx Archer Director", "Selection Weight", 1, "Selection weight of Lynx Archer.");
            DirectorCost = config.Bind("Lynx Archer Director", "Director Cost", 27, "Director cost of Lynx Archer.");
            MinimumStageCompletion = config.Bind("Lynx Archer Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DefaultStageList = config.Bind("Lynx Archer Director", "Default Variant Stage List",
                string.Join(",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    "snowtime_gmconstruct",
                    "snowtime_gmflatgrass"
                ),
                "Stages that Default Lynx Archer appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Lynx Archer Character Stats", "Base Max Health", 140f, "Lynx Archer' base health.");
            BaseMoveSpeed = config.Bind("Lynx Archer Character Stats", "Base Movement Speed", 7f, "Lynx Archer' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Archer Character Stats", "Base Jump Power", 18f, "Lynx Archer' base jump power.");
            BaseDamage = config.Bind("Lynx Archer Character Stats", "Base Damage", 12f, "Lynx Archer' base damage.");
            BaseArmor = config.Bind("Lynx Archer Character Stats", "Base Armor", 0f, "Lynx Archer' base armor.");

            LevelMaxHealth = config.Bind("Lynx Archer Character Stats", "Health per Level", 42f, "Lynx Archer' health increase per level.");
            LevelDamage = config.Bind("Lynx Archer Character Stats", "Damage per Level", 2.4f, "Lynx Archer' damage increase per level.");
            LevelArmor = config.Bind("Lynx Archer Character Stats", "Armor per Level", 0f, "Lynx Archer' armor increase per level.");

            FireArrowCooldown = config.Bind("Lynx Archer Fire Arrow", "Fire Arrow Cooldown", 0f, "Lynx Archer's Fire Arrow cooldown.");
            FireArrowDamage = config.Bind("Lynx Archer Fire Arrow", "Fire Arrow Damage", 1.666f, "Lynx Archer's Fire Arrow damage.");
            FireArrowProcCoefficient = config.Bind("Lynx Archer Fire Arrow", "Fire Arrow Proc Coefficient", 1f, "Lynx Archer's Fire Arrow proc coefficient.");
            FireArrowTimeToTarget = config.Bind("Lynx Archer Fire Arrow", "Fire Arrow Time To Target", 0.8f, "Lynx Archer's Fire Arrow how long it takes for projectile to reach the target in seconds, basically controls how fast the projectile.");
            FireArrowForce = config.Bind("Lynx Archer Fire Arrow", "Fire Arrow Projectile Force", 100f, "Lynx Archer's Fire Arrow force.");

            SingEmoteKey = config.Bind("Lynx Archer Emotes", "Sing Emote", KeyCode.Alpha1, "Key used to Sing.");
        }
    }
}
