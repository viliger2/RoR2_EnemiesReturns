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
    public class Slash2 : BasicMeleeAttack
    {
        public static AnimationCurve acdSlash2;

        public static GameObject swingEffect;

        public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion();

        private Vector3 desiredDirection;

        public override void OnEnter()
        {
            this.baseDuration = 0.48f;
            base.damageCoefficient = Configuration.Judgement.ArraignP1.ThreeHitComboSecondSwingDamage.Value;
            base.hitBoxGroupName = "Sword";
            base.hitEffectPrefab = hitEffect;
            base.procCoefficient = Configuration.Judgement.ArraignP1.ThreeHitComboSecondSwingProcCoefficient.Value;
            base.pushAwayForce = Configuration.Judgement.ArraignP1.ThreeHitComboSecondSwingForce.Value;
            base.forceVector = Vector3.zero;
            base.hitPauseDuration = 0.1f;
            base.swingEffectPrefab = swingEffect;
            base.swingEffectMuzzleString = "SwingCombo2EffectMuzzle";
            base.mecanimHitboxActiveParameter = "Slash2.attack";
            base.shorthopVelocityFromHit = 0f;
            base.beginSwingSoundString = "ER_Arraign_ThreeHitComboSwingP1_Play";
            //base.impactSound = "";
            base.forceForwardVelocity = true;
            base.forwardVelocityCurve = acdSlash2;
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
            PlayCrossfade("Gesture, Override", "Slash2", "combo.playbackRate", duration, 0.05f);
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
            outer.SetNextState(new Slash3());
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
