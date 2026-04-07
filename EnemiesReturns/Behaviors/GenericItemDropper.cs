using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class GenericItemDropper : MonoBehaviour
    {
        public ItemDef itemToDrop;

        [Range(0f, 100f)]
        public float percentChance;

        public void OnEnable()
        {
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        public void OnDisable()
        {
            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!damageReport.victimBody || !damageReport.victimMaster || !damageReport.attackerMaster)
            {
                return;
            }

            if(damageReport.victimMaster.teamIndex == TeamIndex.Player)
            {
                return;
            }

            if (!itemToDrop)
            {
                return;
            }

            if(Util.CheckRoll(percentChance, damageReport.attackerMaster))
            {
                Vector3 corePosition = damageReport.victimBody.corePosition;
                Vector3 velocity = Vector3.up * 10f;
                GenericPickupController.CreatePickupInfo pickupInfo = default(GenericPickupController.CreatePickupInfo);
                pickupInfo.pickup = new UniquePickup(PickupCatalog.FindPickupIndex(itemToDrop.itemIndex));
                pickupInfo.position = corePosition;
                PickupDropletController.CreatePickupDroplet(pickupInfo, corePosition, velocity);
            }
        }
    }
}
