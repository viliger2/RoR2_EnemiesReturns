using R2API;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
