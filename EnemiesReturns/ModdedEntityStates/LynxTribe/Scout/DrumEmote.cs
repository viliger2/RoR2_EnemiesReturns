using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Scout
{
    [RegisterEntityState]
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
