using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class SearchForTarget : BaseState
    {
        //public static float baseDuration => Configuration.General.ProvidenceP1UtilityPreDuration.Value;
        public static float baseDuration => 2f;

        //public static float predictionTime => Configuration.General.ProvidenceP1UtilityPredictionTimer.Value;
        public static float predictionTime => 2f;

        private float duration;

        private RoR2.Projectile.Predictor predictor;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture", "Thundercall", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new FireClones());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            //PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
