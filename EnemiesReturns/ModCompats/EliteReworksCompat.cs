using RoR2;
using System.Runtime.CompilerServices;

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

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void ModifyAeonianElites(EliteDef aenonianElite)
        {
            aenonianElite.damageBoostCoefficient = EliteReworks.Tweaks.ModifyEliteTiers.t2Damage;
            aenonianElite.healthBoostCoefficient = EliteReworks.Tweaks.ModifyEliteTiers.t2Health;
        }
    }
}
