using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class Attack : BaseAttack
    {
        public override string layerName => "Gesture, Override";

        public override string animationStateName => "ExitSkyLeap";

        public override string playbackRateParams => "SkyLeap.playbackRate";

        public override string animatorAttackParam => "SkyLeap.firstAttack";

        public override float baseDuration => 1f;

        public override float earlyExit => 1f;
    }
}