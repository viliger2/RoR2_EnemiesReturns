using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.BrokenTeleporter
{
    // TODO: I am not happy with current implementation, but I don't think there is much that can be done
    public class EnableInteractionOnPhase : MonoBehaviour
    {
        public BrokenTeleporterInteraction teleporterInteraction;

        private void Awake()
        {
            On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter += BossDeath_OnEnter;
        }

        private void BossDeath_OnEnter(On.EntityStates.Missions.BrotherEncounter.BossDeath.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.BossDeath self)
        {
            orig(self);
            if (NetworkServer.active && teleporterInteraction)
            {
                teleporterInteraction.available = true;
            }
        }

        private void OnEnable()
        {
            if (!teleporterInteraction)
            {
                teleporterInteraction = GetComponent<BrokenTeleporterInteraction>();
            }
        }

        public void OnDestroy()
        {
            On.EntityStates.Missions.BrotherEncounter.BossDeath.OnEnter -= BossDeath_OnEnter;
        }

    }
}
