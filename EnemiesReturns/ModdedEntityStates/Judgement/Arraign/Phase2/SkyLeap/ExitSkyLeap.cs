using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap
{
    [RegisterEntityState]
    public class ExitSkyLeap : BaseExitSkyLeap
    {
        public static GameObject firstAttackEffectStatic;

        public static GameObject secondAttackEffectStatic;

        public static GameObject waveProjectileStatic;

        public override GameObject firstAttackEffect => firstAttackEffectStatic;

        public override GameObject secondAttackEffect => secondAttackEffectStatic;

        public override float baseDuration => 2.3f;

        public override string soundString => "";

        public override float firstAttackDamage => 3.2f;

        public override float secondAttackDamage => 3.2f;

        public override float attackForce => 1000f;

        public override float blastAttackRadius => 30f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackParamName => "SkyLeap.playbackRate";

        public override string firstAttackParamName => "SkyLeap.firstAttack";

        public override string secondAttackParamName => "SkyLeap.secondAttack";

        public override GameObject waveProjectile => waveProjectileStatic;

        public override float waveProjectileDamage => 6f;

        public override int waveCount => 12;

        public override float waveProjectileForce => 0f;
    }
}
