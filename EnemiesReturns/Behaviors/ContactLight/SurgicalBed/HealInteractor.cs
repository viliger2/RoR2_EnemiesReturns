using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.ContactLight.SurgicalBed
{
    public class HealInteractor : NetworkBehaviour
    {
        public int maxPurchaseCount;

        private int purchaseCount;

        public void AddStack(Interactor interactor)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!interactor)
            {
                return;
            }

            if(!interactor.TryGetComponent<CharacterBody>(out var characterBody))
            {
                return;
            }

            if (!characterBody.healthComponent)
            {
                return;
            }

            characterBody.healthComponent.HealFraction(1, default);
            CleanseSystem.CleanseBodyServer(characterBody, true, false, true, true, true, false);

            purchaseCount++;
        }
    }
}
