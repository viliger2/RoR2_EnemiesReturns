using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class FireProjectiles : BaseFireProjectiles
    {
        public static GameObject staticEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_LunarWisp.LunarWispBombChargeUp_prefab).WaitForCompletion();

        public static GameObject staticProjecilePrefab;

        public override float baseDuration => ProvidencePhase2.P2OrbsDuration.Value;

        public override float baseSpawnProjectilesTime => ProvidencePhase2.P2OrbsProjectileSpawnTime.Value;

        public override float damageCoefficient => ProvidencePhase2.P2OrbsDamage.Value;

        public override int projectileCount => ProvidencePhase2.P2OrbsProjectileCount.Value;

        public override float baseProjectileDelay => ProvidencePhase2.P2OrbsDelayBetween.Value;

        public override GameObject effectPrefab => staticEffectPrefab;

        public override GameObject projectilePrefab => staticProjecilePrefab;

        public override string layerName => "UpperBodyOnly";

        public override string animationStateName => "ProjectileSummon";

        public override string[] originChildNames => new string[]
        {
            "FirstProjectile",
            "SecondProjectile",
            "ThirdProjectile",
            "FourthProjectile",
            "FifthProjectile"
        };
    }
}
