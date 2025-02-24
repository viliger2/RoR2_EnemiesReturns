using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.MithrixWeaponDrop
{
    public class DropEquipment : MonoBehaviour
    {
        public EquipmentDef equipmentToDrop;

        public ItemDef itemToCheck;

        private CharacterMaster master;

        private void OnEnable()
        {
            master = GetComponent<CharacterMaster>();
            if (master)
            {
                master.onBodyDeath.AddListener(OnBodyDeath);
            }
        }

        public void OnBodyDeath()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!master)
            {
                return;
            }

            var bodyObject = master.GetBodyObject();
            if (!bodyObject)
            {
                return;
            }

            bool itemFound = false;
            foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
            {
                if (!playerCharacterMaster.isConnected || !playerCharacterMaster.master)
                {
                    continue;
                }

                if (!playerCharacterMaster.master.inventory)
                {
                    return;
                }

                if(playerCharacterMaster.master.inventory.GetItemCount(itemToCheck) > 0)
                {
                    itemFound = true;
                    break;
                }
            }

            if (itemFound) 
            {
                var vector = Vector3.up * 20f + transform.forward * 2f;
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipmentToDrop._equipmentIndex), bodyObject.transform.position, vector);
            }
        }

    }
}
