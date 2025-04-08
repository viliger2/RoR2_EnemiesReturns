using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration
{
    public class Judgement : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;

        public static ConfigEntry<bool> EnableAeonianSkins;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Judgement", "Enabled", true, "Enables all content related to Judgement.");

            EnableAeonianSkins = config.Bind("Judgement", "Enable Aeonian Skins", true, "Enables the ability to unlock Aeonian skins.");
        }
    }
}
