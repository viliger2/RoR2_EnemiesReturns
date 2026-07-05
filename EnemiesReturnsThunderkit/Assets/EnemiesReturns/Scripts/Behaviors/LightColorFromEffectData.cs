using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class LightColorFromEffectData : MonoBehaviour
    {
        public Light[] lights;

        public EffectComponent effectComponent;

        private void Start()
        {
            if (!effectComponent.noEffectData)
            {
                var color = effectComponent.effectData.color;
                for(int i = 0; i < lights.Length; i++)
                {
                    var light = lights[i];
                    light.color = color;
                }
            }
        }
    }
}
