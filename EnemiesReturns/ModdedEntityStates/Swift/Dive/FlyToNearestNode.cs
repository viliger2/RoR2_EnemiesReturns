using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Swift.Dive
{
    [RegisterEntityState]
    public class FlyToNearestNode : BaseState
    {
        public static float speedMultiplier = 1.5f;

        public static float launchSpeed = 20f;

        private Vector3 targetPosition;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            var position = GetFootPosition();
            PlayAnimation("Body", "Jump");
            var animator = GetModelAnimator();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("FlyOverride"), 1);
            }

            if (isAuthority)
            {
                var flag = false;
                var airNodes = SceneInfo.instance.airNodes;
                if (airNodes)
                {
                    var nodeIndex = airNodes.FindClosestNode(transform.position, characterBody.hullClassification);
                    flag = nodeIndex != RoR2.Navigation.NodeGraph.NodeIndex.invalid && airNodes.GetNodePosition(nodeIndex, out targetPosition);
                }
                if (!flag)
                {
                    outer.SetNextStateToMain();
                    return;
                }
                if (characterMotor)
                {
                    characterMotor.velocity.y = launchSpeed;
                    characterMotor.Motor.ForceUnground();
                }

                var vector = targetPosition - position;
                var speed = moveSpeedStat * speedMultiplier;
                duration = vector.magnitude / speed;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                var position = GetFootPosition();
                if (characterMotor)
                {
                    characterMotor.moveDirection = (targetPosition - position).normalized * speedMultiplier;
                }
                if (characterDirection)
                {
                    characterDirection.moveVector = (targetPosition - position).normalized;
                }
                if(fixedAge >= duration || Vector3.Distance(position, targetPosition) <= 0.01f)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (characterMotor)
            {
                characterMotor.moveDirection = Vector3.zero;
            }
            if (characterDirection)
            {
                characterDirection.moveVector = characterDirection.forward;
            }
        }

        private Vector3 GetFootPosition()
        {
            if ((bool)base.characterBody)
            {
                return base.characterBody.footPosition;
            }
            return base.transform.position;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
