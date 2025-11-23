using RoR2;
using System.Runtime.CompilerServices;

namespace EnemiesReturns.ModCompats
{
    internal static class EliteReworksCompat
    {
        public const string PLUGIN_GUID = "com.Moffein.EliteReworks2";

        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PLUGIN_GUID);
                }
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void ModifyAeonianElites(EliteDef aenonianElite)
        {
            aenonianElite.damageBoostCoefficient = EliteReworks2.Elites.Malachite.Malachite.damageBoostCoefficient;
            aenonianElite.healthBoostCoefficient = EliteReworks2.Elites.Malachite.Malachite.healthBoostCoefficient;
        }
    }
}
