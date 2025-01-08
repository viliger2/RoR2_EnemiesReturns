using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow
{
    public class Burrow : BaseState
    {
        public static float duration = 1.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Body", "Burrow");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new Burrowed());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
