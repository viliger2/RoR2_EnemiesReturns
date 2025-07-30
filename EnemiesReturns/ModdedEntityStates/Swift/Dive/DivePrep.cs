using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class DivePrep : BaseState
    {
        public static float baseDuration = 1.8f;

        public static float soundTimer = 1.5f;

        private float duration;

        private bool playedSound = false;

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
            base.characterDirection.moveVector = base.inputBank.aimDirection;
            if(fixedAge >= duration && !playedSound)
            {
                Util.PlaySound("ER_Swift_PrepAttack_Play", gameObject);
                playedSound = true;
            }
            if (fixedAge >= duration && isAuthority)
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
