using EnemiesReturns.Behaviors.Judgement.WaveInteractable;
using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class Phase1 : BaseState
    {
        public static int requiredKarmaLevel = 6;

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
#if DEBUG || NOWEAVER
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                outer.SetNextState(new Phase2());
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                outer.SetNextState(new PreEnding());
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                outer.SetNextState(new Phase15());
            }
#endif
            if (missionController && missionController.missionClear && isAuthority)
            {
                if (Utils.ArraignKarmaCheck(requiredKarmaLevel))
                {
                    outer.SetNextState(new PrePhase15());
                }
                else
                {
                    outer.SetNextState(new PrePhase2());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            phaseControllerObject.SetActive(false);
        }

    }
}
