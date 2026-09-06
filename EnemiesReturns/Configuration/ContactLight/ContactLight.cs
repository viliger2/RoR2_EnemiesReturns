using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight
{
    public class ContactLight : IConfiguration
    {
        public static ConfigEntry<bool> ForceUnlock;

        public static ConfigEntry<bool> EliteSlayerDropAllElites;

        public void PopulateConfig(ConfigFile config)
        {
            ForceUnlock = config.Bind("Contact Light", "Force Unlock Content", false, "Force unlocks all content related to Contact Light.");

            EliteSlayerDropAllElites = config.Bind("Elite Slayer", "Can Target All Elites", false, "Allows targeting of all elites and not just elites that can drop their aspect, ex. Void elites can't drop their equipment.");
        }
    }
}
