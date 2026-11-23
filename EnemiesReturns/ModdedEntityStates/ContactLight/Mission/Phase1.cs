using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.CharacterSpeech;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase1 : BaseState
    {
        public static string phaseControllerChildString = "Phase1";

        public static float screensDelay = 1.5f;

        public static float bossSpawnDelay = 5f;

        public static float doorCloseDelay = 6f;

        public static GameObject teleportEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Common_VFX.TeleportOutBoom_prefab).WaitForCompletion();

        public static MusicTrackDef musicTrack => Content.MusicTracks.Precipitation;

        public static Material matPanelBossScreenWarning;

        public static GameObject speechControllerPrefab;

        private GameObject speechControllerInstance;

        private ScriptedCombatEncounter combatEncounter;

        private GameObject phaseControllerObject;

        private GameObject doorToClose;

        private bool hasSpawned;

        private bool hasClosedDoor;

        private bool hasSwitchedMaterial;

        private Transform panelBossScreens;

        public override void OnEnter()
        {
            Utils.KillAllMonsters();
            base.OnEnter();
            var childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    phaseControllerObject.SetActive(true);
                    var phaseChildLocator = phaseControllerObject.GetComponent<ChildLocator>();

                    var combatEncounterTransform = phaseChildLocator.FindChild("CombatEncounter");
                    if (combatEncounterTransform)
                    {
                        combatEncounter = combatEncounterTransform.gameObject.GetComponent<ScriptedCombatEncounter>();
                        if (combatEncounter)
                        {
                            combatEncounter.combatSquad.onMemberAddedServer += CombatSquad_onMemberAddedServer;
                        }
                    }

                    var doorToCloseTransform = phaseChildLocator.FindChild("DoorToClose");
                    if (doorToCloseTransform)
                    {
                        doorToClose = doorToCloseTransform.gameObject;
                    }

                    var musicTrackOverride = phaseChildLocator.FindChild("MusicTrackOverride");
                    if (musicTrackOverride && musicTrackOverride.TryGetComponent<MusicTrackOverride>(out var musicComponent))
                    {
                        musicComponent.track = musicTrack;
                        musicTrackOverride.gameObject.SetActive(true);
                    }

                    var teleportPositions = phaseChildLocator.FindChild("TeleportPositions");
                    if (teleportPositions)
                    {
                        TeleportPlayersToPositions(teleportPositions);
                    }

                    var objectives = phaseChildLocator.FindChild("Objectives");
                    if (objectives)
                    {
                        objectives.gameObject.SetActive(false);
                    }

                    var cutsceneController = phaseChildLocator.FindChild("CutsceneController");
                    if(cutsceneController)
                    {
                        cutsceneController.gameObject.SetActive(true);
                    }


                    panelBossScreens = phaseChildLocator.FindChild("PanelBossScreens");
                }
            }
            ClearCorpses();
        }

        private void CombatSquad_onMemberAddedServer(CharacterMaster master)
        {
            if (speechControllerPrefab)
            {
                speechControllerInstance = Object.Instantiate(speechControllerPrefab);
                speechControllerInstance.GetComponent<CharacterSpeechController>().characterMaster = master;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(!hasSpawned && fixedAge > bossSpawnDelay)
            {
                hasSpawned = true;
                BeginEncounter();
            }

            if (!hasClosedDoor && doorToClose && fixedAge > doorCloseDelay)
            {
                if (NetworkServer.active)
                {
                    var esm = doorToClose.GetComponent<EntityStateMachine>();
                    if (esm)
                    {
                        esm.SetNextState(new ModdedEntityStates.ContactLight.CargoHoldDoors.Closing());
                    }
                }
                hasClosedDoor = true;
            }

            if(!hasSwitchedMaterial && panelBossScreens && fixedAge > screensDelay)
            {
                if (panelBossScreens.gameObject.TryGetComponent<Renderer>(out var renderer))
                {
                    renderer.material = matPanelBossScreenWarning;
                }
                hasSwitchedMaterial = true;
            }

#if DEBUG || NOWEAVER
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                outer.SetNextState(new Phase2());
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                outer.SetNextState(new Phase3());
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                outer.SetNextState(new Phase4());
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                outer.SetNextState(new PostFight());
            }
#endif

            if (NetworkServer.active && fixedAge > bossSpawnDelay + 10f && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Phase2());
            }
        }

        private void BeginEncounter()
        {
            if (NetworkServer.active && combatEncounter)
            {
                combatEncounter.BeginEncounter();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            Utils.KillAllMonsters();
            if (combatEncounter)
            {
                combatEncounter.combatSquad.onMemberAddedServer -= CombatSquad_onMemberAddedServer;
            }
            if (phaseControllerObject)
            {
                phaseControllerObject.SetActive(false);
            }
            if (speechControllerInstance)
            {
                UnityEngine.Object.Destroy(speechControllerInstance);
            }
        }

        private void TeleportPlayersToPositions(Transform teleportPositionsParent)
        {
            var positionsCount = teleportPositionsParent.childCount;

            for(int i = 0; i < PlayerCharacterMasterController.instances.Count; i++)
            {
                var instance = PlayerCharacterMasterController.instances[i];
                CharacterBody body = instance.master.GetBody();
                var position = teleportPositionsParent.GetChild(i % positionsCount).position;
                if (Util.HasEffectiveAuthority(body.gameObject))
                {
                    TeleportHelper.TeleportBody(body, position, true);
                }
                var data = new EffectData()
                {
                    origin = position,
                    rotation = Quaternion.identity,
                };
                EffectManager.SpawnEffect(teleportEffect, data, false);
            }
        }

        public void ClearCorpses()
        {
            for (int num3 = RoR2.Corpse.instancesList.Count - 1; num3 >= 0; num3--)
            {
                RoR2.Corpse.DestroyCorpse(RoR2.Corpse.instancesList[num3]);
            }
        }
    }
}
