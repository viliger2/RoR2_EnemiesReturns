using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo
{
    public class Slash3 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash1;

        private Vector3 desiredDirection;

        public override void OnEnter()
        {
            this.baseDuration = 2.4f;
            base.damageCoefficient = 2f;
            base.hitBoxGroupName = "Sword";
            //base.hitEffectPrefab = 
            base.procCoefficient = 1f;
            base.pushAwayForce = 6000f;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            //base.swingEffectMuzzleString = "";
            //base.mecanimHitboxActiveParameter = "";
            base.shorthopVelocityFromHit = 0f;
            //base.impactSound = "";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = acdSlash1;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = false;
            base.ignoreAttackSpeed = false;

            base.OnEnter();

            desiredDirection = inputBank.aimDirection;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            Vector3 targetMoveVelocity = Vector3.zero;
            characterDirection.forward = Vector3.SmoothDamp(characterDirection.forward, desiredDirection, ref targetMoveVelocity, 0.01f, 45f);
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash3", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
