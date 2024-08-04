using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class DeathDance : BaseState
    {
        private static float duration = 20f;
        private static float healthFraction = 0.5f;

        private float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "DeathDance");
            //characterBody.OnTakeDamageServer 
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if((stopwatch >= duration) || ((healthComponent.health / healthComponent.fullHealth) <= healthFraction))
            {
                outer.SetNextStateToMain();
            }
        }
    }
}
