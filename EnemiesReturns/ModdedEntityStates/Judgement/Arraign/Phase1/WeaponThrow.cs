using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1
{
    [RegisterEntityState]
    public class WeaponThrow : BaseWeaponThrow
    {
        public static GameObject staticProjectilePrefab;

        public override GameObject projectilePrefab => staticProjectilePrefab;

        public override float baseDuration => 4f;

        public override float damageCoefficient => 3f;

        public override float force => 0f;

        public override string layerName => "Gesture, Override";

        public override string animName => "ThrowSword";

        public override string playbackRateParamName => "WeaponThrow.playbackRate";

        public override string childOrigin => "HandR";

        public override string throwSound => "Play_moonBrother_swing_horizontal"; // TODO: replace?

    }
}
