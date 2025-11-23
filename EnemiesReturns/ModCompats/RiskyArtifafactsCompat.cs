using RoR2;
using System.Runtime.CompilerServices;

namespace EnemiesReturns.ModCompats
{
    internal class RiskyArtifafactsCompat
    {
        public const string PLUGIN_GUID = "com.Moffein.RiskyArtifacts";

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
        public static void AddMonsterToArtifactOfOrigin(SpawnCard spawnCard, int bossTier)
        {
            switch (bossTier)
            {
                case 1:
                default:
                    Risky_Artifacts.Artifacts.Origin.AddSpawnCard(spawnCard, Risky_Artifacts.Artifacts.Origin.BossTier.t1);
                    break;
                case 2:
                    Risky_Artifacts.Artifacts.Origin.AddSpawnCard(spawnCard, Risky_Artifacts.Artifacts.Origin.BossTier.t2);
                    break;
                case 3:
                    Risky_Artifacts.Artifacts.Origin.AddSpawnCard(spawnCard, Risky_Artifacts.Artifacts.Origin.BossTier.t3);
                    break;
            }
        }
    }
}
