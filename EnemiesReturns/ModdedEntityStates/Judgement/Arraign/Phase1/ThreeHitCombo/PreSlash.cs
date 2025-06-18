using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    [RegisterEntityState]
    public class PreSlash : BaseState
    {
        public static float baseDuration = 1.0f;

        public static AnimationCurve acdOverlayAlpha;

        private float duration;

        private Renderer swordRenderer;

        private MaterialPropertyBlock swordPropertyBlock;

        private float originalEmissionPower;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SlashInit", "combo.playbackRate", duration, 0.1f);

            var childLocator = GetModelChildLocator();
            swordRenderer = childLocator.FindChildComponent<Renderer>("SwordModel");
            if (swordRenderer)
            {
                originalEmissionPower = swordRenderer.material.GetFloat("_EmPower");
                swordPropertyBlock = new MaterialPropertyBlock();
                swordPropertyBlock.SetFloat("_EmPower", originalEmissionPower);
                swordRenderer.SetPropertyBlock(swordPropertyBlock);
            }

            Util.PlaySound("ER_Arraign_ThreeHitComboCharge_Play", gameObject);
        }

        public override void Update()
        {
            base.Update();
            if(swordRenderer && swordPropertyBlock != null)
            {
                swordPropertyBlock.SetFloat("_EmPower", acdOverlayAlpha.Evaluate(age / duration));
                swordRenderer.SetPropertyBlock(swordPropertyBlock);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new Slash1());
            }
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();

            if (swordRenderer && swordPropertyBlock != null)
            {
                swordPropertyBlock.SetFloat("_EmPower", originalEmissionPower);
                swordRenderer.SetPropertyBlock(swordPropertyBlock);
            }
        }
    }
}
