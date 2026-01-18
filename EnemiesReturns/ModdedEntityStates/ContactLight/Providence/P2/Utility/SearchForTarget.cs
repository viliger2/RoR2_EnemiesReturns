using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class SearchForTarget : BaseSearchForTarget
    {
        public override float baseDuration => Configuration.General.ProvidenceP1UtilityPreDuration.Value;

        public override float predictionTime => Configuration.General.ProvidenceP1UtilityPredictionTimer.Value;

        public override string layerName => "Gesture";

        public override string animationStateName => "Thundercall";

        public override string playbackParamName => "combo.playbackRate";

        public override EntityState GetNextState()
        {
            return new FireClones();
        }
    }
}
