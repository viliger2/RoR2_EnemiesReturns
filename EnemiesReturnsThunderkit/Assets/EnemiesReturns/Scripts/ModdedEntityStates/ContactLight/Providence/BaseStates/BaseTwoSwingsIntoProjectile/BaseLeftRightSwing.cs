using EnemiesReturns.Behaviors;
using EntityStates;
using RoR2;
using System;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile
{
    public abstract class BaseLeftRightSwing : BaseState
    {
        public abstract float baseFirstSwing { get; }

        public abstract float baseSecondSwing { get; }

        public abstract float damageCoefficient { get; }

        public abstract string layerName { get; }

        public abstract string firstSwingStateName { get; }

        public abstract string secondSwingStateName { get; }

        public abstract string playbackParam { get; }

        public abstract string hitboxName { get; }

        public abstract string firstAttackParam { get; }

        public abstract string secondAttackParam { get; }

        private float firstSwing;

        private float secondSwing;

        private DamageDealtReciever damageDealtReciever;

        private Animator animator;

        private OverlapAttack overlapAttack;

        private bool playedSecondAnimation;

        public override void OnEnter()
        {
            base.OnEnter();
            firstSwing = baseFirstSwing / attackSpeedStat;
            secondSwing = baseSecondSwing / attackSpeedStat;
            PlayCrossfade(layerName, firstSwingStateName, playbackParam, firstSwing, 0.1f);
            damageDealtReciever = GetComponent<DamageDealtReciever>();
            if (damageDealtReciever)
            {
                damageDealtReciever.ResetDamageDealt();
            }
            animator = GetModelAnimator();

            var modelTransform = GetModelTransform();
            var hitboxes = modelTransform.GetComponents<HitBoxGroup>();
            overlapAttack = SetupAttack(Array.Find(hitboxes, (element) => element.groupName == hitboxName));
            characterBody.SetAimTimer(firstSwing + secondSwing);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && animator && animator.GetFloat(firstAttackParam) > 0.9f)
            {
                FireFirstAttack();
            }
            if (isAuthority && animator && animator.GetFloat(secondAttackParam) > 0.9f)
            {
                FireSecondAttack();
            }
            if (!playedSecondAnimation && fixedAge > firstSwing)
            {
                StartSecondSwingAnimation();
            }
            if (isAuthority && fixedAge > firstSwing + secondSwing)
            {
                var esm = EntityStateMachine.FindByCustomName(gameObject, "Body");
                if (characterBody.isPlayerControlled && inputBank.skill1.down)
                {
                    esm.SetInterruptState(GetNextStateIfMissed(), InterruptPriority.Skill);
                }
                else if (!characterBody.isPlayerControlled && damageDealtReciever && !damageDealtReciever.DamageDealt)
                {
                    esm.SetInterruptState(GetNextStateIfMissed(), InterruptPriority.Skill);
                }
                outer.SetNextStateToMain();
            }
        }

        public virtual void StartSecondSwingAnimation()
        {
            PlayCrossfade(layerName, secondSwingStateName, playbackParam, secondSwing, 0.1f);
            playedSecondAnimation = true;
            if (isAuthority)
            {
                overlapAttack.ResetIgnoredHealthComponents();
            }
        }

        public virtual void FireSecondAttack()
        {
            overlapAttack.Fire();
        }

        public virtual void FireFirstAttack()
        {
            overlapAttack.Fire();
        }

        public abstract EntityState GetNextStateIfMissed();

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
            if (damageDealtReciever)
            {
                damageDealtReciever.ResetDamageDealt();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        private OverlapAttack SetupAttack(HitBoxGroup hitBoxGroup)
        {
            var attack = new OverlapAttack();
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.isCrit = RollCrit();
            attack.hitBoxGroup = hitBoxGroup;
            attack.procCoefficient = 1f;
            attack.damageType = DamageSource.Primary;

            return attack;
        }
    }
}
