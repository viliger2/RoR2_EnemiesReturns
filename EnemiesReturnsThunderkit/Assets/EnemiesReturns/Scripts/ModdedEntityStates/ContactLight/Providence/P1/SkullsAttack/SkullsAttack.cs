using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack
{
    public class SkullsAttack : BaseSkullsAttack
    {
        public static GameObject staticProjectilePrefab;

        public static GameObject staticEffectPrefab;

        public override GameObject projectilePrefab => staticProjectilePrefab;

        public override GameObject effectPrefab => staticEffectPrefab;

        public override float damageCoefficient => 2f;

        public override float baseFireFrequency => 0.25f;

        public override int projectilesToSpawn => 10;

        public override int additionalProjectilesPerPlayer => 5;

        public override float projectileSpeed => 50f;

        public override float maxDistance => 15f;
    }
}
