using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase2 : BaseState
    {
        public static string phaseControllerChildString = "Phase2";

        public static float bossSpawnDelay = 10f;

        private GameObject phaseControllerObject;

        private ScriptedCombatEncounter combatEncounter;

        private bool hasSpawned;

        public override void OnEnter()
        {
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
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!hasSpawned && fixedAge > bossSpawnDelay)
            {
                hasSpawned = true;
                BeginEncounter();
            }

            if (NetworkServer.active && fixedAge > bossSpawnDelay + 10f && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Phase3());
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
            if (phaseControllerObject)
            {
                phaseControllerObject.SetActive(false);
            }
        }

    }
}
