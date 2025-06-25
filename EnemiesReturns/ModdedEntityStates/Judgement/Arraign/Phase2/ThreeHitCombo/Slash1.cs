using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo
{
    [RegisterEntityState]
    public class Slash1 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash1;

        public static GameObject swingEffect;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/OmniImpactVFXHuntress.prefab").WaitForCompletion();

        private Vector3 desiredDirection;

        public override void OnEnter()
        {
            this.baseDuration = 0.6f;
            base.damageCoefficient = Configuration.Judgement.ArraignP2.ThreeHitComboFirstSwingDamage.Value;
            base.hitBoxGroupName = "Spear";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = Configuration.Judgement.ArraignP2.ThreeHitComboFirstSwingProcCoefficient.Value;
            base.pushAwayForce = Configuration.Judgement.ArraignP2.ThreeHitComboFirstSwingForce.Value;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "LanceEffectMuzzle";
            base.mecanimHitboxActiveParameter = "Slash1.attack";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "ER_Arraign_ThreeHitComboSwingP2_Play";
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
            PlayCrossfade("Gesture, Override", "Slash1", "combo.playbackRate", duration, 0.05f);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.damageType.damageSource = DamageSource.Secondary;
        }

        public override void AuthorityOnFinish()
        {
            outer.SetNextState(new Slash2());
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
