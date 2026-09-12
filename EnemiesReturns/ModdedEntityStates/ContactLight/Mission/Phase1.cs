using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
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

        public static float bossSpawnDelay = 5f;

        public static float doorCloseDelay = 6f;

        public static GameObject teleportEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.TeleportOutBoom_prefab).WaitForCompletion();

        public static MusicTrackDef musicTrack => Content.MusicTracks.Precipitation;

        private ScriptedCombatEncounter combatEncounter;

        private GameObject phaseControllerObject;

        private GameObject doorToClose;

        private bool hasSpawned;

        private bool hasClosedDoor;

        public override void OnEnter()
        {
            KillAllMonsters();
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
                }
            }
            ClearCorpses();
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
            KillAllMonsters();
            if (phaseControllerObject)
            {
                phaseControllerObject.SetActive(false);
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

        public void KillAllMonsters()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            foreach (TeamComponent item in new List<TeamComponent>(TeamComponent.GetTeamMembers(TeamIndex.Monster)))
            {
                if ((bool)item)
                {
                    HealthComponent component = item.GetComponent<HealthComponent>();
                    if ((bool)component)
                    {
                        component.Suicide();
                    }
                }
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
