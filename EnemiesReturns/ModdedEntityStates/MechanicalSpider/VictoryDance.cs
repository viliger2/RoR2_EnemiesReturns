using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    [RegisterEntityState]
    public class VictoryDance : BaseMonsterEmoteState
    {
        public override float duration => 20f;

        public override string soundEventPlayName => "";

        public override string soundEventStopName => "";

        public override string layerName => "Gesture, Override";

        public override string animationName => "DanceStart";

        public override bool stopOnDamage => true;

        public override float healthFraction => 0.5f;
    }
}
