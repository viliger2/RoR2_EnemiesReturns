using EntityStates.GameOver;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Ending
{
    internal class JudgementEndingPlayCutscene : BaseGameOverControllerState
    {
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && (!OutroCutsceneController.instance || OutroCutsceneController.instance.cutsceneIsFinished))
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if ((bool)OutroCutsceneController.instance && (bool)OutroCutsceneController.instance.playableDirector)
            {
                OutroCutsceneController.instance.playableDirector.time = OutroCutsceneController.instance.playableDirector.duration;
            }
            base.OnExit();
        }
    }
}
