using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Hologram;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.CargoHoldDoors
{
    [RegisterEntityState]
    public class Charging : BaseState
    {
        public static float graceTime = 3f;

        public static CostTypeDef costType => Content.CostTypes.AccessCard;

        public static int cost = 1;

        public static float graceExit = 60f;

        private PurchaseInteraction purchaseInteraction;

        private HoldoutZoneController holdoutZoneController;

        private HologramProjector[] hologramProjectors;

        private CostTypeIndex costTypeIndex;

        private bool setCostType;

        public override void OnEnter()
        {
            base.OnEnter();
            costTypeIndex = Utils.GetCostTypeIndex(costType);
            purchaseInteraction = GetComponent<PurchaseInteraction>();
            hologramProjectors = gameObject.GetComponents<HologramProjector>();
            holdoutZoneController = GetComponent<HoldoutZoneController>();
            if (holdoutZoneController)
            {
                holdoutZoneController.onCharged.AddListener(OnCharged);
            }
        }

        private void OnCharged(HoldoutZoneController controller)
        {
            if (isAuthority)
            {
                outer.SetNextState(new Opening());
            }
        }

        private void OnPurchasedWithKeyCard(CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults result)
        {
            if (isAuthority)
            {
                if (holdoutZoneController)
                {
                    holdoutZoneController.FullyChargeHoldoutZone();
                }
                outer.SetNextState(new Opening());
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!setCostType && fixedAge > graceTime && isAuthority)
            {
                if (isAuthority)
                {
                    if (purchaseInteraction && costTypeIndex != CostTypeIndex.None)
                    {
                        purchaseInteraction.costType = costTypeIndex;
                        purchaseInteraction.cost = cost;
                        purchaseInteraction.SetAvailable(true);
                        purchaseInteraction.onDetailedPurchaseServer.AddListener(OnPurchasedWithKeyCard);
                    };
                }
                if (hologramProjectors != null)
                {
                    foreach(var projector in hologramProjectors)
                    {
                        projector.enabled = true;
                    }
                }
                setCostType = true;
            }
            if(fixedAge > graceExit && isAuthority)
            {
                outer.SetNextState(new Opening());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (holdoutZoneController)
            {
                holdoutZoneController.onCharged.RemoveListener(OnCharged);
            }
            if (purchaseInteraction)
            {
                purchaseInteraction.onDetailedPurchaseServer.RemoveListener(OnPurchasedWithKeyCard);
            }
            if (hologramProjectors != null)
            {
                foreach (var projector in hologramProjectors)
                {
                    projector.enabled = false;
                }
            }
        }
    }
}
