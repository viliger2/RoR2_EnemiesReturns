using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.RechargableInteractable
{
    [RegisterEntityState]
    public class Recharging : BaseRechargableInteractable
    {
        public static float baseDuration = 60f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (animator)
            {
                PlayAnimationOnAnimator(animator, "Base", "Recharging", "playback.duration", baseDuration);
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
    }
}
