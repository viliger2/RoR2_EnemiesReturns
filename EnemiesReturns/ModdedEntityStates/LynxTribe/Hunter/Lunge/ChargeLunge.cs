using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Hunter.Lunge
{
    public class ChargeLunge : BaseState
    {
        public static float baseDuration = 1f;

        private float duration; 

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture", "LungePrepare", "Attack.playbackRate", duration, 0.1f);
            if (characterDirection)
            {
                characterDirection.moveVector = GetAimRay().direction;
            }
            if (characterBody)
            {
                characterBody.SetAimTimer(duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new FireLunge());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
