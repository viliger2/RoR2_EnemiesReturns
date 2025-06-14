using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.LeapingDash
{
    [RegisterEntityState]
    public class LeapDashExit : BaseState
    {
        public static float baseDuration = 1.5f;

        public static float additionalGravity = -10f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SwordFlipEnd", "swordFlip.playbackRate", duration, 0.1f);
            Util.PlaySound("Play_moonBrother_spawn", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                characterMotor.velocity.y += additionalGravity * GetDeltaTime();
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

    }
}
