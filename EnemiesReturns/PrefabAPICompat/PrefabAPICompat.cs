using R2API;
using UnityEngine;

namespace EnemiesReturns.PrefabAPICompat
{
    public static class PrefabAPICompat
    {
        private static bool? _enabled;

        public static bool enabled
        {
            get
            {
                if (_enabled == null)
                {
                    _enabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(PrefabAPI.PluginGUID);
                }
                return (bool)_enabled;
            }
        }

        public static GameObject InstantiateClone(GameObject gameObject, string name, bool registerToNetwork)
        {
            return PrefabAPI.InstantiateClone(gameObject, name, registerToNetwork);
        }

        public static void RegisterNetworkPrefab(GameObject gameObject)
        {
            PrefabAPI.RegisterNetworkPrefab(gameObject);
        }
    }
}
