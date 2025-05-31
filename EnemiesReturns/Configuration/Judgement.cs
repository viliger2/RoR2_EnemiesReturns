using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    public class Judgement : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;

        public static ConfigEntry<bool> EnableAnointedSkins;

        public static ConfigEntry<bool> ForceUnlock;

        public static ConfigEntry<string> JudgementEnemyBlacklist;

        public static ConfigEntry<float> MithrixHammerAeonianBonusDamage;
        public static ConfigEntry<float> MithrixHammerDamageCoefficient;
        public static ConfigEntry<float> MithrixHammerCooldown;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Judgement", "Enabled", true, "Enables all content related to Judgement.");
            EnableAnointedSkins = config.Bind("Judgement", "Enable Anointed Skins", true, "Enables the ability to unlock Anointed skins.");
            ForceUnlock = config.Bind("Judgement", "Force Unlock Anointed Skins", false, "Force unlocks all Anointed skins by removing UnlockableDef from them.");
            JudgementEnemyBlacklist = config.Bind("Judgement", "Enemy Blacklist",
                "GeepMaster,GipMaster,GupMaster,ClayBruiserMaster,MinorConstructMaster,VoidMegaCrabMaster,LunarGolemMaster,LunarWispMaster,NullifierMaster,VoidJailerMaster,HalcyoniteMaster,LunarExploderMaster,VoidBarnacleMaster", 
                "List of enemies that are blacklisted from appearing in Judgement. Requiers master names, you can get master names via DebugToolkit's list_ai command");

            MithrixHammerAeonianBonusDamage = config.Bind("Mithrix Hammer", "Mithrix Hammer Bonus Damage Against Aeonians", 500f, "Bonus damage multiplier against Aeonian elites.");
            MithrixHammerDamageCoefficient = config.Bind("Mithrix Hammer", "Mithrix Hammer Damage Coefficient", 30f, "Mithrix Hammer damage coefficient off base damage.");
            MithrixHammerCooldown = config.Bind("Mithrix Hammer", "Mithrix Hammer Cooldown", 15f, "Mithrix Hammer cooldown.");
        }
    }
}
