﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class SpearThrow : BaseWeaponThrow
    {
        public static GameObject staticProjectilePrefab;

        public override GameObject projectilePrefab => staticProjectilePrefab;

        public override float baseDuration => 4f;

        public override float damageCoefficient => 7f;

        public override float force => 0f;

        public override string layerName => "Gesture, Override";

        public override string animName => "ThrowSword";

        public override string playbackRateParamName => "WeaponThrow.playbackRate";

        public override string childOrigin => "HandR";

        public override string throwSound => "";

        public override string chargeSound => "ER_Arraign_SpearThrow_Charge_Play";
    }
}
