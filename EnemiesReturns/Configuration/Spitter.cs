using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public static class Spitter
    {
        public static ConfigEntry<bool> Enabled;
        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;
        public static ConfigEntry<bool> HelminthroostReplaceMushrum;

        public static ConfigEntry<string> DefaultStageList;
        public static ConfigEntry<string> DepthStageList;
        public static ConfigEntry<string> LakesStageList;
        public static ConfigEntry<string> SulfurStageList;

        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseJumpPower;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> BiteCooldown;
        public static ConfigEntry<float> BiteDamageModifier;
        public static ConfigEntry<float> BiteDamageForce;

        public static ConfigEntry<float> ChargedProjectileCooldown;
        public static ConfigEntry<float> ChargedProjectileDamage;
        public static ConfigEntry<float> ChargedProjectileForce;
        public static ConfigEntry<float> ChargedProjectileLargeDoTZoneDamage;
        public static ConfigEntry<float> ChargedProjectileLargeDoTZoneScale;
        public static ConfigEntry<float> ChargedProjectileFlyTime;
        public static ConfigEntry<float> ChargedProjectileSmallDoTZoneDamage;
        public static ConfigEntry<float> ChargedProjectileSmallDoTZoneScale;

        public static ConfigEntry<KeyCode> EmoteKey;

        public static void PopulateConfig(ConfigFile config)
        {
            #region Spitter
            Spitter.Enabled = config.Bind("Spitter Director", "Enable Spitter", true, "Enables Spitter.");
            Spitter.SelectionWeight = config.Bind("Spitter Director", "Selection Weight", 1, "Selection weight of Spitter.");
            Spitter.MinimumStageCompletion = config.Bind("Spitter Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            Spitter.DirectorCost = config.Bind("Spitter Director", "Director Cost", 30, "Director cost of Spitter.");
            Spitter.HelminthroostReplaceMushrum = config.Bind("Spitter Director", "Replace Mini Mushrum On Helminth Hatchery", true, "Spitter replaces Mini Mushrum on Helminth Hatchery");

            Spitter.DefaultStageList = config.Bind("Spitter Director", "Default Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.WetlandAspect),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VoidCell),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ArtifactReliquary),
                    "FBLScene",
                    "agatevillage"
                ),
                "Stages that Default Spitter appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Spitter.LakesStageList = config.Bind("Spitter Director", "Lakes Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.VerdantFalls)
                ),
                "Stages that Lakes Spitter appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Spitter.SulfurStageList = config.Bind("Spitter Director", "Sulfur Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SulfurPools)
                ),
                "Stages that Sulfur Spitter appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");
            Spitter.DepthStageList = config.Bind("Spitter Director", "Depth Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbyssalDepths),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.AbyssalDepthsSimulacrum),
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.HelminthHatchery)
                ),
                "Stages that Depth Spitter appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            Spitter.BaseMaxHealth = config.Bind("Spitter Character Stats", "Base Max Health", 300f, "Spitter's base health.");
            Spitter.BaseMoveSpeed = config.Bind("Spitter Character Stats", "Base Movement Speed", 7f, "Spitter's base movement speed.");
            Spitter.BaseJumpPower = config.Bind("Spitter Character Stats", "Base Jump Power", 20f, "Spitter's base jump power.");
            Spitter.BaseDamage = config.Bind("Spitter Character Stats", "Base Damage", 20f, "Spitter's base damage.");
            Spitter.BaseArmor = config.Bind("Spitter Character Stats", "Base Armor", 0f, "Spitter's base armor.");

            Spitter.LevelMaxHealth = config.Bind("Spitter Character Stats", "Health per Level", 90f, "Spitter's health increase per level.");
            Spitter.LevelDamage = config.Bind("Spitter Character Stats", "Damage per Level", 4f, "Spitter's damage increase per level.");
            Spitter.LevelArmor = config.Bind("Spitter Character Stats", "Armor per Level", 0f, "Spitter's armor increase per level.");

            Spitter.BiteCooldown = config.Bind("Spitter Bite", "Bite Cooldown", 2f, "Spitter's Bite cooldown.");
            Spitter.BiteDamageModifier = config.Bind("Spitter Bite", "Bite Damage", 1.5f, "Spitter's Bite damage.");
            Spitter.BiteDamageForce = config.Bind("Spitter Bite", "Bite Force", 200f, "Spitter's Bite force, by default equal to that of Lemurian.");

            Spitter.ChargedProjectileCooldown = config.Bind("Spitter Charged Spit", "Charged Spit Cooldown", 6f, "Charged Spit Cooldown");
            Spitter.ChargedProjectileDamage = config.Bind("Spitter Charged Spit", "Charged Spit Damage", 1.6f, "Spitter's Charged projectile damage.");
            Spitter.ChargedProjectileForce = config.Bind("Spitter Charged Spit", "Charged Spit Force", 0f, "Spitter's Charged projectile force.");
            Spitter.ChargedProjectileFlyTime = config.Bind("Spitter Charged Spit", "Charged Spit Fly Time", 0.75f, "Spitter's Charged Projectile fly time. The higher the value the bigger the arc and the slower the projectile will be. Don't set it too low otherwise projectile will fly pass targes and puddles will be somewhere in the back.");

            Spitter.ChargedProjectileLargeDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileLargeDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Size Scale", 0.55f, "Spitter's Charged Large DoT Zone size scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");
            Spitter.ChargedProjectileSmallDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileSmallDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Scale", 0.3f, "Spitter's Charged Large DoT Zone scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");

            Spitter.EmoteKey = config.Bind("Spitter Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");
            #endregion

        }

    }
}
