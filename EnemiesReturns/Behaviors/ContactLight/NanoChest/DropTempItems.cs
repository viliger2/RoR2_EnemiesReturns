using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.ContactLight.NanoChest
{
    public class DropTempItems : NetworkBehaviour
    {
        public int numberToDrop = 5;

        public Vector3 localEjectionVelocity = new Vector3(0f, 15f, 8f);

        public bool sameItem = false;

        public AssetReferenceT<PickupDropTable> dropTableReference;

        public bool useNormalReference;

        public int maxPurchaseCount;

        private int purchaseCount;

        private UniquePickup itemToDrop;

        private PurchaseInteraction purchaseInteraction;

        public PickupDropTable dropTable;

        private void Awake()
        {
            purchaseInteraction = GetComponent<PurchaseInteraction>();

            if (!useNormalReference)
            {
                var handle = dropTableReference.LoadAssetAsync();
                handle.Completed += (result) =>
                {
                    if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        dropTable = result.Result;
                        if (sameItem)
                        {
                            itemToDrop = dropTable.GeneratePickup(RoR2.Run.instance.treasureRng);
                            itemToDrop.decayValue = 1f;
                        }
                        Addressables.Release(handle);
                    }
                };
            } else if (!dropTable)
            {
                Log.Warning($"DropTempItems on {gameObject} has no dropTable, doing nothing.");
                if (purchaseInteraction) 
                {
                    purchaseInteraction.SetAvailable(false);
                }
            }
        }

        public void AddStack(Interactor interactor)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var angle = 180f / (numberToDrop + 1); // plus 1 so we split into equal parts and spawn between each
            var quaternion = UnityEngine.Quaternion.AngleAxis(angle, Vector3.up);
            var vector = quaternion * transform.rotation * localEjectionVelocity;
            int spawnedCount = 0;
            while (spawnedCount < numberToDrop)
            {
                if (sameItem)
                {
                    PickupDropletController.CreatePickupDroplet(itemToDrop, transform.position, vector, false, false);
                }
                else
                {
                    var itemToDrop = dropTable.GeneratePickup(RoR2.Run.instance.treasureRng);
                    itemToDrop.decayValue = 1f;
                    PickupDropletController.CreatePickupDroplet(itemToDrop, transform.position, vector, false, false);
                }
                spawnedCount++;
                vector = quaternion * vector;
            }

            purchaseCount++;
            if (purchaseCount >= maxPurchaseCount)
            {
                // do something, I dunoo
            }
        }
    }
}
