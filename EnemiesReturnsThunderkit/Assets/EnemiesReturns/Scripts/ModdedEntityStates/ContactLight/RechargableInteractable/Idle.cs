using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.RechargableInteractable
{
    [RegisterEntityState]
    public class Idle : BaseRechargableInteractable
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (animator)
            {
                PlayAnimationOnAnimator(animator, "Base", "Ready");
            }
            if (isAuthority)
            {
                var purchaseInteraction = gameObject.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction)
                {
                    purchaseInteraction.SetAvailable(true);
                }
            }
        }
    }
}
