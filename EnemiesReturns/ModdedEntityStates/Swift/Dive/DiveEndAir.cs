using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class DiveEndAir : BaseState
    {
        public static float baseDuration = 1.85f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.characterMotor.moveDirection = Vector3.zero;
            base.characterDirection.moveVector = base.characterDirection.forward;
            PlayCrossfade("Gesture, Override", "DiveHalt", "dive.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }
    }
}
