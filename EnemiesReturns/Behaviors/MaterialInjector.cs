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

        public bool loadAsynchronous = true;

        private AsyncOperationHandle<Material> handle;

        private void Awake()
        {
            if (isParticle)
            {
                renderer = GetComponent<ParticleSystemRenderer>();
            }

            if (renderer && !string.IsNullOrEmpty(addressableMaterialPath))
            {
                handle = Addressables.LoadAssetAsync<Material>(addressableMaterialPath);
                if (!handle.IsValid())
                {
                    Log.Error($"Couldn't find material {addressableMaterialPath} while injecting it to {this.gameObject.name}");
                    Addressables.Release(handle);
                    UnityEngine.GameObject.Destroy(this);
                    return;
                }

                if (loadAsynchronous)
                {
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
                else
                {
                    var material = handle.WaitForCompletion();
                    renderer.material = material;
                    if (isTrailParticle)
                    {
                        (renderer as ParticleSystemRenderer).trailMaterial = material;
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
