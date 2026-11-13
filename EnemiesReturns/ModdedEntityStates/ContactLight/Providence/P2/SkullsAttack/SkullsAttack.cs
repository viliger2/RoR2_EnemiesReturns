using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.SkullsAttack
{
    [RegisterEntityState]
    public class SkullsAttack : BaseSkullsAttack
    {
        public override string layerName => "Gesture, Override";

        public override string animationState => "SummonSkullsAttackLoop";

        public override string playbackRateParamName => "Skulls.singlePlayback";

        public override float damageCoefficient => ProvidencePhase2.P2SkullsDamage.Value;

        public override float baseFireFrequency => ProvidencePhase2.P2SkullsFireFrequency.Value;

        public override int projectilesToSpawn => ProvidencePhase2.P2SkullsProjectileCount.Value;

        public override int additionalProjectilesPerPlayer => ProvidencePhase2.P2SkullsProjectileCountPerPlayer.Value;

        public override float projectileSpeed => ProvidencePhase2.P2SkullsProjectileSpeed.Value;

        public override float maxDistance => ProvidencePhase2.P2SkullsMaxDistance.Value;

        public override bool canBeRed => true;

        public override EntityState GetNextState()
        {
            return new SkullsOutro();
        }
    }
}
