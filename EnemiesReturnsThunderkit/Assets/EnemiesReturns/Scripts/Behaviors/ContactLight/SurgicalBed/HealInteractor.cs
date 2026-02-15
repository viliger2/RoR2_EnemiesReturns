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

        public float refreshDuration;

        private int purchaseCount;

        private float refreshTimer;

        private bool waitingForRefresh;

        private PurchaseInteraction purchaseInteraction;

        private void FixedUpdate()
        {
            if (waitingForRefresh)
            {
                refreshTimer -= Time.fixedDeltaTime;
                if (refreshTimer <= 0f && purchaseCount < maxPurchaseCount)
                {
                    purchaseInteraction.SetAvailable(newAvailable: true);
                    waitingForRefresh = false;
                }
            }
        }

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

            characterBody.healthComponent.HealFraction(100, default);
            Util.CleanseBody(characterBody, true, false, true, true, true, false);

            purchaseCount++;
            if(purchaseCount >= maxPurchaseCount)
            {
                // do something, I dunoo
            } else
            {
                refreshTimer += refreshDuration;
                waitingForRefresh = true;
            }
        }
    }
}
