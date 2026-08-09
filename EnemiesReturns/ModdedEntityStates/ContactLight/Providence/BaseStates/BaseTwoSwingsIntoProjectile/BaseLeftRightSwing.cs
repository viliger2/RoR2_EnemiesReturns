using EnemiesReturns.Behaviors;
using EntityStates;
using RoR2;
using System;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile
{
    // TODO: values for static objects
    public abstract class BaseLeftRightSwing : BasicMeleeAttack
    {
        public new static GameObject hitEffectPrefab;

        public new static GameObject swingEffectPrefab;

        public new static NetworkSoundEventDef impactSound;

        public new abstract float baseDuration { get; }

        public new abstract float damageCoefficient { get; }

        public new abstract string hitBoxGroupName { get; }

        public new abstract float procCoefficient { get; }

        public new abstract float pushAwayForce { get; }

        public new abstract Vector3 forceVector { get; }

        public new abstract float hitPauseDuration { get; }

        public new abstract string swingEffectMuzzleString { get; }

        public new abstract string mecanimHitboxActiveParameter { get; }

        public new abstract float shorthopVelocityFromHit { get; }

        public new abstract string beginStateSoundString { get; }

        public new abstract string beginSwingSoundString { get; }

        public new abstract bool forceForwardVelocity { get; }

        public new abstract bool scaleHitPauseDurationAndVelocityWithAttackSpeed { get; }

        public new abstract bool ignoreAttackSpeed { get; }

        public new abstract DamageTypeCombo damageType { get; }

        public abstract float earlyExit { get; }

        private bool targetsHit;

        public override void OnEnter()
        {
            base.baseDuration = baseDuration;
            base.damageCoefficient = damageCoefficient;
            base.hitBoxGroupName = hitBoxGroupName;
            base.hitEffectPrefab = hitEffectPrefab;
            base.procCoefficient = procCoefficient;
            base.pushAwayForce = pushAwayForce;
            base.forceVector = forceVector;
            base.hitPauseDuration = hitPauseDuration;
            base.swingEffectPrefab = swingEffectPrefab;
            base.swingEffectMuzzleString = swingEffectMuzzleString;
            base.mecanimHitboxActiveParameter = mecanimHitboxActiveParameter;
            base.shorthopVelocityFromHit = shorthopVelocityFromHit;
            base.beginStateSoundString = beginStateSoundString;
            base.beginSwingSoundString = beginSwingSoundString;
            base.impactSound = impactSound;
            base.forceForwardVelocity = forceForwardVelocity;
            base.scaleHitPauseDurationAndVelocityWithAttackSpeed = scaleHitPauseDurationAndVelocityWithAttackSpeed;
            base.ignoreAttackSpeed = ignoreAttackSpeed;
            base.damageType = damageType;

            base.OnEnter();

            targetsHit = false;
        }

        public override void OnMeleeHitAuthority()
        {
            base.OnMeleeHitAuthority();
            targetsHit = true;
        }

        public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            base.AuthorityModifyOverlapAttack(overlapAttack);
            overlapAttack.retriggerTimeout = 1f;
        }

        public override void AuthorityFixedUpdate()
        {
            base.AuthorityFixedUpdate();
            if(fixedAge >= earlyExit && !targetsHit)
            {
                outer.SetInterruptState(GetNextStateIfMissed(), InterruptPriority.Skill);
            }
        }

        public abstract EntityState GetNextStateIfMissed();

        //public override void AuthorityOnFinish()
        //{
        //    var esm = EntityStateMachine.FindByCustomName(gameObject, "Body");
        //    if (characterBody.isPlayerControlled && inputBank.skill1.down)
        //    {
        //        esm.SetInterruptState(GetNextStateIfMissed(), InterruptPriority.Skill);
        //    }
        //    else if (!characterBody.isPlayerControlled && targetsHit)
        //    {
        //        esm.SetInterruptState(GetNextStateIfMissed(), InterruptPriority.Skill);
        //    }
        //    outer.SetNextStateToMain();
        //}
    }
}
