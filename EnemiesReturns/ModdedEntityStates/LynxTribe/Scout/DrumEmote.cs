using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Scout
{
    public class DrumEmote : BasePlayerEmoteState
    {
        public override float duration => -1;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "DrumThing", 0.5f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }
    }
}
