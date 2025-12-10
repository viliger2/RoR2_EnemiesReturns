using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class DivePrep : BaseState
    {
        public static float baseDuration = 1.3f;

        public static GameObject effectPrefab;

        private float duration;

        private RoR2.Projectile.Predictor predictor;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            base.characterDirection.moveVector = base.inputBank.aimDirection;
            PlayCrossfade("Gesture, Override", "DivePrep", 0.1f);
            Util.PlaySound("ER_Swift_PrepAttack_Play", gameObject);
            var modelChildLocator = GetModelChildLocator();
            if (modelChildLocator)
            {
                var beakTranform = modelChildLocator.FindChild("Beak");
                if (beakTranform)
                {
                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        rootObject = base.gameObject,
                        modelChildIndex = (short)modelChildLocator.FindChildIndex(beakTranform),
                        origin = beakTranform.position,
                        rotation = Quaternion.identity,
                    }, false);
                }
            }
            if (isAuthority)
            {
                BullseyeSearch search = new BullseyeSearch();
                search.teamMaskFilter = TeamMask.allButNeutral;
                if (teamComponent)
                {
                    search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
                }
                search.maxDistanceFilter = 60f;
                search.maxAngleFilter = 90f;
                var aimRay = GetAimRay();
                search.searchOrigin = aimRay.origin;
                search.searchDirection = aimRay.direction;
                search.filterByLoS = false;
                search.sortMode = BullseyeSearch.SortMode.Angle;
                search.RefreshCandidates();
                var hurtBox = search.GetResults().FirstOrDefault();
                if (hurtBox)
                {
                    predictor = new RoR2.Projectile.Predictor(base.transform);
                    predictor.SetTargetTransform(hurtBox.transform);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(predictor != null)
            {
                predictor.Update();
            }
            base.characterDirection.moveVector = base.inputBank.aimDirection;
            if (fixedAge >= duration && isAuthority)
            {
                Vector3 predictedPosition = Vector3.zero;
                if(predictor != null)
                {
                    var transform = predictor.GetTargetTransform();
                    if (transform)
                    {
                        var distance = Vector3.Distance(transform.position, base.transform.position);
                        if (distance > 0)
                        {
                            var timeToTarget = distance / (moveSpeedStat * EnemiesReturns.ModdedEntityStates.Swift.Dive.Dive.diveSpeedCoefficient);
                            predictor.GetPredictedTargetPosition(timeToTarget, out predictedPosition);
                        }
                    }
                }
                outer.SetNextState(new Dive() { diveTarget = predictedPosition});
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("Gesture, Override", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
