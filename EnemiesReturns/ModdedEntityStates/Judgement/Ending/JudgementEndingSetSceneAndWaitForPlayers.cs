﻿using EnemiesReturns.Reflection;
using EntityStates.GameOver;
using RoR2;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Ending
{
    [RegisterEntityState]
    public class JudgementEndingSetSceneAndWaitForPlayers : BaseGameOverControllerState
    {
        private SceneDef desiredSceneDef;

        public override void OnEnter()
        {
            base.OnEnter();
            FadeToBlackManager.ForceFullBlack();
            FadeToBlackManager.fadeCount++;
            desiredSceneDef = SceneCatalog.GetSceneDefFromSceneName("enemiesreturns_judgementoutro");
            if (NetworkServer.active)
            {
                Run.instance.AdvanceStage(desiredSceneDef);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (NetworkServer.active && NetworkUser.AllParticipatingNetworkUsersReady() && SceneCatalog.mostRecentSceneDef == desiredSceneDef)
            {
                outer.SetNextState(new JudgementEndingPlayCutscene());
            }
        }

        public override void OnExit()
        {
            FadeToBlackManager.fadeCount--;
            base.OnExit();
        }
    }
}
