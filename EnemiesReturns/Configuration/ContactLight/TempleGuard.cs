using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration.ContactLight
{
    public class TempleGuard : IConfiguration
    {
        public static ConfigEntry<KeyCode> EmoteKey;

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

        public static ConfigEntry<float> BarrageCooldown;
        public static ConfigEntry<float> BarrageChargeDuration;
        public static ConfigEntry<float> BarragePerShotCooldown;
        public static ConfigEntry<int> BarrageNumberOfShots;
        public static ConfigEntry<float> BarrageSpreadBloom;
        public static ConfigEntry<float> BarrageProjectileDamage;
        public static ConfigEntry<float> BarrageProjectileSpeed;
        public static ConfigEntry<float> BarrageCorrectionAngle;

        public static ConfigEntry<float> OverclockCooldown;
        public static ConfigEntry<float> OverclockBuffDuration;
        public static ConfigEntry<float> OverclockAttackSpeedBuff;
        public static ConfigEntry<float> OverclockPrimaryCooldownReduction;
        public static ConfigEntry<bool> OverclockRestockPrimary;

        public void PopulateConfig(ConfigFile config)
        {
            EmoteKey = config.Bind("Temple Guard Emotes", "Stretch Emote", KeyCode.Alpha1, "Key used to Stretch.");

            SelectionWeight = config.Bind("Temple Guard Director", "Selection Weight", 1, "Selection weight of Temple Guard.");
            MinimumStageCompletion = config.Bind("Temple Guard Director", "Minimum Stage Completion", 3, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Temple Guard Director", "Director Cost", 350, "Director cost of Temple Guard.");

            DefaultStageList = config.Bind("Temple Guard Director", "Default Variant Stage List",
                string.Join(
                    ",",
                    ""
                ),
                "Stages that Default Temple Guard appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command. Contact Light is included by default and is not removable.");

            BaseMaxHealth = config.Bind("Temple Guard Character Stats", "Base Max Health", 1615f, "Temple Guard' base health.");
            BaseMoveSpeed = config.Bind("Temple Guard Character Stats", "Base Movement Speed", 8f, "Temple Guard' base movement speed.");
            BaseJumpPower = config.Bind("Temple Guard Character Stats", "Base Jump Power", 20f, "Temple Guard' base jump power.");
            BaseDamage = config.Bind("Temple Guard Character Stats", "Base Damage", 35f, "Temple Guard' base damage.");
            BaseArmor = config.Bind("Temple Guard Character Stats", "Base Armor", 0f, "Temple Guard' base armor.");

            LevelMaxHealth = config.Bind("Temple Guard Character Stats", "Health per Level", 485f, "Temple Guard' health increase per level.");
            LevelDamage = config.Bind("Temple Guard Character Stats", "Damage per Level", 7f, "Temple Guard' damage increase per level.");
            LevelArmor = config.Bind("Temple Guard Character Stats", "Armor per Level", 0f, "Temple Guard' armor increase per level.");

            BarrageCooldown = config.Bind("Temple Guard Barrage", "Barrage Cooldown", 10f, "Temple Guard's Barrage cooldown.");
            BarrageChargeDuration = config.Bind("Temple Guard Barrage", "Barrage Charge Duration", 2f, "Pre-fire charge duration of Temple Guard's Barrage skill.");
            BarragePerShotCooldown = config.Bind("Temple Guard Barrage", "Barrage Cooldown Between Shots", 0.5f, "Cooldown between Temple Guard's Barrage shots");
            BarrageNumberOfShots = config.Bind("Temple Guard Barrage", "Barrage Number of Shots", 4, "Number of Temple Guard's Barrage shots");
            BarrageSpreadBloom = config.Bind("Temple Guard Barrage", "Barrage Spread Bloom", 0.2f, "Spread bloom that is applied after each shot in Temple Guard's Barrage.");
            BarrageProjectileDamage = config.Bind("Temple Guard Barrage", "Barrage Projectile Damage", 1f, "Temple Guard's Barrage projectile damage.");
            BarrageProjectileSpeed = config.Bind("Temple Guard Barrage", "Barrage Projectile Speed", 75f, "Temple Guard's Barrage projectile speed.");
            BarrageCorrectionAngle = config.Bind("Temple Guard Barrage", "Barrage Correction Angle", 2.5f, "Temple Guard's Barrage correction angle. Used to aim projectiles more towards the centre between two projectiles, negative values spread them from each other instead.");

            OverclockCooldown = config.Bind("Temple Guard Overclock", "Overclock Cooldown", 30f, "Temple Guard's Overclock cooldown.");
            OverclockBuffDuration = config.Bind("Temple Guard Overclock", "Overclock Buff Duration", 15f, "Temple Guard's Overclock buff duration.");
            OverclockAttackSpeedBuff = config.Bind("Temple Guard Overclock", "Overclock Attack Speed Buff", 50f, "Temple Guard's Overclock attack speed buff.");
            OverclockPrimaryCooldownReduction = config.Bind("Temple Guard Overclock", "Overclock Primary Cooldown Reduction", 1f, "Temple Guard's Overclock primary cooldown redutction, 1 lowers cooldown by half, 2 lowers by 2/3, 1.5 lowers by 1/3, etc.");
            OverclockRestockPrimary = config.Bind("Temple Guard Overclock", "Overclock Restocks Primary", true, "Temple Guard's Overclock restocks Primary (Barrage) on use.");
        }
    }
}
