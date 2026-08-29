using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase1 : BaseState
    {
        public static string phaseControllerChildString = "Phase1";

        public static float bossSpawnDelay = 5f;

        public static float doorCloseDelay = 7f;

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
                        esm.SetNextState(new ModdedEntityStates.ContactLight.CargoHoldDoors.Closed());
                    }
                }
                hasClosedDoor = true;
            }

            if (NetworkServer.active && fixedAge > bossSpawnDelay + 10f && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                //outer.SetNextState(new Phase2());
                outer.SetNextState(new PostFight()); // TODO: REMEMBER TO FIX
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
