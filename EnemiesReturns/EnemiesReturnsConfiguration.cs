using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns
{
    public static class EnemiesReturnsConfiguration
    {
        public static ConfigEntry<float> DebugWalkSpeedValue;

        public static ConfigEntry<float> testconfig;

        public struct Spitter
        {
            public static ConfigEntry<int> DirectorCost;
            public static ConfigEntry<int> SelectionWeight;
            public static ConfigEntry<int> MinimumStageCompletion;

            public static ConfigEntry<float> BaseMaxHealth;
            public static ConfigEntry<float> BaseMoveSpeed;
            public static ConfigEntry<float> BaseJumpPower;
            public static ConfigEntry<float> BaseDamage;
            public static ConfigEntry<float> BaseArmor;
            public static ConfigEntry<float> LevelMaxHealth;
            public static ConfigEntry<float> LevelDamage;
            public static ConfigEntry<float> LevelArmor;

            public static ConfigEntry<float> BiteDamageModifier;
            public static ConfigEntry<float> BiteDamageForce;
            public static ConfigEntry<float> BiteRadius;

            [Obsolete]
            public static ConfigEntry<float> NormalSpitDamage;
            [Obsolete]
            public static ConfigEntry<float> NormalSpitSpeed;
            [Obsolete]
            public static ConfigEntry<float> NormalSpitForce;

            public static ConfigEntry<float> ChargedProjectileCooldown;
            public static ConfigEntry<float> ChargedProjectileDamage;
            public static ConfigEntry<float> ChargedProjectileForce;
            public static ConfigEntry<float> ChargedProjectileLargeDoTZoneDamage;
            public static ConfigEntry<float> ChargedProjectileLargeDoTZoneScale;
            public static ConfigEntry<float> ChargedProjectileFlyTime;
            public static ConfigEntry<float> ChargedProjectileSmallDoTZoneDamage;
            public static ConfigEntry<float> ChargedProjectileSmallDoTZoneScale;
        }

        public static void PopulateConfig(ConfigFile config) 
        {
            config.Clear();

            DebugWalkSpeedValue = config.Bind("Debug", "walkSpeed value", 1f, "Value speed for walkSpeed animation. For debugging.");
            testconfig = config.Bind("test", "test", 5f, "test");

            #region Spitter

            Spitter.SelectionWeight = config.Bind("Director", "Selection Weight", 1, "Selection weight of Spitter.");
            Spitter.MinimumStageCompletion = config.Bind("Director", "Minimum Stage Completion", 0, "Minimum stages players need to complete before monster starts spawning.");
            Spitter.DirectorCost = config.Bind("Director", "Director Cost", 30, "Director cost of Spitter.");
            
            Spitter.BaseMaxHealth = config.Bind("Character Stats", "Base Max Health", 300f, "Spitter's base health.");
            Spitter.BaseMoveSpeed = config.Bind("Character Stats", "Base Movement Speed", 7f, "Spitter's base movement speed.");
            Spitter.BaseJumpPower = config.Bind("Character Stats", "Base Jump Power", 20f, "Spitter's base jump power.");
            Spitter.BaseDamage = config.Bind("Character Stats", "Base Damage", 20f, "Spitter's base damage.");
            Spitter.BaseArmor = config.Bind("Character Stats", "Base Armor", 0f, "Spitter's base armor.");

            Spitter.LevelMaxHealth = config.Bind("Character Stats", "Health per Level", 90f, "Spitter's health increase per level.");
            Spitter.LevelDamage = config.Bind("Character Stats", "Damage per Level", 4f, "Spitter's damage increase per level.");
            Spitter.LevelArmor = config.Bind("Character Stats", "Armor per Level", 0f, "Spitter's armor increase per level.");

            Spitter.BiteDamageModifier = config.Bind("Spitter Bite", "Bite Damage", 1.5f, "Spitter's Bite damage.");
            Spitter.BiteDamageForce = config.Bind("Spitter Bite", "Bite Force", 200f, "Spitter's Bite force, by default equal to that of Lemurian.");
            Spitter.BiteRadius = config.Bind("Spitter Bite", "Bite Radius", 0f, "Spitter's Bite radius.");

            //Spitter.NormalSpitDamage = config.Bind("Spitter Normal Spit", "Spit Damage", 2f, "Spitter's Normal Spit projectile damage.");
            //Spitter.NormalSpitForce = config.Bind("Spitter Normal Spit", "Spit Force", 1000f, "Spitter's Normal Spit force.");
            //Spitter.NormalSpitSpeed = config.Bind("Spitter Normal Spit", "Spit Speed", 50f, "Spitter's Normal Spit projectile speed.");

            Spitter.ChargedProjectileCooldown = config.Bind("Spitter Charged Spit", "Charged Spit Cooldown", 6f, "Charged Spit Cooldown");
            Spitter.ChargedProjectileDamage = config.Bind("Spitter Charged Spit", "Charged Spit Damage", 1.6f, "Spitter's Charged projectile damage.");
            Spitter.ChargedProjectileForce = config.Bind("Spitter Charged Spit", "Charged Spit Force", 0f, "Spitter's Charged projectile force.");
            Spitter.ChargedProjectileFlyTime = config.Bind("Spitter Charged Spit", "Charged Spit Fly Time", 0.75f, "Spitter's Charged Projectile fly time. The higher the value the bigger the arc and the slower the projectile will be. Don't set it too low otherwise projectile will fly pass targes and puddles will be somewhere in the back.");

            Spitter.ChargedProjectileLargeDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileLargeDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Large DoT Zone Size Scale", 0.55f, "Spitter's Charged Large DoT Zone size scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");
            Spitter.ChargedProjectileSmallDoTZoneDamage = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Damage", 0.15f, "Spitter's Charged Large DoT zone damage off projectile's damage.");
            Spitter.ChargedProjectileSmallDoTZoneScale = config.Bind("Spitter Charged Spit", "Charged Spit Small DoT Zone Scale", 0.3f, "Spitter's Charged Large DoT Zone scale off Mini Mushrim's DoT zone (since it was used as basis). Also controls projectile's blast radius.");

            #endregion
        }


    }
}
