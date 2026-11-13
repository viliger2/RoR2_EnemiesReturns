using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack
{
    [RegisterEntityState]
    public class SkullsAttack : BaseSkullsAttack
    {
        public override string layerName => "Gesture, Override";

        public override string animationState => "SummonSkullsAttackLoop";

        public override string playbackRateParamName => "Skulls.singlePlayback";

        public override float damageCoefficient => ProvidencePhase1.P1SkullsDamage.Value;

        public override float baseFireFrequency => ProvidencePhase1.P1SkullsFireFrequency.Value;

        public override int projectilesToSpawn => ProvidencePhase1.P1SkullsProjectileCount.Value;

        public override int additionalProjectilesPerPlayer => ProvidencePhase1.P1SkullsProjectileCountPerPlayer.Value;

        public override float projectileSpeed => ProvidencePhase1.P1SkullsProjectileSpeed.Value;

        public override float maxDistance => ProvidencePhase1.P1SkullsMaxDistance.Value;

        public override bool canBeRed => ProvidencePhase1.P1SkullsCanBeRed.Value;

        public override EntityState GetNextState()
        {
            return new SkullsOutro();
        }
    }
}
