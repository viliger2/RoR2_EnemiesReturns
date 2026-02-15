using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors
{
    public class InteractableSpawnCardSpawner : MonoBehaviour
    {
        public AssetReferenceT<SpawnCard> interactableSpawnCard;

        private void OnEnable()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!Run.instance)
            {
                return;
            }

            SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }

        private void OnDisable()
        {
            SceneDirector.onPostPopulateSceneServer -= SceneDirector_onPostPopulateSceneServer;
        }

        private void SceneDirector_onPostPopulateSceneServer(SceneDirector obj)
        {
            var handle = interactableSpawnCard.LoadAssetAsync();
            handle.Completed += (result) =>
            {
                if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    var spawnCard = handle.Result;
                    DirectorPlacementRule rule = new DirectorPlacementRule()
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Direct,
                        position = transform.position,
                        rotation = transform.rotation,
                    };

                    var gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, rule, Run.instance.stageRng));

                    if ((bool)gameObject)
                    {
                        PurchaseInteraction component = gameObject.GetComponent<PurchaseInteraction>();
                        if ((bool)component && component.costType == CostTypeIndex.Money)
                        {
                            component.Networkcost = Run.instance.GetDifficultyScaledCost(component.cost);
                        }
                    }
                    Addressables.Release(handle);
                }
            };
        }
    }
}
