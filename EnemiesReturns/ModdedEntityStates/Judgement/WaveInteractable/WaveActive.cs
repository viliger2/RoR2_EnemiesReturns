﻿using EnemiesReturns.Behaviors.Judgement.WaveInteractable;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    public class WaveActive : BaseJudgementIntaractable
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active && pickupPickerController)
            {
                pickupPickerController.SetAvailable(false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && JudgementMissionController.instance)
            {
                var instance = JudgementMissionController.instance;
                var endRound = true;
                for (int i = 0; i < instance.combatDirectors.Length; i++)
                {
                    var director = instance.combatDirectors[i];
                    endRound = endRound && director.combatSquad.defeatedServer;
                }
                if (endRound)
                {
                    if(instance.maxWaves <= instance.currentRound)
                    {
                        outer.SetNextState(new Inactive());
                    } else
                    {
                        outer.SetNextState(new AwaitingSelection());
                    }
                }
            }
        }

    }
}
