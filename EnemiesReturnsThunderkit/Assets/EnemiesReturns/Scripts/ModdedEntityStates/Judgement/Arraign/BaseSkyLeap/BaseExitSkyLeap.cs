using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap
{
    public abstract class BaseExitSkyLeap : BaseState
    {
        public abstract GameObject firstAttackEffect { get; }

        public abstract GameObject secondAttackEffect { get; }

        public abstract float baseDuration { get; }

        public abstract string soundString { get; }

        public abstract float attackDamage { get; }

        public abstract float attackForce { get; }

        public abstract float blastAttackRadius { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public abstract string playbackParamName { get; }

        public abstract string firstAttackParamName { get; }

        public abstract string secondAttackParamName { get; }

        public Vector3 dropPosition;

        private float duration;

        private bool secondAttackFired;

        private bool attackFired;

        internal Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(soundString, base.gameObject);
            modelAnimator = GetModelAnimator();
            PlayAnimation(layerName, animationStateName, playbackParamName, duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!attackFired && modelAnimator.GetFloat(firstAttackParamName) > 0.9f)
            {
                if (isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = blastAttackRadius;
                    blastAttack.procCoefficient = 0f;
                    blastAttack.position = dropPosition;
                    blastAttack.attacker = characterBody.gameObject;
                    blastAttack.crit = RollCrit();
                    blastAttack.baseDamage = attackDamage * damageStat;
                    blastAttack.canRejectForce = false;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    blastAttack.baseForce = attackForce;
                    blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                    blastAttack.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility);
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                }
                EffectManager.SimpleEffect(firstAttackEffect, dropPosition, Quaternion.identity, false);
                attackFired = true;
            }

            if(!secondAttackFired && modelAnimator.GetFloat(secondAttackParamName) > 0.9f)
            {
                if (isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = blastAttackRadius;
                    blastAttack.procCoefficient = 0f;
                    blastAttack.position = dropPosition;
                    blastAttack.attacker = characterBody.gameObject;
                    blastAttack.crit = RollCrit();
                    blastAttack.baseDamage = attackDamage * damageStat;
                    blastAttack.canRejectForce = false;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    blastAttack.baseForce = attackForce;
                    blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                    blastAttack.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility);
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                }
                EffectManager.SimpleEffect(secondAttackEffect, dropPosition, Quaternion.identity, false);
                secondAttackFired = true;
            }

            if (isAuthority && base.fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }

    }
}
