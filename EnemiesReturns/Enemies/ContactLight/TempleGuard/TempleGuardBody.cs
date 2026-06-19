using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.ContactLight.TempleGuard
{
    public class TempleGuardBody
    {
        public GameObject SetupBody(GameObject prefab)
        {
            var body = prefab.GetComponent<CharacterBody>();
            if (body)
            {
                body._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            }

            var modelLocator = prefab.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform)
            {
                var footsteps = modelLocator.modelTransform.gameObject.GetComponent<FootstepHandler>();
                if(footsteps)
                {
                    footsteps.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion();
                }
            }

            return prefab;
        }


    }
}
