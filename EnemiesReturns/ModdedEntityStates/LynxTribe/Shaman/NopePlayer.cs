using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class NopePlayer : BasePlayerEmoteState
    {
        public override float duration => -1f;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture", "Nope");
        }

        public override void OnExit()
        {
            //PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
            base.OnExit();
        }
    }
}
