using EnemiesReturns.Reflection;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.RechargableInteractable
{
    [RegisterEntityState]
    public class Recharging : BaseRechargableInteractable
    {
        public static float baseDuration = 60f;

        private Transform disabledLight;

        public override void OnEnter()
        {
            base.OnEnter();
            if (animator)
            {
                PlayAnimationOnAnimator(animator, "Base", "Recharging", "playback.duration", baseDuration);
            }
            if (childLocator)
            {
                disabledLight = childLocator.FindChild("DisabledLight");
                if (disabledLight)
                {
                    disabledLight.gameObject.SetActive(true);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new Idle());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (disabledLight)
            {
                disabledLight.gameObject.SetActive(false);
            }
        }
    }
}
