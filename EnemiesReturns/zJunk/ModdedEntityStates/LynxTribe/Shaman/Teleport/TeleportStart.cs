﻿using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport
{
    [RegisterEntityState]
    public class TeleportStart : BaseState
    {
        public static float baseDuration => 2f;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "CastTeleport", "CastTeleport.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new Teleport());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Stun;
        }
    }
}
