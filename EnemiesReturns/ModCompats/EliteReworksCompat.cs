using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using EliteReworks;

namespace EnemiesReturns.ModCompats
{
    internal static class EliteReworksCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EliteReworks");
                }
                return (bool)_enabled;
            }
        }

        public static void ModifyAeonianElites(EliteDef aenonianElite)
        {
            aenonianElite.damageBoostCoefficient = EliteReworks.Tweaks.ModifyEliteTiers.t2Damage;
            aenonianElite.healthBoostCoefficient = EliteReworks.Tweaks.ModifyEliteTiers.t2Health;
        }
    }
}
