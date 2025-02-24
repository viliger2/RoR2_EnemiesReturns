using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.PileOfDirt
{
    public class DropItem : MonoBehaviour
    {
        public ItemDef itemToDrop;

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

            var vector = Vector3.up * 20f + transform.forward * 2f;
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(itemToDrop.itemIndex), transform.position, vector);
            NetworkServer.Destroy(this.gameObject);
        }

    }
}
