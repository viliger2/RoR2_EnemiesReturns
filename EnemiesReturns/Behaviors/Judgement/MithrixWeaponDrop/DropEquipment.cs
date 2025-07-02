using RoR2;
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

            if (master.teamIndex == TeamIndex.Monster)
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

            TryToDropEquipment(body.gameObject);
        }

        private void TryToDropEquipment(GameObject bodyObject)
        {
            bool itemFound = false;
            if (LunarFlowerCheckerSingleton.instance)
            {
                itemFound = LunarFlowerCheckerSingleton.instance.haveFlower;
            }

            if (itemFound)
            {
                var vector = Vector3.up * 20f + transform.forward * 2f;
                PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(equipmentToDrop.equipmentIndex), bodyObject.transform.position, vector);

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
}
