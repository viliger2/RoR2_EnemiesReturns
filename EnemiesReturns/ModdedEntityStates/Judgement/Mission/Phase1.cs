using EnemiesReturns.Behaviors.Judgement.WaveInteractable;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    public class Phase1 : BaseState
    {
        public static string phaseControllerChildString = "Phase1";

        private ChildLocator childLocator;

        private GameObject phaseControllerObject;

        private JudgementMissionController missionController;

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
                    missionController = phaseControllerObject.GetComponent<JudgementMissionController>();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(missionController.missionClear && isAuthority)
            {
                outer.SetNextState(new PrePhase2());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            phaseControllerObject.SetActive(false);
        }

    }
}
