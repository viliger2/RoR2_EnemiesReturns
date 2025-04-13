//using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SwordBeam
{
    //[RegisterEntityState]
    public class SwordBeamStart : BaseState
    {
        public static float baseDuration = 1.4f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SwordLaserBegin", "SwordBeam.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new SwordBeamLoop());
            }
        }
    }
}
