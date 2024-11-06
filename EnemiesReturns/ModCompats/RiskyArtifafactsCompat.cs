using AdvancedPrediction.Prediction;
using EntityStates.Croco;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModCompats
{
    internal class RiskyArtifafactsCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskyArtifacts");
                }
                return (bool)_enabled;
            }
        }

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
