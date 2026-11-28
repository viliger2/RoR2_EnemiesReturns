using EnemiesReturns.EditorHelpers;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.NewPrimary;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary
{
    public abstract class BaseProviSwing : BasicMeleeAttack
    {
        public static GameObject hitEffectPrefabStatic;

        public static GameObject swingEffectStaticPrefab;

        public abstract AnimationCurveDef acdForwardVelocity { get; }

        public abstract float baseSwingDuration { get; }

        public abstract float baseDamageCoef { get; }

        public abstract string baseHitBox { get; }

        public abstract float baseProcCoefficient { get; }

        public abstract float basePushAwayForce { get; }

        public abstract float baseHitPauseDuration { get; }

        public abstract string baseSwingEffectMuzzle { get; }

        public abstract string hitboxActiveString { get; }

        public abstract string baseBeginSwingSoundString { get; }

        public override void OnEnter()
        {
            this.baseDuration = baseSwingDuration;
            this.damageCoefficient = baseDamageCoef;
            this.hitBoxGroupName = baseHitBox;
            this.hitEffectPrefab = hitEffectPrefabStatic;
            this.procCoefficient = baseProcCoefficient;
            this.pushAwayForce = basePushAwayForce;
            this.hitPauseDuration = baseHitPauseDuration;
            this.swingEffectInstance = swingEffectStaticPrefab;
            this.swingEffectMuzzleString = baseSwingEffectMuzzle;
            this.mecanimHitboxActiveParameter = hitboxActiveString;
            this.beginSwingSoundString = baseBeginSwingSoundString;
            this.forceForwardVelocity = true;
            this.forwardVelocityCurve = acdForwardVelocity.curve;
            this.damageType = RoR2.DamageTypeCombo.GenericPrimary;
            base.OnEnter();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
