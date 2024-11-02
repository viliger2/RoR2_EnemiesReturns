using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.MechanicalSpider
{
    public class Dash : BaseState
    {
        public static float dashDistance = 20f; // TODO: config, taken from Imp

        public static float duration = 1f;

        private static readonly int dashForward = Animator.StringToHash("dashForward");

        private static readonly int dashRight = Animator.StringToHash("dashRight");

        private Animator modelAnimator;

        private Vector3 dashStart;

        private Vector3 dashDestination;

        private CharacterAnimatorWalkParamCalculator animatorWalkParamCalculator;

        private BodyAnimatorSmoothingParameters.SmoothingParameters smoothingParameters;

        public override void OnEnter()
        {
            base.OnEnter();

            if(characterMotor)
            {
                characterMotor.enabled = false;
            }

            modelAnimator = GetModelAnimator();

            if (modelAnimator)
            {
                GetBodyAnimatorSmoothingParameters(out smoothingParameters);
                var moveVector = Vector3.zero;
                if (inputBank)
                {
                    moveVector = inputBank.moveVector;
                }
                var animatorForward = transform.forward;
                if (characterDirection)
                {
                    animatorForward = characterDirection.animatorForward;
                }
                animatorWalkParamCalculator.Update(moveVector, animatorForward, in smoothingParameters, duration); // yeah I am a hack
                modelAnimator.SetFloat(dashForward, animatorWalkParamCalculator.animatorWalkSpeed.x);
                modelAnimator.SetFloat(dashRight, animatorWalkParamCalculator.animatorWalkSpeed.y);
                //modelAnimator.SetFloat(dashForward, animatorWalkParamCalculator.animatorWalkSpeed.x, smoothingParameters.forwardSpeedSmoothDamp, duration);
                //modelAnimator.SetFloat(dashRight, animatorWalkParamCalculator.animatorWalkSpeed.y, smoothingParameters.rightSpeedSmoothDamp, duration);
                PlayCrossfade("Gesture", "Dash", 0.2f);
            }

            //(Vector2)aimRay.origin;

            var destination = inputBank.moveVector * dashDistance;
            dashStart = transform.position;
            dashDestination = transform.position;

            var groundNodes = SceneInfo.instance.groundNodes;
            var nodeIndex = groundNodes.FindClosestNode(transform.position + destination, characterBody.hullClassification);
            groundNodes.GetNodePosition(nodeIndex, out dashDestination);
            dashDestination += transform.position - characterBody.footPosition;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Log.Info($"dashForward: {modelAnimator.GetFloat(dashForward)}, dashRight: {modelAnimator.GetFloat(dashRight)}");
            if(characterMotor)
            {
                characterMotor.velocity = Vector3.zero; // not sure why its needed
            }
            SetPosition(Vector3.Lerp(dashStart, dashDestination, fixedAge / duration));
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void SetPosition(Vector3 newPosition)
        {
            if (characterMotor)
            {
                characterMotor.Motor.SetPosition(newPosition); // without rotation for now
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (characterMotor)
            {
                characterMotor.enabled = true;
            }
            if(modelAnimator)
            {
                PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
