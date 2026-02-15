using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine.Networking;
using static RoR2.CharacterMasterNotificationQueue;

namespace EnemiesReturns.Behaviors.ContactLight.KeyCardDoors
{
    public class KeycardDoorInteractable : NetworkBehaviour, IInteractable, IInspectable, IDisplayNameProvider
    {
        public ItemDef requiredItem;

        public string contextString;

        public string displayNameString;

        public UnityEvent<Interactor> onInteractionServer;

        public UnityEvent onInteractionClient;

        [SyncVar]
        private bool available = true;

        private ChildLocator childLocator;

        private void Awake()
        {
            childLocator = GetComponent<ChildLocator>();
        }

        public string GetContextString([NotNull] Interactor activator)
        {
            return RoR2.Language.GetString(contextString);
        }

        public string GetDisplayName()
        {
            return RoR2.Language.GetString(displayNameString);
        }

        public IInspectInfoProvider GetInspectInfoProvider()
        {
            return GetComponent<IInspectInfoProvider>();
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

            if (characterBody.inventory.GetItemCountPermanent(requiredItem) > 0)
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

            var targetObject = this.gameObject;
            if (childLocator)
            {
                var target = childLocator.FindChild("TargetForItemOrb");
                if (target)
                {
                    targetObject = target.gameObject;
                }
            }

            Inventory.ItemTransformation itemTransformation = default(Inventory.ItemTransformation);
            itemTransformation.originalItemIndex = requiredItem.itemIndex;
            itemTransformation.transformationType = (ItemTransformationTypeIndex)TransformationType.None;
            if(itemTransformation.TryTake(characterBody.inventory, out var result))
            {
                ScrapperController.CreateItemTakenOrb(characterBody.transform.position, targetObject, requiredItem.itemIndex);
                onInteractionServer?.Invoke(activator);
                RpcInvokeOnInteractionClient();

                available = false;
            }
        }

        [ClientRpc]
        private void RpcInvokeOnInteractionClient()
        {
            onInteractionClient?.Invoke();
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
