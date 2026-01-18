using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Mission
{
    [RegisterEntityState]
    public class Phase3 : BaseState
    {
        public static string phaseControllerChildString = "Phase3";

        private ScriptedCombatEncounter combatEncounter;

        private GameObject phaseControllerObject;

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

                    combatEncounter = phaseControllerObject.transform.Find("BossSpawn").gameObject.GetComponent<ScriptedCombatEncounter>();
                }
            }
            BeginEncounter();
            ClearCorpses();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active && fixedAge > 2 && combatEncounter && combatEncounter.combatSquad.memberCount == 0)
            {
                outer.SetNextState(new Idle());
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
