using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using System.Linq;

namespace EnemiesReturns.PrefabAPICompat
{
    public static class MyPrefabAPI
    {
        private static GameObject parent;

        private static List<GameObject> networkedPrefabs = new List<GameObject>();

        public static GameObject InstantiateClone(this GameObject gameObject, string name, bool registerToNetwork)
        {
            if (PrefabAPICompat.enabled)
            {
                return PrefabAPICompat.InstantiateClone(gameObject, name, registerToNetwork);
            }
            else
            {
                return InstantiateCloneInternal(gameObject, name, registerToNetwork);
            }
        }

        public static void RegisterNetworkPrefab(this GameObject gameObject)
        {
            if (PrefabAPICompat.enabled)
            {
                PrefabAPICompat.RegisterNetworkPrefab(gameObject);
            }
            else
            {
                RegisterToNetwork(gameObject);
            }
        }

        private static GameObject GetParent()
        {
            if (!parent)
            {
                parent = new GameObject(EnemiesReturnsPlugin.ModName + "NetworkParent");
                UnityEngine.Object.DontDestroyOnLoad(parent);
                parent.SetActive(false);
            }

            On.RoR2.Util.IsPrefab += (orig, obj) =>
            {
                if (obj.transform.parent && obj.transform.parent.gameObject.name == EnemiesReturnsPlugin.ModName + "NetworkParent") return true;
                return orig(obj);
            };

            return parent;
        }

        private static GameObject InstantiateCloneInternal(GameObject gameObject, string name, bool registerToNetwork)
        {
            var prefab = UnityEngine.Object.Instantiate(gameObject, GetParent().transform);
            prefab.name = name;
            if (registerToNetwork)
            {
                RegisterToNetwork(prefab);
            }

            return prefab;
        }

        private static void RegisterToNetwork(GameObject gameObject)
        {
            networkedPrefabs.Add(gameObject);
            var networkIdentity = gameObject.GetComponent<NetworkIdentity>();
            if (networkIdentity)
            {
                SetFieldValue(networkIdentity, "m_AssetId", NetworkHash128.Parse(MakeHash(gameObject.name + EnemiesReturnsPlugin.ModName)));
            }
        }

        private static void SetFieldValue(NetworkIdentity identity, string fieldName, object value)
        {
            var field = identity.GetType().GetField(fieldName, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
            if(field != null)
            {
                field.SetValue(identity, value);
            } else
            {
                Log.Error("Couldn't find field " + fieldName + " in " + identity);
            }
        }

        // shamelessly stolen from r2api, even more so than the rest
        private static string MakeHash(string s)
        {
            var hash = MD5.Create();
            byte[] prehash = hash.ComputeHash(Encoding.UTF8.GetBytes(s));
            hash.Dispose();
            var sb = new StringBuilder();

            foreach (var t in prehash)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

    }
}
