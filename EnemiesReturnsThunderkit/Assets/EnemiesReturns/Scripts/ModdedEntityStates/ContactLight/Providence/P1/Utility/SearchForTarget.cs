using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class SearchForTarget : BaseSearchForTarget
    {
        public override float baseDuration => 1f;

        public override float predictionTime => 1f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SlashInit";

        public override string playbackParamName => "combo.playbackRate";

        public override EntityState GetNextState()
        {
            return new Disappear();
        }
    }
}
