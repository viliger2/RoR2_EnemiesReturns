using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModCompats
{
    public class Starstorm2Compat
    {
        public const string PLUGIN_GUID = "com.TeamMoonstorm";

        public const string PLUGIN_GUID_OLD = "com.TeamMoonstorm.Starstorm2";

        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID) || BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID_OLD);
                }
                return (bool)_enabled;
            }
        }
    }
}
