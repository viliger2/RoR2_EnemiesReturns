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
    public class ClosedWithKeycard : BaseState
    {
        public static CostTypeDef costType => Content.CostTypes.AccessCard;

        public static int cost = 1;

        public static Material matTerminalYellow;

        private PurchaseInteraction purchaseInteraction;

        public override void OnEnter()
        {
            base.OnEnter();

            var costTypeIndex = Utils.GetCostTypeIndex(costType);
            purchaseInteraction = GetComponent<PurchaseInteraction>();
            var hologramProjectors = gameObject.GetComponents<HologramProjector>();

            if (purchaseInteraction)
            {
                purchaseInteraction.costType = costTypeIndex;
                purchaseInteraction.cost = cost;
                purchaseInteraction.SetAvailable(true);
            };

            if (NetworkServer.active)
            {
                purchaseInteraction.SetAvailable(true);
                purchaseInteraction.onDetailedPurchaseServer = new DetailedPurchaseEvent(); // to remove existing listeners
                purchaseInteraction.onDetailedPurchaseServer.AddListener(OnPurchasedWithKeyCard);
            }

            if (hologramProjectors != null)
            {
                foreach (var projector in hologramProjectors)
                {
                    projector.enabled = true;
                }
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

            if (NetworkServer.active)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "ENEMIES_RETURNS_CONTACTLIGHT_CARGO_DOOR_AWAITS_KEYCARD"
                });
            }
        }

        private void OnPurchasedWithKeyCard(CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults result)
        {
            if (isAuthority)
            { 
                outer.SetNextState(new Opening());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (purchaseInteraction)
            {
                purchaseInteraction.onDetailedPurchaseServer.RemoveListener(OnPurchasedWithKeyCard);
            }
        }
    }
}
