using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.SkullsAttack
{
    [RegisterEntityState]
    public class SkullsAttack : BaseSkullsAttack
    {
        public static GameObject staticProjectilePrefab;

        public static GameObject staticEffectPrefab;

        public static GameObject staticEffectRedPrefab;

        public override GameObject projectilePrefab => staticProjectilePrefab;

        public override GameObject effectPrefab => staticEffectPrefab;

        public override float damageCoefficient => 2f;

        public override float baseFireFrequency => 0.15f;

        public override int projectilesToSpawn => 25;

        public override int additionalProjectilesPerPlayer => 15;

        public override float projectileSpeed => 50f;

        public override float maxDistance => 15f;

        public override GameObject effectPrefabRed => staticEffectRedPrefab;

        public override bool canBeRed => true;
    }
}
