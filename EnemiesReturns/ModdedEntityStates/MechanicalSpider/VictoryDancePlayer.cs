using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    public class VictoryDancePlayer : BasePlayerEmoteState
    {
        public override float duration => 20f;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "DanceStart");
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }
    }
}
