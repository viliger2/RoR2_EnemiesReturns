using RoR2;
using System;
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
                wasMonster = master.teamIndex != TeamIndex.Player;
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

            if (master.teamIndex != TeamIndex.Player)
            {
                return;
            }

            if (master.inventory.GetItemCount(RoR2Content.Items.Ghost) > 0)
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

            if (!master || !body)
            {
                return;
            }

            if (!master.IsDeadAndOutOfLivesServer())
            {
                return;
            }

            if(master.inventory.GetItemCount(RoR2Content.Items.Ghost) > 0)
            {
                return;
            }

            TryToDropEquipment(body.gameObject);
        }

        private void TryToDropEquipment(GameObject bodyObject)
        {
            bool itemFound = false;
            if (LunarFlowerCheckerSingleton.instance)
            {
                itemFound = LunarFlowerCheckerSingleton.instance.haveFlower;
            }

            if (!itemFound)
            {
                foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
                {
                    if (!playerCharacterMaster.isConnected || !playerCharacterMaster.master)
                    {
                        continue;
                    }

                    if (!playerCharacterMaster.master.inventory)
                    {
                        continue;
                    }

                    if (playerCharacterMaster.master.inventory.GetItemCount(itemToCheck) > 0)
                    {
                        itemFound = true;
                        break;
                    }
                }
            }

            if (itemFound)
            {
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipmentToDrop.equipmentIndex), bodyObject.transform.position, GetItemDropVelocity(bodyObject.transform.position));

                if (!string.IsNullOrEmpty(dropChatToken))
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = dropChatToken
                    });
                }
            }
        }

        private Vector3 GetItemDropVelocity(Vector3 origin)
        {
            var vector3 = Vector3.up * 20f + transform.forward * 2f;

            if (VoidRaidGauntletController.instance)
            {
                var donut = VoidRaidGauntletController.instance.currentDonut;
                var desiredDropPosition = donut.crabPosition.position;

                vector3 = Utils.CalculateLaunchVelocityForRigidBody(origin, desiredDropPosition, 4f);
            }

            return vector3;
        }


    }
}
