using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    public class Phase2 : BaseState
    {
        public static float spawnDelay = 3f;

        public static string phaseControllerChildString = "Phase2";

        private ScriptedCombatEncounter combatEncounter;

        private BossGroup phaseBossGroup;

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private bool hasSpawned;

        public override void OnEnter()
        {
            base.OnEnter();
            childLocator = GetComponent<ChildLocator>();
            if (childLocator)
            {
                phaseControllerObject = childLocator.FindChild(phaseControllerChildString).gameObject;
                if (phaseControllerObject)
                {
                    phaseControllerObject.SetActive(true);
                    combatEncounter = phaseControllerObject.GetComponent<ScriptedCombatEncounter>();
                    phaseBossGroup = phaseControllerObject.GetComponent<BossGroup>();
                }
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
            if (phaseControllerObject)
            {
                phaseControllerObject.SetActive(false);
            }
        }
    }
}
