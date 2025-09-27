using BepInEx.Configuration;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public class SandCrab : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;

        public static ConfigEntry<int> DirectorCost;
        public static ConfigEntry<int> SelectionWeight;
        public static ConfigEntry<int> MinimumStageCompletion;

        public static ConfigEntry<string> DefaultStageList;
        public static ConfigEntry<string> GrassyStageList;
        public static ConfigEntry<string> SandyStateList;
        public static ConfigEntry<string> SulfurStageList;

        public static ConfigEntry<float> BaseMaxHealth;
        public static ConfigEntry<float> BaseMoveSpeed;
        public static ConfigEntry<float> BaseJumpPower;
        public static ConfigEntry<float> BaseDamage;
        public static ConfigEntry<float> BaseArmor;
        public static ConfigEntry<float> LevelMaxHealth;
        public static ConfigEntry<float> LevelDamage;
        public static ConfigEntry<float> LevelArmor;

        public static ConfigEntry<float> SnipCooldown;
        public static ConfigEntry<float> SnipHoldMaxDuration;
        public static ConfigEntry<float> SnipDamage;
        public static ConfigEntry<float> SnipForce;

        public static ConfigEntry<float> BubbleCooldown;
        public static ConfigEntry<float> BubbleDamage;
        public static ConfigEntry<float> BubbleProjectileSpread;
        public static ConfigEntry<float> BubbleSize;
        public static ConfigEntry<float> BubbleExplosionSize;
        public static ConfigEntry<float> BubbleSpeed;
        public static ConfigEntry<int> BubbleCountPerShot;
        public static ConfigEntry<int> BubbleShotCount;
        public static ConfigEntry<float> BubbleForce;
        public static ConfigEntry<float> BubbleBaseHealth;
        public static ConfigEntry<float> BubbleHealthPerLevel;
        public static ConfigEntry<float> BubbleLifetime;
        public static ConfigEntry<float> BubbleGlobalDeathProcCoefficient;

        public static ConfigEntry<KeyCode> EmoteKey;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Sand Crab Director", "Enable Sand Crab", true, "Enables Sand Crab.");

            DirectorCost = config.Bind("Sand Crab Director", "Director Cost", 40, "Director cost of Sand Crab.");
            SelectionWeight = config.Bind("Sand Crab Director", "Selection Weight", 1, "Selection weight of Sand Crab.");
            MinimumStageCompletion = config.Bind("Sand Crab Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");

            DefaultStageList = config.Bind("Sand Crab Director", "Default Variant Stage List",
                string.Join
                (
                    ",",
                    ""
                ),
                "Stages that Default Sand Crab appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            GrassyStageList = config.Bind("Sand Crab Director", "Grassy Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.DistantRoost),
                    "FBLScene"
                ),
                "Stages that Grassy Sand Crab appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            SandyStateList = config.Bind("Sand Crab Director", "Sandy Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.ReformedAltar)
                ),
                "Stages that Sandy Sand (lol, lmao) Crab appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            SulfurStageList = config.Bind("Sand Crab Director", "Sulfur Variant Stage List",
                string.Join
                (
                    ",",
                    DirectorAPI.ToInternalStageName(DirectorAPI.Stage.SulfurPools)
                ),
                "Stages that Sulfur Sand Crab appears in. Stages should be separated by coma, internal names can be found in game via \"list_scenes\" command.");

            BaseMaxHealth = config.Bind("Sand Crab Character Stats", "Base Max Health", 480f, "Sand Crab's base health.");
            BaseMoveSpeed = config.Bind("Sand Crab Character Stats", "Base Movement Speed", 10f, "Sand Crab's base movement speed.");
            BaseJumpPower = config.Bind("Sand Crab Character Stats", "Base Jump Power", 18f, "Sand Crab's base jump power.");
            BaseDamage = config.Bind("Sand Crab Character Stats", "Base Damage", 16f, "Sand Crab's base damage.");
            BaseArmor = config.Bind("Sand Crab Character Stats", "Base Armor", 0f, "Sand Crab's base armor.");

            LevelMaxHealth = config.Bind("Sand Crab Character Stats", "Health per Level", 144f, "Sand Crab's health increase per level.");
            LevelDamage = config.Bind("Sand Crab Character Stats", "Damage per Level", 3.2f, "Sand Crab's damage increase per level.");
            LevelArmor = config.Bind("Sand Crab Character Stats", "Armor per Level", 0f, "Sand Crab's armor increase per level.");

            SnipCooldown = config.Bind("Sand Crab Snip", "Snip Cooldown", 4f, "Sand Crab's Snip cooldown.");
            SnipHoldMaxDuration = config.Bind("Sand Crab Snip", "Snip Hold State Max Duration", 2f, "Sand Crab's Snip skill hold max duration.");
            SnipDamage = config.Bind("Sand Crab Snip", "Snip Damage", 5f, "Sand Crab's Snip damage.");
            SnipForce = config.Bind("Sand Crab Snip", "Snip Force", 200f, "Sand Crab's Snip force");

            BubbleCooldown = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Cooldown", 15f, "Sand Crab's Fire Bubbles cooldown.");
            BubbleDamage = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Damage", 1.5f, "Sand Crab's Fire Bubbles projectile damage.");
            BubbleProjectileSpread = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Spread", 120f, "Sand Crab's Fire Bubbles projectile spread. The bigger the angle, the more further apart projectiles will be on spawn.");
            BubbleSize = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Size", 2.25f, "Sand Crab's Fire Bubbles projectile size");
            BubbleShotCount = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Shots Count", 6, "Sand Crab's Fire Bubbles shots count");
            BubbleExplosionSize = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Explosion Radius", 2.5f, "Sand Crab's Fire Bubbles projectile explosion radius");
            BubbleSpeed = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Speed", 12f, "Sand Crab's Fire Bubbles projectile speed");
            BubbleForce = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Force", 0f, "Sand Crab's Fire Bubbles projectile force.");

            BubbleBaseHealth = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Base Health", 35f, "Sand Crab's Fire Bubbles projectile base health.");
            BubbleHealthPerLevel = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Per Level Health", 10f, "Sand Crab's Fire Bubbles projectile per level health.");
            BubbleLifetime = config.Bind("Sand Ctab Fire Bubbles", "Fire Bubbles Projectile Lifetime", 12f, "Sand Crab's Fire Bubbles projectile lifetime.");
            BubbleGlobalDeathProcCoefficient = config.Bind("Sand Crab Fire Bubbles", "Fire Bubbles Global Death Proc Coefficient", 0f, "Sand Crab's Fire Bubbles projectile global death proc coefficient, basically controls how frequently on death procs happen when bubble is killed.");

            EmoteKey = config.Bind("Sand Crab Emotes", "Dance Emote", KeyCode.Alpha1, "Key used to Dance.");
        }
    }
}
