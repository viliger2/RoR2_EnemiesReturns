using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Configuration.ContactLight
{
    public class AdrenalineCore : IConfiguration
    {

        public static ConfigEntry<bool> EnableUI;

        public static ConfigEntry<bool> TransendanceSupport;

        public void PopulateConfig(ConfigFile config)
        {
            EnableUI = config.Bind("Main Functionality", "Enable UI", true, "Enables UI (level bar under health bar) of Adrenaline Core.");
            TransendanceSupport = config.Bind("Main Functionality", "Transendance Support", true, "Transendance support. Swaps HP check from health to shields if player has Transendance.");
        }
    }
}
