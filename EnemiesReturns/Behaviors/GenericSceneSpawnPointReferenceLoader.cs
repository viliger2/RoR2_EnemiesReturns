using RoR2.Networking;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(GenericSceneSpawnPoint))]
    public class GenericSceneSpawnPointReferenceLoader : MonoBehaviour
    {
        public AssetReferenceT<GameObject> assetReference;

        private void Awake()
        {
            var gssp = GetComponent<GenericSceneSpawnPoint>();
            if (gssp)
            {
                gssp.networkedObjectPrefab = assetReference.LoadAssetAsync().WaitForCompletion();
            }
        }
    }
}
