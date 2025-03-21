using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    public class Slash2 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash1;

        public override void OnEnter()
        {
            this.baseDuration = 4f;
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

            if (characterDirection)
            {
                characterDirection.forward = inputBank.aimDirection;
            }
        }

        public override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", "Slash2", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }
    }
}
