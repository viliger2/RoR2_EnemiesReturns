using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.ContactLight.SwordHilt
{
    public class SwordHiltInteractable : NetworkBehaviour, IInteractable
    {
        [SyncVar]
        public bool available;

        [Tooltip("Effect that is used when artifact mission is completed.")]
        public GameObject unlockEffect;

        public ItemDef itemToTake;

        public bool requireArtifactMission = true;

        public int requiredItemNumber;

        public string contextString;

        public string lockedString;

        public string unlockString;

        public EntityStateMachine esm;

        private int itemsGiven;

        private bool artifactMissionCompleted;

        private void OnEnable()
        {
            if (requireArtifactMission)
            {
                RoR2.ArtifactTrialMissionController.onShellDeathServer += ArtifactTrialMissionController_onShellDeathServer;
                artifactMissionCompleted = false;
            }
        }

        private void OnDisable()
        {
            if (requireArtifactMission)
            {
                RoR2.ArtifactTrialMissionController.onShellDeathServer -= ArtifactTrialMissionController_onShellDeathServer;
            }
        }

        private void ArtifactTrialMissionController_onShellDeathServer(ArtifactTrialMissionController arg1, DamageReport arg2)
        {
            //available = true;
            artifactMissionCompleted = true;
            if (NetworkServer.active)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = unlockString,
                });
                DisableLockedEffect();
                RpcDisableLockedEffect();
            }
        }

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

            if(requireArtifactMission && !artifactMissionCompleted)
            {
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = lockedString,
                });
                Invoke("UpdateAvailability", 1.5f);
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

            UpdateAvailability();
        }

        public void UpdateAvailability()
        {
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

        [ClientRpc]
        private void RpcDisableLockedEffect()
        {
            if (!NetworkServer.active)
            {
                DisableLockedEffect();
            }
        }

        private void DisableLockedEffect()
        {
            var modelLocator = GetComponent<ModelLocator>();
            if (!modelLocator || !modelLocator.modelTransform)
            {
                return;
            }

            var childLocator = modelLocator.modelTransform.gameObject.GetComponent<ChildLocator>();
            if (!childLocator)
            {
                return;
            }

            var lockedEffect = childLocator.FindChild("LockedEffect");
            if (lockedEffect)
            {
                lockedEffect.gameObject.SetActive(false);
                if (unlockEffect)
                {
                    EffectManager.SimpleEffect(unlockEffect, lockedEffect.position, lockedEffect.rotation, false);
                }               
            }
        }
    }
}
