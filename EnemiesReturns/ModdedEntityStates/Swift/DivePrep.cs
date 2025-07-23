using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    public class DivePrep : BaseState
    {
        public static float baseDuration = 1.8f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.characterDirection.moveVector = base.inputBank.aimDirection;
            PlayCrossfade("Gesture, Override", "DivePrep", "dive.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new Dive());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
