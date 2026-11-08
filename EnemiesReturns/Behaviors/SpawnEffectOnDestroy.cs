using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class SpawnEffectOnDestroy : MonoBehaviour
    {
        public GameObject effectPrefab;

        public Transform effectOrigin;

        private void OnDestroy()
        {
            if (effectPrefab)
            {
                var origin = base.transform;
                if (effectOrigin)
                {
                    origin = effectOrigin;
                }

                EffectManager.SimpleEffect(effectPrefab, origin.position, Quaternion.identity, false);
            }
        }

    }
}
