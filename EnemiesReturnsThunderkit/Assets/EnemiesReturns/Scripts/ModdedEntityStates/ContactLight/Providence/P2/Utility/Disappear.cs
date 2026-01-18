using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class Disappear : BaseDisappear
    {
        public static GameObject staticPredictedPositionEffect;

        public override GameObject predictedPositionEffect => staticPredictedPositionEffect;

        public override float baseDuration => 1f;

        public override EntityState GetNextState()
        {
            return new Attack();
        }
    }
}
