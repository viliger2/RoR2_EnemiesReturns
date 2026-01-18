using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Secondary
{
    [RegisterEntityState]
    public class DashPrep : BaseState
    {
        public static float baseDuration => 1f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Gesture, Override", "SlashInit", "combo.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new DashAttack());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
