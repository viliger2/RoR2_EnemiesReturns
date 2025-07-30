using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class DiveEnd : BaseState
    {
        public static float baseDuration = 2.6f;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            characterMotor.moveDirection = Vector3.zero;
            characterDirection.moveVector = characterDirection.forward;
            PlayCrossfade("Gesture, Override", "DiveGround", "dive.playbackRate", duration, 0.1f);
            Util.PlaySound("ER_Swift_AttackGroundImpact_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                if (characterBody.isPlayerControlled)
                {
                    outer.SetNextStateToMain();
                }
                else
                {
                    outer.SetNextState(new FlyToNearestNode());
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }
    }
}
