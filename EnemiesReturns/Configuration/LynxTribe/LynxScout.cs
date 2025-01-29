using BepInEx.Configuration;
using UnityEngine;

namespace EnemiesReturns.Configuration.LynxTribe
{
    public class LynxScout
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

        public static ConfigEntry<float> DoubleSlashCooldown;
        public static ConfigEntry<float> DoubleSlashDamage;
        public static ConfigEntry<float> DoubleSlashProcCoefficient;

        public static ConfigEntry<KeyCode> SingEmoteKey;

        public static void PopulateConfig(ConfigFile config)
        {
            SelectionWeight = config.Bind("Lynx Scout Director", "Selection Weight", 1, "Selection weight of Lynx Scout.");
            DirectorCost = config.Bind("Lynx Scout Director", "Director Cost", 28, "Director cost of Lynx Scout.");

            BaseMaxHealth = config.Bind("Lynx Scout Character Stats", "Base Max Health", 210f, "Lynx Scout' base health.");
            BaseMoveSpeed = config.Bind("Lynx Scout Character Stats", "Base Movement Speed", 12f, "Lynx Scout' base movement speed.");
            BaseJumpPower = config.Bind("Lynx Scout Character Stats", "Base Jump Power", 18f, "Lynx Scout' base jump power.");
            BaseDamage = config.Bind("Lynx Scout Character Stats", "Base Damage", 12f, "Lynx Scout' base damage.");
            BaseArmor = config.Bind("Lynx Scout Character Stats", "Base Armor", 0f, "Lynx Scout' base armor.");

            LevelMaxHealth = config.Bind("Lynx Scout Character Stats", "Health per Level", 42f, "Lynx Scout' health increase per level.");
            LevelDamage = config.Bind("Lynx Scout Character Stats", "Damage per Level", 2.4f, "Lynx Scout' damage increase per level.");
            LevelArmor = config.Bind("Lynx Scout Character Stats", "Armor per Level", 0f, "Lynx Scout' armor increase per level.");

            DoubleSlashCooldown = config.Bind("Lynx Scout Double Slash", "Double Slash Cooldown", 0f, "Lynx Scout's Double Slash cooldown.");
            DoubleSlashDamage = config.Bind("Lynx Scout Double Slash", "Double Slash Damage", 1.8f, "Lynx Scout's Double Slash damage of each slash.");
            DoubleSlashProcCoefficient = config.Bind("Lynx Scout Double Slash", "Double Slash Proc Coefficient", 1f, "Lynx Scout's Double Slash proc coefficient.");

            SingEmoteKey = config.Bind("Lynx Scout Emotes", "Sing Emote", KeyCode.Alpha1, "Key used to Sing.");
        }
    }
}
