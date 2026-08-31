using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight
{
    public class ContactLight : IConfiguration
    {
        public static ConfigEntry<bool> ForceUnlock;

        public void PopulateConfig(ConfigFile config)
        {
            ForceUnlock = config.Bind("Contact Light", "Force Unlock Content", false, "Force unlocks all content related to Contact Light.");
        }
    }
}
