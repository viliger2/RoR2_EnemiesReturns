using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class PostFight : BaseState
    {
        public static string phaseControllerChildString = "PostFight";

        public static InteractableSpawnCard portalBazaar = Addressables.LoadAssetAsync<InteractableSpawnCard>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_PortalShop.iscShopPortal_asset).WaitForCompletion();

        public static event Action onProvidenceDefeated;

        public static float consoleAvailable = 3f;

        public static float portalSpawns = 6f;

        public static float doorUnlocks = 9f;

        private GameObject closedDoor;

        private GameObject console;

        private Transform portalLocation;

        private bool enabledConsole;

        private bool spawnedPortal;

        private bool unlockedDoor;

        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active)
            {
                onProvidenceDefeated?.Invoke();
            }

            if (RoR2.Run.instance)
            {
                RoR2.Run.instance.SetEventFlag(Enemies.ContactLight.SetupContactLight.PROVIDENCE_FLAG);
            }

            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var phaseObject = childLocator.FindChild(phaseControllerChildString);
                if (phaseObject)
                {
                    childLocator = phaseObject.GetComponent<ChildLocator>();
                    if (childLocator)
                    {
                        var closedDoor = childLocator.FindChild("ClosedDoor");
                        if (closedDoor)
                        {
                            this.closedDoor = closedDoor.gameObject;
                        }

                        var console = childLocator.FindChild("Console");
                        if (console)
                        {
                            this.console = console.gameObject;
                        }

                        portalLocation = childLocator.FindChild("PortalSpawnLocation");
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > consoleAvailable && console && !enabledConsole)
            {
                ModifyConsole(console);
                enabledConsole = true;
            }
            if(fixedAge > portalSpawns && portalLocation && !spawnedPortal)
            {
                OpenPortal(portalBazaar, portalLocation.position, portalLocation.localRotation.eulerAngles);
                spawnedPortal = true;
            }
            if(fixedAge > doorUnlocks && closedDoor && !unlockedDoor)
            {
                ModifyClosedDoor(closedDoor);
                unlockedDoor = true;
            }
        }

        private void ModifyClosedDoor(GameObject closedDoor)
        {
            var esm = closedDoor.GetComponent<EntityStateMachine>();
            if (esm && Util.HasEffectiveAuthority(closedDoor))
            {
                esm.SetNextState(new ModdedEntityStates.ContactLight.CargoHoldDoors.ClosedWithKeycard());
            }
        }

        private void ModifyConsole(GameObject console)
        {
            var esm = console.GetComponent<EntityStateMachine>();
            if (esm && Util.HasEffectiveAuthority(console))
            {
                esm.SetNextState(new ModdedEntityStates.ContactLight.Console.WaitForGameEnd());
            }
        }

        private void OpenPortal(InteractableSpawnCard portalCard, Vector3 position, Vector3 rotation)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!Run.instance)
            {
                return;
            }

            if (DirectorCore.instance)
            {
                var obj = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(portalCard, new DirectorPlacementRule()
                {
                    placementMode = DirectorPlacementRule.PlacementMode.DirectWithoutRandomRotation,
                    position = position,
                    rotation = Quaternion.Euler(rotation)
                },
                    Run.instance.stageRng)
                );
                if (obj)
                {
                    Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                    {
                        baseToken = "PORTAL_SHOP_OPEN"
                    });
                }
            }
        }
    }
}
