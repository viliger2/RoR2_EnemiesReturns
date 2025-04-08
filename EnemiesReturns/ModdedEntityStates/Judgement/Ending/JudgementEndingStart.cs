using EntityStates.GameOver;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Ending
{
    public class JudgementEndingStart : BaseGameOverControllerState
    {
        public static float duration = 6f;

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
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
