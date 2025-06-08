using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EnemiesReturns.Behaviors
{
    public class MaterialInjector : MonoBehaviour
    {
        public string addressableMaterialPath;

        public bool isParticle;

        public bool isTrailParticle;

        public Renderer renderer;

        private AsyncOperationHandle<Material> handle;

        private void Awake()
        {
            if (isParticle)
            {
                renderer = GetComponent<ParticleSystemRenderer>();
            }

            if (renderer && !string.IsNullOrEmpty(addressableMaterialPath))
            {
                if (!Addressables.LoadAssetAsync<GameObject>(addressableMaterialPath).IsValid())
                {
                    Log.Error($"Couldn't find material {addressableMaterialPath} while injecting it to {this.gameObject.name}");
                    UnityEngine.GameObject.Destroy(this);
                    return;
                }

                if (Addressables.LoadAssetAsync<Material>(addressableMaterialPath).IsValid())
                {
                    handle = Addressables.LoadAssetAsync<Material>(addressableMaterialPath);
                    handle.Completed += (operationResult) =>
                    {
                        if (operationResult.Status == AsyncOperationStatus.Succeeded)
                        {
                            renderer.material = operationResult.Result;
                            if (isTrailParticle)
                            {
                                (renderer as ParticleSystemRenderer).trailMaterial = operationResult.Result;
                            }
                        }
                        else
                        {
                            Addressables.Release(operationResult);
                        }
                        //UnityEngine.GameObject.Destroy(this);
                    };
                }
            }
        }

        private void OnDestroy() 
        {
            Addressables.Release(handle);
        }
    }
}
