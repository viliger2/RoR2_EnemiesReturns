using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Special
{
    [RegisterEntityState]
    public class RotatingLaserHold : BaseState
    {
        public static float baseDuration = 2f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "BeamLoop");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextState(new RotatingLaserOutro());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
