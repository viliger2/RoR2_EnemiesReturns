using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    [RegisterEntityState]
    public class SexYesEmote : BasePlayerEmoteState
    {
        public override float duration => 12f;

        public override string soundEventPlayName => "ER_Totem_SexYes_Play";

        public override string soundEventStopName => "ER_Totem_SexYes_Stop";

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture, Override", "Bandicoot", 0.1f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.5f);
            base.OnExit();
        }
    }
}
