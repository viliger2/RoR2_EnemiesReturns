using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class Disappear : BaseDisappear
    {
        public static GameObject staticPredictedPositionEffect;

        public override GameObject predictedPositionEffect => staticPredictedPositionEffect;

        public override float baseDuration => Configuration.General.ProvidenceP1UtilityInvisibleDuration.Value;

        public override EntityState GetNextState()
        {
            return new Attack();
        }
    }
}
