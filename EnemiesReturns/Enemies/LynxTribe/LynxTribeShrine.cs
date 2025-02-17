using EnemiesReturns.Behaviors;
using JetBrains.Annotations;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe
{
    public class LynxTribeShrine : NetworkBehaviour, IInteractable, IHologramContentProvider, IInspectable
    {
        public GameObject shrineUseEffect;

        [SyncVar]
        public bool available;

        [SyncVar(hook = "OnPickupChanged")]
        public int pickupValue;

        [SyncVar]
        public bool activated = false;

        public float escapeDuration = 60f;

        public PickupIndex pickupIndex;

        public PickupDropTable dropTable;

        public Vector3 localEjectionVelocity = new Vector3(0f, 15f, 8f);

        public LynxTribeSpawner spawner;

        public NetworkSoundEventDef failureSound;

        public NetworkSoundEventDef successSound;

        public PickupDisplay pickupDisplay;

        private NetworkIdentity networkIdentity;

        private float escapeTimer;

        private void Awake()
        {
            available = true;
            networkIdentity = GetComponent<NetworkIdentity>();
            if (spawner && spawner.combatSquad)
            {
                spawner.combatSquad.onDefeatedServer += CombatSquad_onDefeatedServer;
            }
        }

        private void CombatSquad_onDefeatedServer()
        {
            if (pickupIndex != PickupIndex.none)
            {
                DropItems();
            }
            activated = false;
            DisableItemDisplay();
            pickupValue = pickupIndex.value;
            PointSoundManager.EmitSoundServer(successSound.index, base.transform.position);
        }

        private void DropItems()
        {
            if (Run.instance)
            {
                int participatingPlayerCount = Run.instance.participatingPlayerCount;
                if (participatingPlayerCount == 0)
                {
                    return;
                }

                var angle = 180f / (participatingPlayerCount + 1); // plus 1 so we split into equal parts and spawn between each
                var quaternion = UnityEngine.Quaternion.AngleAxis(angle, Vector3.up);
                var vector = quaternion * pickupDisplay.transform.rotation * localEjectionVelocity;
                int spawnedCount = 0;
                while (spawnedCount < participatingPlayerCount)
                {
                    PickupDropletController.CreatePickupDroplet(pickupIndex, pickupDisplay.transform.position, vector);
                    spawnedCount++;
                    vector = quaternion * vector;
                }
            }
        }

        private void DisableItemDisplay()
        {
            pickupDisplay.SetPickupIndex(PickupIndex.none);
            pickupDisplay.enabled = false;
            pickupIndex = PickupIndex.none;
        }

        private void OnEnable()
        {
            InstanceTracker.Add(this);
        }

        private void OnDisable()
        {
            InstanceTracker.Remove(this);
        }

        private void Start()
        {
            if (NetworkServer.active)
            {
                var rng = new Xoroshiro128Plus(Run.instance.treasureRng.nextUlong);
                pickupIndex = dropTable.GenerateDrop(rng);
                pickupValue = pickupIndex.value;
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

        public void OnPickupChanged(int newPickupValue)
        {
            this.pickupValue = newPickupValue;
            this.pickupIndex = new PickupIndex(this.pickupValue);

            if (pickupDisplay)
            {
                pickupDisplay.SetPickupIndex(pickupIndex);
                if (pickupIndex == PickupIndex.none)
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

            if (shrineUseEffect)
            {
                EffectManager.SpawnEffect(shrineUseEffect, new EffectData
                {
                    origin = base.transform.position,
                    rotation = Quaternion.identity,
                    scale = 1f,
                    color = Color.yellow
                }, transmit: true);
            }
            spawner.SpawnLynxTribesmen(activator.transform);
            activated = true;
            if (networkIdentity)
            {
                networkIdentity.isPingable = false;
            }
            available = false;
        }

        private void FixedUpdate()
        {
            if (activated)
            {
                if (escapeTimer > escapeDuration)
                {
                    if (spawner && spawner.combatSquad)
                    {
                        spawner.combatSquad.onDefeatedServer -= CombatSquad_onDefeatedServer;
                    }
                    if (NetworkServer.active)
                    {
                        spawner.Escape();
                        if (spawner.combatSquad && spawner.combatSquad.memberCount > 0)
                        {
                            var bodyObject = spawner.combatSquad.membersList[0].GetBodyObject();
                            if (bodyObject)
                            {
                                ScrapperController.CreateItemTakenOrb(pickupDisplay.transform.position, bodyObject, pickupIndex.pickupDef.itemIndex);
                            }
                        }
                        PointSoundManager.EmitSoundServer(failureSound.index, base.transform.position);
                    }

                    DisableItemDisplay();
                    activated = false;
                }
                escapeTimer += Time.fixedDeltaTime;
            }
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

        public bool ShouldDisplayHologram(GameObject viewer)
        {
            return activated;
        }

        public GameObject GetHologramContentPrefab()
        {
            return LynxTribeStuff.CustomHologramContent;
        }

        public void UpdateHologramContent(GameObject hologramContentObject, Transform viewerBody)
        {
            var component = hologramContentObject.GetComponent<CustomCostHologramContent>();
            if (component)
            {
                component.displayValue = escapeDuration - escapeTimer;
            }
        }

        public IInspectInfoProvider GetInspectInfoProvider()
        {
            return GetComponent<IInspectInfoProvider>();
        }
    }
}
