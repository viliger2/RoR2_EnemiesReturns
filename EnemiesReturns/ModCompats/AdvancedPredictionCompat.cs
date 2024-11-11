using AdvancedPrediction.Prediction;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModCompats
{
    public static class AdvancedPredictionCompat
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

        public static Ray GetPredictAimRay(Ray aimRay, CharacterBody characterBody, GameObject projectilePrefab)
        {
            return PredictionUtils.PredictAimray(aimRay, characterBody, projectilePrefab);
        }
    }
}
