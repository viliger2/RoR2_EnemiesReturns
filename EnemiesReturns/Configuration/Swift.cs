using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Configuration
{
    public class Swift : IConfiguration
    {
        public static ConfigEntry<bool> Enabled;

        public static ConfigEntry<KeyCode> EmoteKey;

        public void PopulateConfig(ConfigFile config)
        {
            Enabled = config.Bind("Swift Director", "Enable Swift", true, "Enables Swift.");

            EmoteKey = config.Bind("Swift Emotes", "Duck Dance Emote", KeyCode.Alpha1, "Key used to do the Duck Dance.");
        }
    }
}
