using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors
{
    public class MaterialInjector : MonoBehaviour
    {
        public string addressableMaterialPath;

        public bool isParticle;

        public Renderer renderer;

        private void Awake()
        {
            if (isParticle)
            {
                renderer = GetComponent<ParticleSystemRenderer>();
            }

            if(renderer && !string.IsNullOrEmpty(addressableMaterialPath))
            {
                try
                {
                    var material = Addressables.LoadAssetAsync<Material>(addressableMaterialPath).WaitForCompletion();
                    renderer.material = material;
                }
                catch (Exception e) 
                {
                    Log.Error($"Error while injecting material into renderer belonging to {this.gameObject.name}: {e}");
                }
            }
        }

    }
}
