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

        public string dropChatToken;

        private CharacterMaster master;

        private CharacterBody body;

        private bool wasMonster = false;

        private void OnEnable()
        {
            master = GetComponent<CharacterMaster>();
            if (master)
            {
                master.onBodyStart += Master_onBodyStart;
                master.onBodyDeath.AddListener(OnBodyDeath);
                wasMonster = master.teamIndex == TeamIndex.Monster;
            }
        }

        private void Master_onBodyStart(CharacterBody obj)
        {
            this.body = obj;
        }

        private void FixedUpdate()
        {
            if (!wasMonster || !master || !NetworkServer.active)
            {
                return;
            }

            if (!body || body.isPlayerControlled)
            {
                return;
            }

            if(master.teamIndex == TeamIndex.Monster)
            {
                return;
            }

            // handling chirr befriending mithrix
            // since we are now on team player and wasn't on player team before
            TryToDropEquipment(body.gameObject);

            wasMonster = false;
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

            if (!master.IsDeadAndOutOfLivesServer())
            {
                return;
            }

            var bodyObject = master.GetBodyObject();
            if (!bodyObject)
            {
                return;
            }

            TryToDropEquipment(bodyObject);
        }

        private void TryToDropEquipment(GameObject bodyObject)
        {
            bool itemFound = false;
            if (!itemToCheck)
            {
                itemFound = true;
            }
            else
            {
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

                    if (playerCharacterMaster.master.inventory.GetItemCount(itemToCheck) > 0)
                    {
                        itemFound = true;
                        break;
                    }
                }

                if (!itemFound)
                {
                    var returner = bodyObject.GetComponent<ReturnStolenItemsOnGettingHit>();
                    if (returner)
                    {
                        var itemStealController = returner.itemStealController;
                        if (itemStealController)
                        {
                            foreach (var stolenInfo in itemStealController.stolenInventoryInfos)
                            {
                                if (stolenInfo != null && stolenInfo.lentItemStacks != null
                                    && stolenInfo.lentItemStacks.Length > (int)Content.Items.LunarFlower.itemIndex
                                    && stolenInfo.lentItemStacks[(int)Content.Items.LunarFlower.itemIndex] > 0)
                                {
                                    itemFound = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            if (itemFound)
            {
                var vector = Vector3.up * 20f + transform.forward * 2f;
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipmentToDrop.equipmentIndex), bodyObject.transform.position, vector);
            }

            if (!string.IsNullOrEmpty(dropChatToken))
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = dropChatToken
                });
            }
        }
    }
}
