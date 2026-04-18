using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.BonusRoomDoors
{
    [RegisterEntityState]
    public class Closed : BaseState
    {
        public static CostTypeDef costType => Content.CostTypes.AccessCard;

        public override void OnEnter()
        {
            base.OnEnter();
            var purchase = gameObject.GetComponent<PurchaseInteraction>();
            if (purchase && costType != null)
            {
                //purchase.costType = Utils.GetCostTypeIndex(costType);
            }
        }
    }
}
