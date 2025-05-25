using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    [RegisterEntityState]
    public class ExitSkyLeap : BaseExitSkyLeap
    {
        public static GameObject staticFirstAttackEffect;

        public static GameObject staticSecondAttackEffect;

        public override GameObject firstAttackEffect => staticFirstAttackEffect;

        public override GameObject secondAttackEffect => staticSecondAttackEffect;

        public override float baseDuration => 2.5f;

        public override string soundString => "";

        public override float firstAttackDamage => 8f;

        public override float secondAttackDamage => 6f;

        public override float attackForce => 1000f;

        public override float blastAttackRadius => 20f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackParamName => "SkyLeap.playbackRate";

        public override string firstAttackParamName => "SkyLeap.firstAttack";

        public override string secondAttackParamName => "SkyLeap.secondAttack";

    }
}
