using EnemiesReturns.Behaviors.Judgement.WaveInteractable;
using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
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
                    var judgementObject = phaseControllerObject.transform.Find("JudgementMissionController");
                    if (judgementObject)
                    {
                        missionController = judgementObject.GetComponent<JudgementMissionController>();
                    }
                    phaseControllerObject.SetActive(true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(missionController && missionController.missionClear && isAuthority)
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
