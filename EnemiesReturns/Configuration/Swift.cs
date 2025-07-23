using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    public class Swift : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Swift Director", "Enable Swift", true, "Enables Swift.");
        }
    }
}
