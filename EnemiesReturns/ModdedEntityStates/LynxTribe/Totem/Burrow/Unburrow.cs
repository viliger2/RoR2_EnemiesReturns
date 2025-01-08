using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow
{
    public class Unburrow : BaseState
    {
        public static float duration = 1.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Body", "Unburrow");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
