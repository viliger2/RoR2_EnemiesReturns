using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    public class Inactive : BaseJudgementIntaractable
    {
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active && pickupPickerController)
            {
                pickupPickerController.SetAvailable(false);
            }
            var childLocator = gameObject.GetComponent<ChildLocator>();
            if (childLocator)
            {
                var beamEffect = childLocator.FindChild("BeamEffect");
                if (beamEffect)
                {
                    beamEffect.gameObject.SetActive(false);
                }
            }
        }
    }
}
