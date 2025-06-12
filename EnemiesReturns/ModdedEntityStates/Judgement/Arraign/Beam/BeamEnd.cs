using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Beam
{
    [RegisterEntityState]
    public class BeamEnd : BaseState
    {
        public static float baseDuration = 1.84f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration;
            PlayCrossfade("Gesture, Override", "SwordLaserEnd", "SwordBeam.playbackRate", duration, 0.1f);
            Util.PlaySound("ER_Arraign_BeamEnd_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
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
