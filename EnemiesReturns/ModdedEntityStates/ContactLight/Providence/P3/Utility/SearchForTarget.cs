using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Utility
{
    [RegisterEntityState]
    public class SearchForTarget : BaseSearchForTarget
    {
        public override float baseDuration => 0.75f;

        public override float predictionTime => ProvidencePhase3.P3UtilityPredictionTimer.Value;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "Disappear";

        public override string playbackParamName => "combo.playbackRate";

        public override EntityState GetNextState()
        {
            return new FireClones();
        }
    }
}
