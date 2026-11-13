using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class FireProjectiles : BaseFireProjectiles
    {
        public static GameObject staticEffectPrefab;

        public static GameObject staticProjecilePrefab;

        public override float baseDuration => ProvidencePhase1.P1OrbsDuration.Value;

        public override float baseSpawnProjectilesTime => ProvidencePhase1.P1OrbsProjectileSpawnTime.Value;

        public override float damageCoefficient => ProvidencePhase1.P1OrbsDamage.Value;

        public override int projectileCount => ProvidencePhase1.P1OrbsProjectileCount.Value;

        public override float baseProjectileDelay => ProvidencePhase1.P1OrbsDelayBetween.Value;

        public override GameObject effectPrefab => staticEffectPrefab;

        public override GameObject projectilePrefab => staticProjecilePrefab;

        public override string layerName => "UpperBodyOnly";

        public override string animationStateName => "ProjectileSummon";

        public override string[] originChildNames => new string[]
        {
            "FirstProjectile",
            "SecondProjectile",
            "ThirdProjectile"
        };
    }
}
