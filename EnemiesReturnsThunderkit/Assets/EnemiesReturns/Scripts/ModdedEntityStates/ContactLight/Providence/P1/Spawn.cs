using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1
{
    [RegisterEntityState]
    public class Spawn : BaseState
    {
        public static float baseDuration = 7.5f;

        private HurtBoxGroup hurtboxGroup;

        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            Transform modelTransform = GetModelTransform();
            if (modelTransform)
            {
                hurtboxGroup = modelTransform.GetComponent<HurtBoxGroup>();
            }

            PlayAnimation("Body", "Spawn1", "Spawn1.playbackRate", baseDuration);

            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter++;
            }
            animator = GetModelAnimator();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0f);
            }
            if (NetworkServer.active)
            {
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, baseDuration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1f);
                animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1f);
            }
            if (hurtboxGroup)
            {
                hurtboxGroup.hurtBoxesDeactivatorCounter--;
            }
            if (NetworkServer.active)
            {
                if (characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

    }
}
