using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class SearchForTarget : BaseState
    {
        public static float baseDuration => Configuration.General.ProvidenceP1UtilityPreDuration.Value;

        public static float predictionTime => Configuration.General.ProvidenceP1UtilityPredictionTimer.Value;

        private float duration;

        private RoR2.Projectile.Predictor predictor;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            SetupPredictor();
            PlayCrossfade("Gesture, Override", "SlashInit", "combo.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (predictor != null && !predictor.hasTargetTransform)
            {
                FindTarget();
            } else
            {
                predictor.Update();
            }

            Log.Info($"targetPosition {predictor.GetTargetTransform()?.position}");

            if (fixedAge > duration && isAuthority)
            {
                Vector3 targetPosition = transform.position;
                if (predictor.hasTargetTransform)
                {
                    predictor.GetPredictedTargetPosition(predictionTime, out targetPosition);
                }
                Log.Info($"predictedPosition {targetPosition}");
                var nextState = new Disappear();
                nextState.predictedPosition = targetPosition;
                outer.SetNextState(nextState);
            }
        }

        private void SetupPredictor()
        {
            predictor = new RoR2.Projectile.Predictor(characterBody.transform);
            FindTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        private void FindTarget()
        {
            var aimRay = GetAimRay();
            BullseyeSearch search = new BullseyeSearch()
            {
                searchOrigin = aimRay.origin,
                searchDirection = aimRay.direction,
                filterByLoS = false,
                teamMaskFilter = TeamMask.allButNeutral,
                sortMode = BullseyeSearch.SortMode.Angle
            };
            if (teamComponent)
            {
                search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            }
            search.RefreshCandidates();

            var hurtBox = search.GetResults().FirstOrDefault();

            if (hurtBox)
            {
                predictor.SetTargetTransform(hurtBox.transform);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
