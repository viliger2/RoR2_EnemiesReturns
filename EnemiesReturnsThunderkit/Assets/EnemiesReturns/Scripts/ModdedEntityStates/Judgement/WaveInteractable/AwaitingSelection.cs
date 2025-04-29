using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    public class AwaitingSelection : BaseJudgementIntaractable
    {
        private Transform idleEffects;

        public override void OnEnter()
        {
            base.OnEnter();
            if(NetworkServer.active && pickupPickerController)
            {
                pickupPickerController.SetAvailable(true);
            }
            if (childLocator)
            {
                idleEffects = childLocator.FindChild("Idle");
                if (idleEffects)
                {
                    idleEffects.gameObject.SetActive(true);
                }

            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (idleEffects)
            {
                idleEffects.gameObject.SetActive(false);
            }
            var waveFinishedEffect = childLocator.FindChild("WaveFinishedEffect");
            if (waveFinishedEffect)
            {
                waveFinishedEffect.gameObject.SetActive(false);
            }
        }
    }
}
