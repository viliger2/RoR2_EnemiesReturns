using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.CharacterSpeech;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Phase2 : BaseState
    {
        public static float spawnDelay = 3f;

        public static string phaseControllerChildString = "Phase2";

        public static float enableDirectorDelay = 9f;

        public static GameObject speechControllerPrefab;

        private ScriptedCombatEncounter combatEncounter;

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private bool hasSpawned;

        private CombatSquad combatSquad;

        public override void OnEnter()
        {
            KillAllMonsters();
            base.OnEnter();
            childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    phaseControllerObject.SetActive(true);
                    combatEncounter = phaseControllerObject.GetComponent<ScriptedCombatEncounter>();
                }
            }
            if (phaseControllerObject)
            {
                combatSquad = phaseControllerObject.GetComponent<CombatSquad>();
                if (combatSquad)
                {
                    combatSquad.onMemberAddedServer += CombatSquad_onMemberAddedServer;
                }
            }
        }

        private void CombatSquad_onMemberAddedServer(CharacterMaster master)
        {
            if (speechControllerPrefab)
            {
                UnityEngine.Object.Instantiate(speechControllerPrefab, master.transform).GetComponent<CharacterSpeechController>().characterMaster = master;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!hasSpawned)
            {
                if(fixedAge > spawnDelay)
                {
                    BeginEncounter();
                }
            }
            if(NetworkServer.active && fixedAge > spawnDelay + 2 && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Phase3());
            }
        }

        private void BeginEncounter()
        {
            hasSpawned = true;
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
            if (combatSquad)
            {
                combatSquad.onMemberAddedServer -= CombatSquad_onMemberAddedServer;
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
    }
}
