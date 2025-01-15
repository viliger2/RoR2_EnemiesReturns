using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public static class LynxShaman
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

        public static ConfigEntry<float> SummonProjectilesCooldown;
        public static ConfigEntry<float> SummonProjectilesDamage;
        public static ConfigEntry<float> SummonProjectileProcCoefficient;
        public static ConfigEntry<int> SummonProjectilesCount;
        public static ConfigEntry<float> SummonProjectilesDebuffDuration;
        public static ConfigEntry<float> SummonProjectilesHealingFraction;
        public static ConfigEntry<float> SummonProjectilesLifetime;
        public static ConfigEntry<float> SummonProjectilesSpeed;
        public static ConfigEntry<float> SummonProjectilesTurnSpeed;

        public static ConfigEntry<float> PushBackCooldown;
        public static ConfigEntry<float> PushBackDamage;
        public static ConfigEntry<float> PushBackProcCoefficient;
        public static ConfigEntry<float> PushBackForce;
        public static ConfigEntry<float> PushBackRadius;

        public static ConfigEntry<KeyCode> NopeEmoteKey;
        public static ConfigEntry<KeyCode> SingEmoteKey;

        public static void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Lynx Shaman Director", "Enable Lynx Shaman", true, "Enables Lynx Shaman.");
            SelectionWeight = config.Bind("Lynx Shaman Director", "Selection Weight", 1, "Selection weight of Lynx Shaman.");
            MinimumStageCompletion = config.Bind("Lynx Shaman Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            DirectorCost = config.Bind("Lynx Shaman Director", "Director Cost", 40, "Director cost of Lynx Shaman.");
            DefaultStageList = config.Bind("Lynx Totem Director", "Default Variant Stage List",
                string.Join(",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlains),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DistantRoost),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ShatteredAbodes),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DisturbedImpact),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.WetlandAspect),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TreebornColony),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.GoldenDieback),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SunderedGrove),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadow),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),

                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.TitanicPlainsSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SkyMeadowSimulacrum)
                ),
                "Stages that Default Lynx Totem appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Lynx Shaman Character Stats", "Base Max Health", 300f, "Lynx Shaman' base health.");
            BaseMoveSpeed = config.Bind("Lynx Shaman Character Stats", "Base Movement Speed", 6f, "Lynx Shaman' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Shaman Character Stats", "Base Jump Power", 14f, "Lynx Shaman' base jump power.");
            BaseDamage = config.Bind("Lynx Shaman Character Stats", "Base Damage", 20f, "Lynx Shaman' base damage.");
            BaseArmor = config.Bind("Lynx Shaman Character Stats", "Base Armor", 0f, "Lynx Shaman' base armor.");

            LevelMaxHealth = config.Bind("Lynx Shaman Character Stats", "Health per Level", 90f, "Lynx Shaman' health increase per level.");
            LevelDamage = config.Bind("Lynx Shaman Character Stats", "Damage per Level", 4f, "Lynx Shaman' damage increase per level.");
            LevelArmor = config.Bind("Lynx Shaman Character Stats", "Armor per Level", 0f, "Lynx Shaman' armor increase per level.");

            SummonProjectilesCooldown = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Cooldown", 8f, "Lynx Shaman's Summon Projectiles cooldown.");
            SummonProjectilesDamage = config.Bind("Lynx Shaman Summon Projectiles", "Lunx Shaman Summon Projectiles Damage", 1f, "Lynx Shaman's Summon Projectiles damage coefficient.");
            SummonProjectileProcCoefficient = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Proc Coefficient", 1f, "Lynx Shaman's Summon Projectiles proc coefficient.");
            SummonProjectilesCount = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Count", 5, "Lynx Shaman's Summon Projectiles count.");
            SummonProjectilesDebuffDuration = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Debuff Duration", 3f, "Lynx Shaman's Summon Projectiles minus healing debuff duration.");
            SummonProjectilesHealingFraction = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Healing Debuff Fraction", 0.25f, "Lynx Shaman's Summon Projectiles healing reduction.");
            SummonProjectilesLifetime = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Lifetime", 7f, "Lynx Shaman's Summon Projectiles lifetime of projectiles.");
            SummonProjectilesSpeed = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Speed", 8.5f, "Lynx Shaman's Summon Projectiles speed of projectiles.");
            SummonProjectilesTurnSpeed = config.Bind("Lynx Shaman Summon Projectiles", "Lynx Shaman Summon Projectiles Turn Speed", 90f, "Lynx Shaman's Summon Projectiles turn speed of projectiles.");

            PushBackCooldown = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Cooldown", 4f, "Lynx Shaman's Push Back Cooldown.");
            PushBackDamage = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Damage", 1f, "Lynx Shaman's Push Back damage coefficient.");
            PushBackProcCoefficient = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Proc Coefficient", 1f, "Lynx Shaman's Push Back proc coefficient.");
            PushBackForce = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Force", 2000f, "Lynx Shaman's Push Back force.");
            PushBackRadius = config.Bind("Lynx Shaman Push Back", "Lynx Shaman Push Back Radius", 6f, "Lynx Shaman's Push Back attack radius.");

            NopeEmoteKey = config.Bind("Lynx Shaman Emotes", "Nope Emote", KeyCode.Alpha2, "Key used to Nope.");
            SingEmoteKey = config.Bind("Lynx Shaman Emotes", "Sing Emote", KeyCode.Alpha1, "Key used to Sing.");
        }
    }
}
