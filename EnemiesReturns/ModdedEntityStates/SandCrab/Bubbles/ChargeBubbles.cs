using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.SandCrab.Bubbles
{
    [RegisterEntityState]
    public class ChargeBubbles : BaseState
    {
        public static float baseDuration = 1f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override, Mask", "ChargeBubbles", "ChargeBubbles.playbackRate", duration, 0.1f);
            Util.PlaySound("ER_SandCrab_FireBubblesPrep_Play", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration)
            {
                outer.SetNextState(new FireBubbles());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override, Mask", "BufferEmpty");
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
