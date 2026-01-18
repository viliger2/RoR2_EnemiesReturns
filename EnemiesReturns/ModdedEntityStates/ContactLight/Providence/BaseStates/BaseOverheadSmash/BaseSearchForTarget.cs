using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash
{
    public abstract class BaseSearchForTarget : BaseState
    {
        public abstract float baseDuration { get; }

        public abstract float predictionTime { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public abstract string playbackParamName { get; }

        private float duration;

        private RoR2.Projectile.Predictor predictor;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            SetupPredictor();
            PlayCrossfade(layerName, animationStateName, playbackParamName, duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (predictor != null && !predictor.hasTargetTransform)
            {
                FindTarget();
            }
            else
            {
                predictor.Update();
            }

            if (fixedAge > duration && isAuthority)
            {
                Vector3 targetPosition = transform.position;
                if (predictor.hasTargetTransform)
                {
                    predictor.GetPredictedTargetPosition(predictionTime, out targetPosition);
                }
                var nextState = GetNextState();
                if (nextState is BaseDisappear)
                {
                    (nextState as BaseDisappear).predictedPosition = targetPosition;
                }
                outer.SetNextState(nextState);
            }
        }

        public abstract EntityState GetNextState();

        private void SetupPredictor()
        {
            predictor = new RoR2.Projectile.Predictor(characterBody.transform);
            FindTarget();
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
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
