using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    public class BaseJudgementIntaractable : BaseState
    {
        internal PickupPickerController pickupPickerController;

        public override void OnEnter()
        {
            base.OnEnter();
            pickupPickerController = GetComponent<PickupPickerController>();
        }
    }
}
