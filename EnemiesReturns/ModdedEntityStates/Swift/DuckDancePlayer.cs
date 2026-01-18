using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    internal class DuckDancePlayer : BasePlayerEmoteState
    {
        public override float duration => -1f;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "DuckDance");
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }
    }
}
