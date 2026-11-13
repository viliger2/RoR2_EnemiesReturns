using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Secondary
{
    [RegisterEntityState]
    public class SkullsAttack : BaseSkullsAttack
    {
        public override string layerName => "Gesture, Override";

        public override string animationState => "SummonSkullsAttackLoop";

        public override string playbackRateParamName => "Skulls.singlePlayback";

        public override float damageCoefficient => ProvidencePhase3.P3SkullsDamage.Value;

        public override float baseFireFrequency => ProvidencePhase3.P3SkullsFireFrequency.Value;

        public override int projectilesToSpawn => ProvidencePhase3.P3SkullsProjectileCount.Value;

        public override int additionalProjectilesPerPlayer => ProvidencePhase3.P3SkullsProjectileCountPerPlayer.Value;

        public override float projectileSpeed => ProvidencePhase3.P3SkullsProjectileSpeed.Value;

        public override float maxDistance => ProvidencePhase3.P3SkullsMaxDistance.Value;

        public override bool canBeRed => ProvidencePhase3.P3SkullsCanBeRed.Value;

        public override EntityState GetNextState()
        {
            return new SkullsOutro();
        }
    }
}
