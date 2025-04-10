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

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Judgement", "Enabled", true, "Enables all content related to Judgement.");
            EnableAnointedSkins = config.Bind("Judgement", "Enable Anointed Skins", true, "Enables the ability to unlock Anointed skins.");
            ForceUnlock = config.Bind("Judgement", "Force Unlock Anointed Skins", false, "Force unlocks all Anointed skins by removing UnlockableDef from them.");
        }
    }
}
