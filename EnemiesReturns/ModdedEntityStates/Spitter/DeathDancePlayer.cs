using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class DeathDancePlayer : BaseEmoteState
    {
        public override float duration => 20f;

        public override string soundEventPlayName => "ER_Spitter_Laugh_Play";

        public override string soundEventStopName => "ER_Spitter_Laugh_Stop";

        public Transform target;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "DeathDance");
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }
    }
}
