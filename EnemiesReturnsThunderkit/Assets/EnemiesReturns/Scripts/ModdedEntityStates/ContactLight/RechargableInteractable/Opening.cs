using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.RechargableInteractable
{
    [RegisterEntityState]
    public class Opening : BaseRechargableInteractable
    {
        public static float baseDuration = 3f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (animator)
            {
                PlayAnimationOnAnimator(animator, "Base", "Opening");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new Recharging());
            }
        }
    }
}
