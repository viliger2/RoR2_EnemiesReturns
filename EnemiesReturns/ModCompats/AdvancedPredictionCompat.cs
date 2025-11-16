using AdvancedPrediction.Prediction;
using RoR2;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EnemiesReturns.ModCompats
{
    internal static class AdvancedPredictionCompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.score.AdvancedPrediction");
                }
                return (bool)_enabled;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static Ray GetPredictAimRay(Ray aimRay, CharacterBody characterBody, GameObject projectilePrefab)
        {
            return PredictionUtils.PredictAimray(aimRay, characterBody, projectilePrefab);
        }
    }
}
