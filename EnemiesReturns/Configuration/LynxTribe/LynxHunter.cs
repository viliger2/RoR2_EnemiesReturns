using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxHunter
    {
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;

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

        public static void PopulateConfig(ConfigFile config)
        {
            SelectionWeight = config.Bind("Lynx Hunter Director", "Selection Weight", 1, "Selection weight of Lynx Hunter.");
            DirectorCost = config.Bind("Lynx Hunter Director", "Director Cost", 28, "Director cost of Lynx Hunter.");

            BaseMaxHealth = config.Bind("Lynx Hunter Character Stats", "Base Max Health", 180f, "Lynx Hunter' base health.");
            BaseMoveSpeed = config.Bind("Lynx Hunter Character Stats", "Base Movement Speed", 7f, "Lynx Hunter' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Hunter Character Stats", "Base Jump Power", 18f, "Lynx Hunter' base jump power.");
            BaseDamage = config.Bind("Lynx Hunter Character Stats", "Base Damage", 12f, "Lynx Hunter' base damage.");
            BaseArmor = config.Bind("Lynx Hunter Character Stats", "Base Armor", 0f, "Lynx Hunter' base armor.");

            LevelMaxHealth = config.Bind("Lynx Hunter Character Stats", "Health per Level", 54f, "Lynx Hunter' health increase per level.");
            LevelDamage = config.Bind("Lynx Hunter Character Stats", "Damage per Level", 2.4f, "Lynx Hunter' damage increase per level.");
            LevelArmor = config.Bind("Lynx Hunter Character Stats", "Armor per Level", 0f, "Lynx Hunter' armor increase per level.");

            StabCooldown = config.Bind("Lynx Hunter Stab", "Stab Cooldown", 3f, "Lynx Hunter's Stab cooldown.");
            StabDamage = config.Bind("Lynx Hunter Stab", "Stab Damage", 3f, "Lynx Hunter's Stab damage.");
            StabProcCoefficient = config.Bind("Lynx Hunter Stab", "Stab Proc Coefficient", 1f, "Lynx Hunter's Stab proc coefficient.");

            SingEmoteKey = config.Bind("Lynx Hunter Emotes", "Sing Emote", KeyCode.Alpha1, "Key used to Sing.");
        }
    }
}
