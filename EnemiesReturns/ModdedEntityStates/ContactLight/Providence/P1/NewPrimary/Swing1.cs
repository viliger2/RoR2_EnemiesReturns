using EnemiesReturns.EditorHelpers;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.NewPrimary
{
    [RegisterEntityState]
    public class Swing1 : BaseProviSwing
    {
        public static AnimationCurveDef acdPrefab;

        public override AnimationCurveDef acdForwardVelocity => acdPrefab;

        public override float baseSwingDuration => 0.75f;

        public override float baseDamageCoef => Configuration.ContactLight.Providence.ProvidencePhase1.P1PrimaryDamage.Value;

        public override string baseHitBox => "SwordHitbox";

        public override float baseProcCoefficient => Configuration.ContactLight.Providence.ProvidencePhase1.P1PrimaryProcCoefficient.Value;

        public override float basePushAwayForce => Configuration.ContactLight.Providence.ProvidencePhase1.P1PrimaryPushAwayForce.Value;

        public override float baseHitPauseDuration => 0.1f;

        public override string baseSwingEffectMuzzle => "FirstSwingMuzzle";

        public override string hitboxActiveString => "Slash1.attack";

        public override string baseBeginSwingSoundString => "ER_Providence_Swing_Play";

        public override void AuthorityOnFinish()
        {
            outer.SetNextState(new Swing2());
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Swing1", "combo.playbackRate", duration, 0.1f);
        }
    }
}
