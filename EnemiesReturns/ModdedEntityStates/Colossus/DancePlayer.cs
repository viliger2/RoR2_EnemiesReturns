using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    [RegisterEntityState]
    public class DancePlayer : BasePlayerEmoteState
    {
        public override float duration => 0;

        public override string soundEventPlayName => "ER_Colossus_GOGOGO_Play";

        public override string soundEventStopName => "ER_Colossus_GOGOGO_Stop";

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Override", "DanceEnter", "dance.playbackrate", 1.9f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "DanceExit", 0.1f);
            base.OnExit();
        }
    }
}
