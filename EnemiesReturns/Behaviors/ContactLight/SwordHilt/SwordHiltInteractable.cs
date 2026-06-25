using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.ContactLight.SwordHilt
{
    public class SwordHiltInteractable : NetworkBehaviour, IInteractable
    {
        [SyncVar]
        public bool available;

        public ItemDef itemToTake;

        public int requiredItemNumber;

        public string contextString;

        public EntityStateMachine esm;

        private int itemsGiven;

        public string GetContextString([NotNull] Interactor activator)
        {
            return RoR2.Language.GetString(contextString);
        }

        public Interactability GetInteractability([NotNull] Interactor activator)
        {
            if (!available)
            {
                return Interactability.Disabled;
            }

            if (!activator || !activator.TryGetComponent<CharacterBody>(out var characterBody))
            {
                return Interactability.Disabled;
            }

            if (!characterBody.inventory)
            {
                return Interactability.Disabled;
            }

            if (characterBody.inventory.GetItemCountPermanent(itemToTake) > 0)
            {
                return Interactability.Available;
            }

            return Interactability.Disabled;
        }

        public void OnInteractionBegin([NotNull] Interactor activator)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!activator || !activator.TryGetComponent<CharacterBody>(out var characterBody))
            {
                return;
            }

            if (!characterBody.inventory)
            {
                return;
            }

            var itemTakenCount = characterBody.inventory.GetItemCountPermanent(itemToTake);
            itemsGiven += itemTakenCount;

            characterBody.inventory.RemoveItemPermanent(itemToTake, itemTakenCount);
            ScrapperController.CreateItemTakenOrb(characterBody.transform.position, this.gameObject, itemToTake.itemIndex);
            Invoke("UpdateActiveSwordShards", 1.5f);
            available = false;
        }

        public void UpdateActiveSwordShards()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (esm)
            {
                esm.SetNextState(new ModdedEntityStates.ContactLight.SwordHilt.Idle() { activeShardsCount = itemsGiven});
            }

            available = itemsGiven < requiredItemNumber;
        }

        public bool ShouldIgnoreSpherecastForInteractibility([NotNull] Interactor activator)
        {
            return false;
        }

        public bool ShouldProximityHighlight()
        {
            return true;
        }

        public bool ShouldShowOnScanner()
        {
            return false;
        }
    }
}
