using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.Newt
{
    public class NewtTrader : NetworkBehaviour, IInteractable
    {
        public string contextString;

        public ItemDef itemToTake;

        public ItemDef itemToGive;

        public string chatMessageOnInteraction;

        [SyncVar]
        public bool available;

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

            if(characterBody.inventory.GetItemCount(itemToTake) > 0)
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

            characterBody.inventory.RemoveItem(itemToTake, characterBody.inventory.GetItemCount(itemToTake));
            ScrapperController.CreateItemTakenOrb(characterBody.transform.position, this.gameObject, itemToTake.itemIndex);
            Invoke("SpawnPickupOrb", 1.5f);
            available = false;
        }

        public void SpawnPickupOrb()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var vector = Vector3.up * 20f + transform.forward * 8f;
            PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(itemToGive.itemIndex), this.gameObject.transform.position, vector);

            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = chatMessageOnInteraction,
            });
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
