using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.zJunk.ModdedEntityStates.Judgement.Arraign
{
    public abstract class BaseSlashDash : BaseState
    {
        public static AnimationCurve speedCoefficientCurve;

        public abstract float baseDuration { get; }

        public abstract float damageCoefficient { get; }

        public abstract float procCoefficient { get; }

        public abstract float turnSpeed { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public abstract string playbackParamName { get; }

        public abstract string hitBoxGroupName { get; }

        private static float turnSmoothTime = 0.01f;

        internal float duration;

        private OverlapAttack swordAttack;

        private Vector3 targetMoveVector;

        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            animator = GetModelAnimator();
            PlayAnimation(layerName, animationStateName, playbackParamName, duration);
            SetupSwordAttack(GetModelTransform());
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            characterBody.outOfCombatStopwatch = 0f;
            Vector3 targetMoveVelocity = Vector3.zero;
            targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(targetMoveVector, inputBank.aimDirection, ref targetMoveVelocity, turnSmoothTime, turnSpeed), Vector3.up).normalized;
            characterDirection.moveVector = targetMoveVector;
            Vector3 forward = characterDirection.forward;
            characterMotor.rootMotion += speedCoefficientCurve.Evaluate(fixedAge / duration) * moveSpeedStat * targetMoveVector * GetDeltaTime();

            if (isAuthority)
            {
                swordAttack.Fire();
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void SetupSwordAttack(Transform modelTransform)
        {
            swordAttack = new OverlapAttack();
            swordAttack.attacker = gameObject;
            swordAttack.inflictor = gameObject;
            swordAttack.teamIndex = TeamComponent.GetObjectTeam(gameObject);
            swordAttack.damage = damageCoefficient * damageStat;
            //swordAttack.hitEffectPrefab = ;
            swordAttack.isCrit = RollCrit();
            swordAttack.hitBoxGroup = Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (element) => element.groupName == hitBoxGroupName);
            swordAttack.procCoefficient = procCoefficient;
            swordAttack.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
        }
    }
}
