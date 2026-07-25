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
        public static GameObject healNovaPrefab;

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

            if (healNovaPrefab)
            {
                var newObject = UnityEngine.Object.Instantiate(healNovaPrefab, this.transform.position, this.transform.rotation);
                newObject.GetComponent<TeamFilter>().teamIndex = characterBody.teamComponent.teamIndex;
                NetworkServer.Spawn(newObject);
            }

            purchaseCount++;
        }
    }
}
