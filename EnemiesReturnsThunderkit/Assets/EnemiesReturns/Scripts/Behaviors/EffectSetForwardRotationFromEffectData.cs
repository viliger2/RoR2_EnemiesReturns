using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class EffectSetForwardRotationFromEffectData : MonoBehaviour
    {
        public EffectComponent effectComponent;

        private void Start()
        {
            effectComponent.OnEffectComponentReset = (Action<bool>)Delegate.Combine(effectComponent.OnEffectComponentReset, new Action<bool>(SetupRotation));
        }

        private void SetupRotation(bool hasEffectDate)
        {
            if (hasEffectDate) 
            {
                effectComponent.transform.forward = effectComponent.effectData.rotation.eulerAngles;
            }
        }
    }
}
