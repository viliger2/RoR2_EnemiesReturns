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
                switch (pickupIndex.pickupDef.itemTier) // TODO: config
                {
                    case ItemTier.Tier1:
                        spawner.minSpawnCount = 2;
                        spawner.maxSpawnCount = 3;
                        spawner.eliteBias = 1f;
                        break;
                    case ItemTier.Tier2:
                        spawner.minSpawnCount = 3;
                        spawner.maxSpawnCount = 4;
                        spawner.eliteBias = 0.75f;
                        break;
                    case ItemTier.Tier3:
                        spawner.minSpawnCount = 5;
                        spawner.maxSpawnCount = 6;
                        spawner.eliteBias = 0.4f;
                        break;
                    case ItemTier.Boss:
                        spawner.minSpawnCount = 4;
                        spawner.maxSpawnCount = 5;
                        spawner.eliteBias = 0.5f;
                        break;
                }
                spawner.CreateSpawnInfo();
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
