using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class FireProjectiles : BaseFireProjectiles
    {
        public static GameObject staticEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_LunarWisp.LunarWispBombChargeUp_prefab).WaitForCompletion();

        public static GameObject staticProjecilePrefab;

        public override float baseDuration => 2.2f;

        public override float baseSpawnProjectiles => 1f;

        public override float damageCoefficient => 2f;

        public override int projectileCount => 3;

        public override float baseProjectileDelay => 0.25f;

        public override GameObject effectPrefab => staticEffectPrefab;

        public override GameObject projectilePrefab => staticProjecilePrefab;

        public override string layerName => "Gesture";

        public override string animationStateName => "Thundercall";

        public override string[] originChildNames => new string[]
        {
            "FirstProjectile",
            "SecondProjectile",
            "ThirdProjectile"
        };
    }
}
