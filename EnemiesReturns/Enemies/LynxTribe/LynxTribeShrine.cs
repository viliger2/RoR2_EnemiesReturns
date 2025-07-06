using EnemiesReturns.Behaviors;
using JetBrains.Annotations;
using RoR2;
using RoR2.Audio;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe
{
    // TODO: implement item text on ping in chat
    // this would require somewhat complex il hook that I am not willing to do, so no chat message for now
    // TODO: redo network sync of pickupindex, maybe just request it from the server until you get it
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

        private CharacterBody interactor;

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
                var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                Chat.SendBroadcastChat(new LynxShrineChatMessage
                {
                    subjectAsCharacterBody = this.interactor,
                    baseToken = "ENEMIES_RETURNS_LYNX_SHRINE_INTERACT_SUCCESS",
                    pickupToken = pickupDef?.nameToken ?? PickupCatalog.invalidPickupToken,
                    pickupColor = pickupDef?.baseColor ?? Color.black,
                    pickupQuantity = 0
                });
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
                int participatingPlayerCount = 1;
                if (Configuration.LynxTribe.LynxStuff.LynxShrineMultiplayerScaling.Value)
                {
                    participatingPlayerCount = Run.instance.participatingPlayerCount;
                }
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
                var pickupIndex = dropTable.GenerateDrop(rng);
                SetPickupIndex(pickupIndex.value);
                switch (pickupIndex.pickupDef.itemTier)
                {
                    case ItemTier.Tier1:
                        spawner.minSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTier1MinSpawns.Value;
                        spawner.maxSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTier1MaxSpawns.Value;
                        spawner.eliteBias = Configuration.LynxTribe.LynxStuff.LynxShrineTier1EliteBias.Value;
                        break;
                    case ItemTier.Tier2:
                        spawner.minSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTier2MinSpawns.Value;
                        spawner.maxSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTier2MaxSpawns.Value;
                        spawner.eliteBias = Configuration.LynxTribe.LynxStuff.LynxShrineTier2EliteBias.Value;
                        break;
                    case ItemTier.Tier3:
                        spawner.minSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTier3MinSpawns.Value;
                        spawner.maxSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTier3MaxSpawns.Value;
                        spawner.eliteBias = Configuration.LynxTribe.LynxStuff.LynxShrineTier3EliteBias.Value;
                        break;
                    case ItemTier.Boss:
                        spawner.minSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTierBossMinSpawns.Value;
                        spawner.maxSpawnCount = Configuration.LynxTribe.LynxStuff.LynxShrineTierBossMaxSpawns.Value;
                        spawner.eliteBias = Configuration.LynxTribe.LynxStuff.LynxShrineTierBossEliteBias.Value;
                        break;
                }
                spawner.CreateSpawnInfo();
            }
            if (NetworkClient.active)
            {
                UpdatePickupDisplay();
            }
        }

        public void OnPickupChanged(int newPickupValue)
        {
            SetPickupIndex(newPickupValue);
            if (NetworkClient.active)
            {
                UpdatePickupDisplay();
            }
        }

        private void SetPickupIndex(int newPickupValue)
        {
            if(pickupValue != newPickupValue)
            {
                pickupValue = newPickupValue;
                pickupIndex = new PickupIndex(pickupValue);
            }
        }

        private void UpdatePickupDisplay()
        {
            if (pickupDisplay)
            {
                pickupDisplay.SetPickupIndex(pickupIndex);
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
            var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            this.interactor = activator.GetComponent<CharacterBody>();
            Chat.SendBroadcastChat(new LynxShrineChatMessage
            {
                subjectAsCharacterBody = this.interactor,
                baseToken = "ENEMIES_RETURNS_LYNX_SHRINE_INTERACT",
                pickupToken = pickupDef?.nameToken ?? PickupCatalog.invalidPickupToken,
                pickupColor = pickupDef?.baseColor ?? Color.black,
                pickupQuantity = (uint)escapeDuration
            });
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
                        var pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                        Chat.SendBroadcastChat(new LynxShrineChatMessage
                        {
                            subjectAsCharacterBody = this.interactor,
                            baseToken = "ENEMIES_RETURNS_LYNX_SHRINE_INTERACT_FAILURE",
                            pickupToken = pickupDef?.nameToken ?? PickupCatalog.invalidPickupToken,
                            pickupColor = pickupDef?.baseColor ?? Color.black,
                            pickupQuantity = 0
                        });
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
