using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.MechanicalSpider.Drone
{
    public class MechanicalSpiderDroneOnPurchaseEvents : MonoBehaviour
    {
        public PurchaseInteraction purchaseInteraction;

        public SummonMasterBehavior summonMasterBehavior;

        public EventFunctions eventFunctions;

        public Inventory inventory;

        public static float initialStageDifficultyCoefficient { get; private set; } // this should be in purchase component but that thing is ror2 so we just gonna put it here

        private void Awake()
        {
            if (!purchaseInteraction)
            {
                purchaseInteraction = GetComponent<PurchaseInteraction>();
                if (!purchaseInteraction)
                {
                    return;
                }
            }

            if (!summonMasterBehavior)
            {
                summonMasterBehavior = GetComponent<SummonMasterBehavior>();
            }

            if (!eventFunctions)
            {
                eventFunctions = GetComponent<EventFunctions>();
            }

            if (!inventory)
            {
                inventory = GetComponent<Inventory>();
            }

            purchaseInteraction.onDetailedPurchaseServer.AddListener(OnDetailedPurchase);
            //purchaseInteraction.onPurchase.AddListener(OnPurchase);
        }

        public void OnDetailedPurchase(CostTypeDef.PayCostContext payCostContext, CostTypeDef.PayCostResults payCostResult)
        {
            purchaseInteraction.SetAvailable(false);
            if (!NetworkServer.active)
            {
                return;
            }

            if (summonMasterBehavior)
            {
                var master = summonMasterBehavior.OpenSummonReturnMaster(payCostContext.activator);
                if (master && master.inventory && inventory)
                {
                    master.inventory.CopyEquipmentFrom(inventory, true);
                    master.inventory.AddItemsFrom(inventory);
                    GiveMinionItems(master.inventory);
                }
            }

            if (eventFunctions)
            {
                eventFunctions.DestroySelf();
            }
        }

        private void GiveMinionItems(Inventory inventory)
        {
            inventory.GiveItemPermanent(RoR2Content.Items.MinionLeash, 1);
            inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, Configuration.MechanicalSpider.DroneBonusHP.Value);
            inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage, Configuration.MechanicalSpider.DroneBonusDamage.Value);
            if (ModCompats.RiskyModCompat.enabled)
            {
                inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyMarker, 1);
                inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyScaling, 1);
                inventory.GiveItemPermanent(ModCompats.RiskyModCompat.RiskyModAllyRegen, 40);
            }
        }

        public static void Hooks()
        {
            SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }

        private static void SceneDirector_onPostPopulateSceneServer(SceneDirector obj)
        {
            initialStageDifficultyCoefficient = Run.instance.difficultyCoefficient;
        }
    }
}
