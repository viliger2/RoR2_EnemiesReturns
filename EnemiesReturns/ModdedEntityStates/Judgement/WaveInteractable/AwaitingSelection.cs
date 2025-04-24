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
            if (childLocator)
            {
                var beamEffect = childLocator.FindChild("BeamEffect");
                if (beamEffect)
                {
                    beamEffect.gameObject.SetActive(true);
                }
                var lightning = childLocator.FindChild("ProngLightning");
                if (lightning)
                {
                    lightning.gameObject.SetActive(true);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (childLocator)
            {
                var waveFinishedEffect = childLocator.FindChild("WaveFinishedEffect");
                if (waveFinishedEffect)
                {
                    waveFinishedEffect.gameObject.SetActive(false);
                }
                var lightning = childLocator.FindChild("ProngLightning");
                if (lightning)
                {
                    lightning.gameObject.SetActive(false);
                }
            }
        }
    }
}
