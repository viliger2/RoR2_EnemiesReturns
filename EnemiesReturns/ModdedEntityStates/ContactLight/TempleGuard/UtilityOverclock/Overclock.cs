using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine;
using EnemiesReturns.Reflection;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard.UtilityOverclock
{
    [RegisterEntityState]
    public class Overclock : BaseState
    {
        public static float buffDuration => 15f;

        public static float baseDuration => 2f;

        public static float baseAnimationDuration => 1.1f;

        public static GameObject preShieldEffect;

        public static Material overclockMaterial;

        private float duration;

        private float animationDuration;

        private Animator animator;

        private bool addedBuff;

        private CharacterModel characterModel;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            animationDuration = baseAnimationDuration / attackSpeedStat;

            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                characterModel = modelTransform.gameObject.GetComponent<CharacterModel>();
            }

            animator = GetModelAnimator();
            Util.PlaySound("Play_lunar_golem_attack2_buildUp", base.gameObject);
            PlayCrossfade("Gesture, Additive", "PreShell", "shell.duration", animationDuration, 0.1f);
            EffectManager.SimpleMuzzleFlash(preShieldEffect, gameObject, "Body", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (animator && animator.GetFloat("shell.applyBuff") > 0.9f && !addedBuff)
            {
                Util.PlaySound("Play_lunar_golem_attack2_shieldActivate", gameObject);
                if (characterModel)
                {
                    TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(base.gameObject);
                    temporaryOverlayInstance.duration = buffDuration;
                    temporaryOverlayInstance.originalMaterial = overclockMaterial;
                    temporaryOverlayInstance.animateShaderAlpha = true;
                    temporaryOverlayInstance.alphaCurve = AnimationCurve.Constant(0f, 1f, 1f);
                    temporaryOverlayInstance.transmit = false;
                    temporaryOverlayInstance.destroyComponentOnEnd = true;
                    temporaryOverlayInstance.AddToCharacterModel(characterModel);
                }
                if (NetworkServer.active)
                {
                    characterBody.AddTimedBuff(Content.Buffs.TempleGuardOverclock, buffDuration);
                }
                if (isAuthority && skillLocator && skillLocator.primary)
                {
                    skillLocator.primary.ApplyAmmoPack();
                }
                addedBuff = true;
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Additive", "BufferEmpty", 0.1f);
        }

    }
}
