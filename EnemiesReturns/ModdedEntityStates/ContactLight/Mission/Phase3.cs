using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase3 : BaseState
    {
        public static string phaseControllerChildString = "Phase3";

        public static float bossSpawnDelay = 5f;

        public static float templeGuardDirectorDelay = 15f;

        private ScriptedCombatEncounter combatEncounter;

        private GameObject phaseControllerObject;

        private GameObject templeGuardsCombatDirector;

        private bool hasSpawned;

        private bool hasEnabledTempleGuards;

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
                    var templeGuardsCombatDirectorTransform = phaseChildLocator.FindChild("TempleGuardCombatDirector");
                    if (templeGuardsCombatDirectorTransform)
                    {
                        templeGuardsCombatDirector = templeGuardsCombatDirectorTransform.gameObject;

                        var combatDirector = templeGuardsCombatDirector.GetComponent<CombatDirector>();
                        var placementArray = templeGuardsCombatDirector.GetComponent<CustomPlacement_Array>();
                        if(combatDirector && placementArray)
                        {
                            combatDirector.customPlacementBase = placementArray;
                        }
                    }
                }
            }
            ClearCorpses();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!hasSpawned && fixedAge > bossSpawnDelay)
            {
                hasSpawned = true;
                BeginEncounter();
            }

            if(!hasEnabledTempleGuards && fixedAge > templeGuardDirectorDelay) 
            {
                templeGuardsCombatDirector.SetActive(true);
                hasEnabledTempleGuards = true;
            }

            if (NetworkServer.active && fixedAge > bossSpawnDelay + 10 && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Phase4());
            }
        }

        private void BeginEncounter()
        {
            if (NetworkServer.active)
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
