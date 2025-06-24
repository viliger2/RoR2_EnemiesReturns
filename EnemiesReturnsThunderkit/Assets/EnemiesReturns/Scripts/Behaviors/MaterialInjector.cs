using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class MaterialInjector : MonoBehaviour
    {
        public string addressableMaterialPath;

        public bool isParticle;

        public bool isTrailParticle;

        public bool loadAsynchronous = true;

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
                    //var material = Addressables.LoadAssetAsync<Material>(addressableMaterialPath).WaitForCompletion();
                    //renderer.material = material;
                    //if (isTrailParticle)
                    //{
                    //    (renderer as ParticleSystemRenderer).trailMaterial = material;
                    //}
                }
                catch (Exception e) 
                {
                    //Log.Error($"Error while injecting material into renderer belonging to {this.gameObject.name}: {e}");
                }
            }
            UnityEngine.GameObject.Destroy(this);
        }

    }
}
