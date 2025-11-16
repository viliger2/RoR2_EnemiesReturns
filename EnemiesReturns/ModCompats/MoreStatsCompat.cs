using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using MoreStats;

namespace EnemiesReturns.ModCompats
{
    public class MoreStatsCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskOfBrainrot.MoreStats");
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

            if (Configuration.LynxTribe.LynxShaman.Enabled.Value && sender.HasBuff(Content.Buffs.ReduceHealing))
            {
                args.healingPercentIncreaseMult -= Configuration.LynxTribe.LynxShaman.SummonProjectilesHealingFraction.Value;
            }
        }
    }
}
