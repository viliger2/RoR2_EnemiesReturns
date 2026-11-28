using EnemiesReturns.EditorHelpers;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.NewPrimary
{
    [RegisterEntityState]
    public class Swing2 : BaseProviSwing
    {
        public static AnimationCurveDef acdPrefab;

        public override AnimationCurveDef acdForwardVelocity => acdPrefab;

        public override float baseSwingDuration => 1.5f;

        public override float baseDamageCoef => Configuration.ContactLight.Providence.ProvidencePhase2.P2PrimaryDamage.Value;

        public override string baseHitBox => "SwordHitbox";

        public override float baseProcCoefficient => Configuration.ContactLight.Providence.ProvidencePhase2.P2PrimaryProcCoefficient.Value;

        public override float basePushAwayForce => Configuration.ContactLight.Providence.ProvidencePhase2.P2PrimaryPushAwayForce.Value;

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
