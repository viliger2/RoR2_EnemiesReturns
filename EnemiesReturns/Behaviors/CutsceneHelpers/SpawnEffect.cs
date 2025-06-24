using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class SpawnEffect : MonoBehaviour
    {
        public string assetPath;

        public float scale = -1f;

        private GameObject effectPrefab;

        private void Awake()
        {
            if (!string.IsNullOrEmpty(assetPath))
            {
                var handle = Addressables.LoadAssetAsync<GameObject>(assetPath);
                if (handle.IsValid())
                {
                    effectPrefab = handle.WaitForCompletion();
                }
            }
        }
        private void OnEnable()
        {
            if (effectPrefab)
            {
                var effectData = new EffectData();
                effectData.origin = transform.position;
                effectData.rotation = transform.rotation;
                if(scale > 0)
                {
                    effectData.scale = scale;
                }

                EffectManager.SpawnEffect(effectPrefab, effectData, false);
            }
        }
    }
}
