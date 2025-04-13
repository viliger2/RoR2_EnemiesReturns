//using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SwordBeam
{
    //[RegisterEntityState]
    public class SwordBeamLoop : BaseState
    {
        public static float baseDuration = 10f;

        public static float degreesPerSecond = 40f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "SwrodLaserLoop", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                base.characterMotor.transform.RotateAround(base.characterMotor.transform.position, Vector3.up, degreesPerSecond * GetDeltaTime());
                base.characterMotor.Motor.SetPositionAndRotation(base.characterMotor.transform.position, base.characterMotor.transform.rotation);
            }

            if(fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new SwordBeamEnd());
            }
        }

    }
}
