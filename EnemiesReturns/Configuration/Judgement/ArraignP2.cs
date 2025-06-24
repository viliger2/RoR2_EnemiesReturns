using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.Judgement
{
    public class ArraignP2 : IConfiguration
    {

        public static ConfigEntry<float> P2BaseMaxHealth;
        public static ConfigEntry<float> P2BaseMoveSpeed;
        public static ConfigEntry<float> P2BaseDamage;
        public static ConfigEntry<float> P2BaseArmor;
        public static ConfigEntry<float> P2LevelMaxHealth;
        public static ConfigEntry<float> P2LevelDamage;
        public static ConfigEntry<float> P2LevelArmor;
        public static ConfigEntry<float> P2SprintMultiplier;
        public static ConfigEntry<int> P2HealthSegments;
        public void PopulateConfig(ConfigFile config)
        {
            P2BaseMaxHealth = config.Bind("Arraign P2 Character Stats", "Base Max Health", 4000f, "Arraign P2' base health.");
            P2BaseMoveSpeed = config.Bind("Arraign P2 Character Stats", "Base Movement Speed", 15f, "Arraign P2' base movement speed.");
            P2BaseDamage = config.Bind("Arraign P2 Character Stats", "Base Damage", 16f, "Arraign P2' base damage.");
            P2BaseArmor = config.Bind("Arraign P2 Character Stats", "Base Armor", 20f, "Arraign P2' base armor.");

            P2LevelMaxHealth = config.Bind("Arraign P2 Character Stats", "Health per Level", 1200f, "Arraign P2' health increase per level.");
            P2LevelDamage = config.Bind("Arraign P2 Character Stats", "Damage per Level", 3.2f, "Arraign P2' damage increase per level.");
            P2LevelArmor = config.Bind("Arraign P2 Character Stats", "Armor per Level", 0f, "Arraign P2' armor increase per level.");

            P2SprintMultiplier = config.Bind("Arraign P2 Character Stats", "Sprint Multiplier", 3.2f, "Arraign P2' health increase per level.");

            P2HealthSegments = config.Bind("Arraign P2 Character Stats", "Number of Health Segments", 3, "Arraign P2' number of health segments that you need to break with hammer.");
        }
    }
}
