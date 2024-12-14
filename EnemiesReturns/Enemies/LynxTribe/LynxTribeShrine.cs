using EnemiesReturns.Behaviors;
using JetBrains.Annotations;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe
{
    public class LynxTribeShrine : NetworkBehaviour, IInteractable
    {
        [SyncVar]
        public bool available;

        [SyncVar(hook = "OnPickupChanged")]
        private PickupIndex pickupIndex;

        public PickupDropTable dropTable;

        public Vector3 localEjectionVelocity = new Vector3(0f, 15f, 8f);

        public LynxTribeSpawner spawner;

        public PickupDisplay pickupDisplay;

        private NetworkIdentity networkIdentity;

        private void Awake()
        {
            available = true;
            networkIdentity = GetComponent<NetworkIdentity>();
        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                var rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
                pickupIndex = dropTable.GenerateDrop(rng);
                if (pickupDisplay)
                {
                    pickupDisplay.SetPickupIndex(pickupIndex);
                }
            }
        }

        public void OnPickupChanged(PickupIndex newPickupIndex)
        {
            this.pickupIndex = newPickupIndex;
            if (pickupDisplay)
            {
                pickupDisplay.SetPickupIndex(pickupIndex);
                if(pickupIndex == PickupIndex.none)
                {
                    pickupDisplay.enabled = false;
                }
            }
        }

        public string GetContextString([NotNull] Interactor activator)
        {
            return RoR2.Language.GetString("ENEMIES_RETURNS_LYNX_SHRINE_CONTEXT");
        }

        public Interactability GetInteractability([NotNull] Interactor activator)
        {
            if (!available)
            {
                return Interactability.Disabled;
            }

            return Interactability.Available;
        }

        public void OnInteractionBegin([NotNull] Interactor activator)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if(pickupIndex != PickupIndex.none)
            {
                PickupDropletController.CreatePickupDroplet(pickupIndex, pickupDisplay.transform.position, pickupDisplay.transform.rotation * localEjectionVelocity);
            }

            spawner.SpawnLynxTribesmen(activator.transform);

            pickupDisplay.SetPickupIndex(PickupIndex.none);
            pickupDisplay.enabled = false;
            pickupIndex = PickupIndex.none;

            if (networkIdentity)
            {
                networkIdentity.isPingable = false;
            }
            enabled = false;
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
            return true;
        }
    }
}
