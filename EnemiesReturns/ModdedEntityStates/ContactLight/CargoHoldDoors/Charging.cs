using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Hologram;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.CargoHoldDoors
{
    [RegisterEntityState]
    public class Charging : BaseState
    {
        public static float graceTime = 3f;

        public static Material matTerminalYellow;

        public static string chargingSound = "Play_ui_obj_nullWard_activate";

        public static CostTypeDef costType => Content.CostTypes.AccessCard;

        public static int cost = 1;

        public static float graceExit = 35f; // 5 seconds longer than holdout zone

        private PurchaseInteraction purchaseInteraction;

        private HoldoutZoneController holdoutZoneController;

        private HologramProjector[] hologramProjectors;

        private CostTypeIndex costTypeIndex;

        private bool setCostType;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound(chargingSound, gameObject);

            costTypeIndex = Utils.GetCostTypeIndex(costType);
            purchaseInteraction = GetComponent<PurchaseInteraction>();
            hologramProjectors = gameObject.GetComponents<HologramProjector>();
            holdoutZoneController = GetComponent<HoldoutZoneController>();
            if (holdoutZoneController)
            {
                holdoutZoneController.enabled = true;
                holdoutZoneController.onCharged.AddListener(OnCharged);
            }
            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                if (matTerminalYellow)
                {
                    var terminal1 = childLocator.FindChild("Terminal1");
                    if (terminal1)
                    {
                        var renderer = terminal1.GetComponent<Renderer>();
                        if (renderer)
                        {
                            renderer.material = matTerminalYellow;
                        }
                    }

                    var terminal2 = childLocator.FindChild("Terminal2");
                    if (terminal2)
                    {
                        var renderer = terminal2.GetComponent<Renderer>();
                        if (renderer)
                        {
                            renderer.material = matTerminalYellow;
                        }
                    }
                }
            }

            var sfxLocator = GetComponent<SfxLocator>();
            if (sfxLocator)
            {
                Util.PlaySound(sfxLocator.openSound, gameObject);
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
            if (!setCostType && fixedAge > graceTime)
            {
                if (purchaseInteraction && costTypeIndex != CostTypeIndex.None)
                {
                    purchaseInteraction.costType = costTypeIndex;
                    purchaseInteraction.cost = cost;
                };

                if (NetworkServer.active)
                {
                    purchaseInteraction.SetAvailable(true);
                    purchaseInteraction.onDetailedPurchaseServer.AddListener(OnPurchasedWithKeyCard);
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
        }
    }
}
