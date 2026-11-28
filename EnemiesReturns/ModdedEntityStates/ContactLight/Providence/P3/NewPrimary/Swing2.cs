using EnemiesReturns.EditorHelpers;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.NewPrimary
{
    [RegisterEntityState]
    public class Swing2 : BaseProviSwing
    {
        public static AnimationCurveDef acdPrefab;

        public override AnimationCurveDef acdForwardVelocity => acdPrefab;

        public override float baseSwingDuration => 1.5f;

        public override float baseDamageCoef => 2.5f;

        public override string baseHitBox => "SwordHitbox";

        public override float baseProcCoefficient => 1f;

        public override float basePushAwayForce => 6000f;

        public override float baseHitPauseDuration => 0.1f;

        public override string baseSwingEffectMuzzle => "SecondSwingMuzzle";

        public override string hitboxActiveString => "Slash1.attack";

        public override string baseBeginSwingSoundString => "ER_Providence_Swing_Play";

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Swing2", "combo.playbackRate", duration, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }
    }
}
