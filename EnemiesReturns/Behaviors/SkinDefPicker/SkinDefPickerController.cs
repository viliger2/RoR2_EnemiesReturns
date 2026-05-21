using EnemiesReturns.Components;
using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using static UnityEngine.ResourceManagement.ResourceProviders.AssetBundleResource;

namespace EnemiesReturns.Behaviors.SkinDefPicker
{
    [RequireComponent(typeof(NetworkUIPromptController))]
    public class SkinDefPickerController : NetworkBehaviour, IInteractable
    {
        public struct Option
        {
            public SkinDef skin;

            public bool available;
        }

        [Serializable]
        public class SkinDefIndexUnityEvent : UnityEvent<SkinDef> { }

        public GameObject panelPrefab;

        public SkinDefIndexUnityEvent onPickupSelected;

        public GenericInteraction.InteractorUnityEvent onServerInteractionBegin;

        public float cutoffDistance;

        public string contextString = "";

        public bool shouldProximityHightlight = true;

        public bool allowRemoteOpInteraction = false;

        protected NetworkUIPromptController networkUIPromptController;

        protected GameObject panelInstance;

        protected SkinDefPickerPanel panelInstanceController;

        private EventFunctions eventFunctions;

        public Option[] options = Array.Empty<Option>();

        public bool available;

        private const uint AVAILABLE_DIRTY_BIT = 1u;

        private const uint OPTIONS_DIRTY_BIT = 2u;

        private Interactor interactor;

        private void Awake()
        {
            networkUIPromptController = GetComponent<NetworkUIPromptController>();
            if (NetworkClient.active)
            {
                networkUIPromptController.onDisplayBegin += NetworkUIPromptController_onDisplayBegin;
                networkUIPromptController.onDisplayEnd += NetworkUIPromptController_onDisplayEnd;
            }
            if (NetworkServer.active)
            {
                networkUIPromptController.messageFromClientHandler = HandleClientMessage;
            }
            eventFunctions = GetComponent<EventFunctions>();
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                var currentMaster = networkUIPromptController.currentParticipantMaster;
                if (currentMaster)
                {
                    var currentBody = currentMaster.GetBody();
                    if (!currentBody || (currentBody.inputBank.aimOrigin - transform.position).sqrMagnitude > cutoffDistance * cutoffDistance)
                    {
                        networkUIPromptController.SetParticipantMaster(null);
                    }
                }
            }
        }

        protected void OnPanelDestroyed(OnDestroyCallback onDestroyCallback)
        {
            NetworkWriter networkWriter = networkUIPromptController.BeginMessageToServer();
            networkWriter.Write((byte)1);
            networkUIPromptController.FinishMessageToServer(networkWriter);
        }

        private void OnEnable()
        {
            InstanceTracker.Add(this);
        }

        private void OnDisable()
        {
            InstanceTracker.Remove(this);
        }

        protected virtual void HandleClientMessage(NetworkReader reader)
        {
            switch (reader.ReadByte())
            {
                case 0:
                    {
                        int choiceIndex = reader.ReadInt32();
                        HandlePickupSelected(choiceIndex);
                        break;
                    }
                case 1:
                    networkUIPromptController.SetParticipantMaster(null);
                    break;
            }
        }


        private void NetworkUIPromptController_onDisplayEnd(NetworkUIPromptController arg1, LocalUser arg2, CameraRigController arg3)
        {
            Destroy(panelInstance);
            panelInstance = null;
            panelInstanceController = null;
        }

        private void NetworkUIPromptController_onDisplayBegin(NetworkUIPromptController networkUIPromptController, LocalUser localUser, CameraRigController cameraRigController)
        {
            panelInstance = Instantiate(panelPrefab, cameraRigController.hud.mainContainer.transform);
            panelInstanceController = panelInstance.GetComponent<SkinDefPickerPanel>();
            panelInstanceController.pickerController = this;
            panelInstanceController.SetPickupOptions(options);
            OnDestroyCallback.AddCallback(panelInstance, OnPanelDestroyed);
        }

        public void SetAvailable(bool newAvailable)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void EnemiesReturns.Behaviors.SkinDefPickerController::SetAvailable(System.Boolean)' called on client");
                return;
            }
            available = newAvailable;
            SetDirtyBit(AVAILABLE_DIRTY_BIT);
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            uint num = syncVarDirtyBits;
            if (initialState)
            {
                num = AVAILABLE_DIRTY_BIT | OPTIONS_DIRTY_BIT;
            }

            writer.WritePackedUInt32(num);
            if ((num & AVAILABLE_DIRTY_BIT) != 0)
            {
                writer.Write(available);
            }
            if ((num & OPTIONS_DIRTY_BIT) != 0)
            {
                writer.WritePackedUInt32((uint)options.Length);
                for (int i = 0; i < options.Length; i++)
                {
                    writer.WritePackedUInt32((uint)options[i].skin.skinIndex);
                    writer.Write(options[i].available);
                }
            }

            return num != 0;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            uint num = reader.ReadPackedUInt32();
            if ((num & AVAILABLE_DIRTY_BIT) != 0)
            {
                available = reader.ReadBoolean();
            }
            if ((num & OPTIONS_DIRTY_BIT) != 0)
            {
                var options = new Option[reader.ReadPackedUInt32()];
                for (int i = 0; i < options.Length; i++)
                {
                    ref Option reference = ref options[i];
                    reference.skin = SkinCatalog.GetSkinDef((SkinIndex)reader.ReadPackedUInt32());
                    reference.available = reader.ReadBoolean();
                }
                SetOptionsInternal(options);
            }
        }

        private void SetOptionsInternal(Option[] newOptions)
        {
            Array.Resize(ref options, newOptions.Length);
            Array.Copy(newOptions, options, newOptions.Length);
            if (panelInstanceController)
            {
                panelInstanceController.SetPickupOptions(options);
            }
            if (NetworkServer.active)
            {
                SetDirtyBit(OPTIONS_DIRTY_BIT);
            }
        }

        public void AssignPotentialInteractor(Interactor potentialInteractor)
        {
            if (!NetworkServer.active)
            {
                Log.Warning("[Server] function 'System.Void EnemiesReturns.Behaviors.SkinDefPickerController::AssignPotentialInteractor(RoR2.Interactor)' called on client");
            }
            else
            {
                interactor = potentialInteractor;
            }
        }

        public void SetSkinPickerOptions(Interactor interactor)
        {
            if (!NetworkServer.active)
            {
                Log.Warning("[Server] function 'System.Void EnemiesReturns.Behaviors.SkinDefPickerController::SetSkinPickerOptions(RoR2.Interactor)' called on client");
            }
            else
            {
                SetOptionsFromInteractor(interactor);
            }
        }

        private void SetOptionsFromInteractor(Interactor interactor)
        {
            if (!interactor)
            {
                return;
            }

            var body = interactor.GetComponent<CharacterBody>();
            if (!body || !body.modelLocator || !body.modelLocator.modelTransform)
            {
                return;
            }

            var modelSkinController = body.modelLocator.modelTransform.GetComponent<ModelSkinController>();
            if (!modelSkinController)
            {
                return;
            }

            var skins = modelSkinController.skins;

            if (skins.Length == 0)
            {
                return;
            }

            NetworkUser networkUser = Util.LookUpBodyNetworkUser(interactor.gameObject);
            if (!networkUser)
            {
                return;
            }

            List<Option> options = new List<Option>();

            for (int i = 0; i < skins.Length; i++)
            {
                // TODO: maybe just lock unavailable skins instead of hiding them?
                if (skins[i].unlockableDef)
                {
                    if (!(networkUser.localUser?.userProfile?.HasUnlockable(skins[i].unlockableDef) ?? networkUser.unlockables.Contains(skins[i].unlockableDef)))
                    {
                        continue;
                    }
                }

                if (i == body.skinIndex)
                {
                    continue;
                }

                options.Add(new Option
                {
                    available = true,
                    skin = skins[i]
                });
            }

            SetOptionsInternal(options.ToArray());
        }

        public void SwapSkin(SkinDef newSkin)
        {
            if (!NetworkServer.active)
            {
                Log.Warning("[Server] function 'System.Void EnemiesReturns.Behaviors.SkinDefPickerController::SwapSkin(SkinDef)' called on client");
                return;
            }

            if (!interactor)
            {
                return;
            }

            var body = interactor.GetComponent<CharacterBody>();
            if (!body || !body.master)
            {
                return;
            }

            var localSkinIndex = SkinCatalog.FindLocalSkinIndexForBody(body.bodyIndex, newSkin);
            if (localSkinIndex < 0)
            {
                return;
            }

            body.master.loadout.bodyLoadoutManager.SetSkinIndex(body.bodyIndex, (uint)localSkinIndex);
            body.master.SetDirtyBit(CharacterMaster.loadoutDirtyBit);
            body.skinIndex = (uint)localSkinIndex;

            RpcSwapSkin(localSkinIndex, body.netId);
        }

        [ClientRpc]
        public void RpcSwapSkin(int localSkinIndex, NetworkInstanceId netId)
        {
            if (!NetworkClient.active)
            {
                return;
            }

            var characterBodyObject = Util.FindNetworkObject(netId);
            if (!characterBodyObject)
            {
                return;
            }

            var characterBody = characterBodyObject.GetComponent<CharacterBody>();
            if (!characterBody || !characterBody.master)
            {
                return;
            }

            characterBody.skinIndex = (uint)(localSkinIndex);

            StartCoroutine(characterBody.modelLocator.modelTransform.GetComponent<ModelSkinController>().ApplySkinAsync(localSkinIndex, RoR2.ContentManagement.AsyncReferenceHandleUnloadType.OnSceneUnload));

#if DEBUG || NOWEAVER
            Log.Info($"recieved skin index {localSkinIndex}, skin index from loadout {characterBody.master.loadout.bodyLoadoutManager.GetSkinIndex(characterBody.bodyIndex)}");
#endif
        }

        public void SubmitChoice(int choiceIndex)
        {
            if (!NetworkServer.active)
            {
                NetworkWriter networkWriter = networkUIPromptController.BeginMessageToServer();
                networkWriter.Write((byte)0);
                networkWriter.Write(choiceIndex);
                networkUIPromptController.FinishMessageToServer(networkWriter);
            }
            else
            {
                HandlePickupSelected(choiceIndex);
            }
        }

        protected virtual void HandlePickupSelected(int choiceIndex)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void EnemiesReturns.Behaviors.SkinDefPickerController::HandlePickupSelected(System.Int32)' called on client");
            }
            else
            {
                if (choiceIndex >= options.Length)
                {
                    return;
                }
                ref Option reference = ref options[choiceIndex];
                if (reference.available)
                {
                    onPickupSelected?.Invoke(reference.skin);
                }
            }
        }

        public string GetContextString([NotNull] Interactor activator)
        {
            return RoR2.Language.GetString(contextString);
        }

        public Interactability GetInteractability([NotNull] Interactor activator)
        {
            if (activator.isRemoteOp && !allowRemoteOpInteraction)
            {
                return Interactability.Disabled;
            }

            if (networkUIPromptController.inUse)
            {
                return Interactability.ConditionsNotMet;
            }

            if (!available)
            {
                return Interactability.Disabled;
            }

            return Interactability.Available;
        }

        public void OnInteractionBegin([NotNull] Interactor activator)
        {
            if (enabled)
            {
                onServerInteractionBegin?.Invoke(activator);
                networkUIPromptController.SetParticipantMasterFromInteractor(activator);
            }
        }

        public bool ShouldIgnoreSpherecastForInteractibility([NotNull] Interactor activator)
        {
            return false;
        }

        public bool ShouldProximityHighlight()
        {
            return shouldProximityHightlight;
        }

        public bool ShouldShowOnScanner()
        {
            return true;
        }
    }
}
