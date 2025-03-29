using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.BrokenTeleporter
{
    public class BrokenTeleporterInteraction : NetworkBehaviour, IInteractable
    {
        [SyncVar]
        public bool available;

        public string contextString;

        public ItemDef requiredItem;

        public Transform portalSpawnLocation;

        public GameObject portalPrefab;

        private ChildLocator childLocator;

        private void Awake()
        {
            childLocator = GetComponent<ChildLocator>();
        }

        public string GetContextString([NotNull] Interactor activator)
        {
            return contextString;
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

            if (characterBody.inventory.GetItemCount(requiredItem) > 0)
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

            if(!portalSpawnLocation || !portalPrefab)
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

            characterBody.inventory.RemoveItem(requiredItem, characterBody.inventory.GetItemCount(requiredItem));
            ScrapperController.CreateItemTakenOrb(characterBody.transform.position, this.gameObject, requiredItem.itemIndex);
            Invoke("EnableVisuals", 1.5f);

            available = false;
        }

        public void EnableVisuals()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            EnableProngsAndFlower();
            RpcEnableProngsAndFlower();

            Invoke("SpawnPortal", 6f);
        }

        [ClientRpc]
        public void RpcEnableProngsAndFlower()
        {
            EnableProngsAndFlower();
        }

        private void EnableProngsAndFlower()
        {
            if (childLocator)
            {
                var flower = childLocator.FindChild("Flower");
                if (flower)
                {
                    flower.gameObject.SetActive(true);
                }
                var prongs = childLocator.FindChild("Prongs");
                if (prongs)
                {
                    prongs.gameObject.SetActive(true);
                }
            }
        }

        public void SpawnPortal()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var portal = UnityEngine.Object.Instantiate(portalPrefab, portalSpawnLocation.position, portalSpawnLocation.rotation);
            NetworkServer.Spawn(portal);
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
