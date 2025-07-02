using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.MithrixWeaponDrop
{
    public class LunarFlowerCheckerSingleton : MonoBehaviour
    {
        public static LunarFlowerCheckerSingleton instance;

        public ItemDef itemToCheck;

        public bool haveFlower { get; private set; } = false;

        private void Awake()
        {
            instance = SingletonHelper.Assign(instance, this);
        }

        private void OnDestroy()
        {
            instance = SingletonHelper.Unassign(instance, this);
        }

        public void CheckForFlower()
        {
            if (!NetworkServer.active)
            {
                return;
            }

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
                    haveFlower = true;
                    return;
                }
            }
        }

    }
}
