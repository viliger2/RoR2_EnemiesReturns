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

                LegacyResourcesAPI.LoadAsyncCallback(addressableMaterialPath, delegate (Material result)
                {
                    renderer.material = result;
                    if (isTrailParticle)
                    {
                        (renderer as ParticleSystemRenderer).trailMaterial = result;
                    }
                    UnityEngine.GameObject.Destroy(this);
                });
            }
            //    try
            //    {

            //        //LegacyResourcesAPI.LoadAsyncCallback(addressableMaterialPath, delegate (Material result)
            //        //{
            //        //    renderer.material = result;
            //        //    if (isTrailParticle)
            //        //    {
            //        //        (renderer as ParticleSystemRenderer).trailMaterial = result;
            //        //    }
            //        //});
            //        var material = Addressables.LoadAssetAsync<Material>(addressableMaterialPath).WaitForCompletion();
            //        renderer.material = material;
            //        if (isTrailParticle)
            //        {
            //            (renderer as ParticleSystemRenderer).trailMaterial = material;
            //        }
            //    }
            //    catch (Exception e) 
            //    {
            //        Log.Error($"Error while injecting material into renderer belonging to {this.gameObject.name}: {e}");
            //    }
            //}
            //UnityEngine.GameObject.Destroy(this);
        }
    }
}
