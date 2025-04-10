using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    [RegisterEntityState]
    public class NopeEmotePlayer : BasePlayerEmoteState
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
