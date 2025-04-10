using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    [RegisterEntityState]
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
