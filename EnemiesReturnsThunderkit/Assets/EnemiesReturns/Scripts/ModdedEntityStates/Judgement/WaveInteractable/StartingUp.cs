using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.WaveInteractable
{
    public class StartingUp : BaseJudgementIntaractable
    {
        public static float duration = 4f;

        public static string soundEntryEvent = "Play_ui_obj_nullWard_activate";

        private Transform startingUpEffects;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active && pickupPickerController)
            {
                pickupPickerController.SetAvailable(false);
            }

            if (childLocator)
            {
                startingUpEffects = childLocator.FindChild("IdleToCharging");
                if (startingUpEffects)
                {
                    startingUpEffects.gameObject.SetActive(true);
                }
            }
            Util.PlaySound(soundEntryEvent, gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (startingUpEffects)
            {
                startingUpEffects.gameObject.SetActive(false);
            }
        }



    }


}
