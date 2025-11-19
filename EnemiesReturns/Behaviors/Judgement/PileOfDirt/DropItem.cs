using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.PileOfDirt
{
    public class DropItem : MonoBehaviour
    {
        public ItemDef itemToDrop;

        public Transform dropletOrigin;

        public void DropAndDestroySelf()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!itemToDrop)
            {
                return;
            }

            var spawnPoint = dropletOrigin;
            if (!spawnPoint)
            {
                spawnPoint = transform;
            }

            var vector = Vector3.up * 20f + transform.forward * 2f;
            PickupDropletController.CreatePickupDroplet(new UniquePickup(PickupCatalog.FindPickupIndex(itemToDrop.itemIndex)), spawnPoint.position, vector, false);
            NetworkServer.Destroy(this.gameObject);
        }

    }
}
