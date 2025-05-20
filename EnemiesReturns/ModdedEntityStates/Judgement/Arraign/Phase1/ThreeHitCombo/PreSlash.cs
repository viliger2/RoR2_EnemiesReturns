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
    // TODO: just make sword glow instead?
    [RegisterEntityState]
    public class PreSlash : BaseState
    {
        public static float baseDuration = 1.0f;

        public static AnimationCurve acdOverlayAlpha;

        public static Material overlayMaterial;

        private float duration;

        private Material originalMaterial;

        private Material overlayCopyMaterial;

        private SkinnedMeshRenderer swordRenderer;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Override", "SlashInit", "combo.playbackRate", duration, 0.1f);
            var swordModelTransoform = FindModelChild("SwordModel");
            if (swordModelTransoform)
            {
                swordRenderer = swordModelTransoform.GetComponent<SkinnedMeshRenderer>();
                if (swordRenderer)
                {
                    originalMaterial = swordRenderer.material;
                    overlayCopyMaterial = UnityEngine.Object.Instantiate(overlayMaterial);
                }
            }
            Util.PlaySound("Play_scav_attack1_chargeup", gameObject); // TODO: REPLACE
        }

        public override void Update()
        {
            base.Update();
            if(swordRenderer && originalMaterial && overlayCopyMaterial)
            {
                overlayCopyMaterial.SetFloat("_AlphaBoost", acdOverlayAlpha.Evaluate(fixedAge / duration));
                swordRenderer.sharedMaterials = new Material[] { originalMaterial, overlayCopyMaterial };
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
            if(swordRenderer && originalMaterial)
            {
                swordRenderer.sharedMaterials = new Material[] { originalMaterial };
            }
            UnityEngine.Object.Destroy(overlayCopyMaterial);
        }
    }
}
