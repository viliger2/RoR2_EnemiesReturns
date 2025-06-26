using EnemiesReturns.Reflection;
using EntityStates.GameOver;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Ending
{
    [RegisterEntityState]
    public class JudgementEndingStart : BaseGameOverControllerState
    {
        public static float duration = 3f;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && base.fixedAge >= duration)
            {
                outer.SetNextState(new JudgementEndingSetSceneAndWaitForPlayers());
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            if (Configuration.General.SkipJudgementCutscene.Value)
            {
                outer.SetNextState(new EntityStates.GameOver.LingerShort());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
