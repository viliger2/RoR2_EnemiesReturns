using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    [RegisterEntityState]
    public class AwaitingSelection : BaseJudgementIntaractable
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if(NetworkServer.active && pickupPickerController)
            {
                pickupPickerController.SetAvailable(true);
            }
        }
    }
}
