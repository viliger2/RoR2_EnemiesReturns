using MoreStats;
using System.Runtime.CompilerServices;

namespace EnemiesReturns.ModCompats
{
    public class MoreStatsCompat
    {
        public const string PLUGIN_GUID = "com.RiskOfBrainrot.MoreStats";

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
        public static void Hooks()
        {
            MoreStats.StatHooks.GetMoreStatCoefficients += StatHooks_GetMoreStatCoefficients;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void StatHooks_GetMoreStatCoefficients(RoR2.CharacterBody sender, StatHooks.MoreStatHookEventArgs args)
        {
            if (!sender)
            {
                return;
            }

            if (Configuration.General.EnableLynxShaman.Value && sender.HasBuff(Content.Buffs.ReduceHealing))
            {
                args.healingPercentIncreaseMult -= Configuration.LynxTribe.LynxShaman.SummonProjectilesHealingFraction.Value;
            }
        }
    }
}
