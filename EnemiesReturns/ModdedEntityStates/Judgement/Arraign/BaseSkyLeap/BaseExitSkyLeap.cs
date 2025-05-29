using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap
{
    public abstract class BaseExitSkyLeap : BaseState
    {
        public abstract GameObject firstAttackEffect { get; }

        public abstract GameObject secondAttackEffect { get; }

        public abstract float baseDuration { get; }

        public abstract string soundString { get; }

        public abstract float firstAttackDamage { get; }

        public abstract float secondAttackDamage { get; }

        public abstract float attackForce { get; }

        public abstract float blastAttackRadius { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public abstract string playbackParamName { get; }

        public abstract string firstAttackParamName { get; }

        public abstract string secondAttackParamName { get; }

        public static AnimationCurve acdOverlayAlpha;

        public Vector3 dropPosition;

        private float duration;

        private bool secondAttackFired;

        private bool attackFired;

        private float startAge;

        internal Animator modelAnimator;

        private Renderer swordRenderer;

        private MaterialPropertyBlock swordPropertyBlock;

        private float originalEmissionPower;

        private Transform removeSwordMuzzle;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(soundString, base.gameObject);
            modelAnimator = GetModelAnimator();
            PlayAnimation(layerName, animationStateName, playbackParamName, duration);

            removeSwordMuzzle = FindModelChild("SkyLeapRemoveSwordMuzzle");

            var childLocator = GetModelChildLocator();
            swordRenderer = childLocator.FindChildComponent<Renderer>("SwordModel");
            if (swordRenderer)
            {
                originalEmissionPower = swordRenderer.material.GetFloat("_EmPower");
                swordPropertyBlock = new MaterialPropertyBlock();
                swordPropertyBlock.SetFloat("_EmPower", originalEmissionPower);
                swordRenderer.SetPropertyBlock(swordPropertyBlock);
            }
        }

        public override void Update()
        {
            base.Update();
            if (attackFired)
            {
                if(startAge == 0)
                {
                    startAge = age;
                }
                swordPropertyBlock.SetFloat("_EmPower", acdOverlayAlpha.Evaluate(Mathf.Min(1f, age - startAge / startAge + 1f)));
                swordRenderer.SetPropertyBlock(swordPropertyBlock);
            }
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
                    blastAttack.baseDamage = firstAttackDamage * damageStat;
                    blastAttack.canRejectForce = false;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    blastAttack.baseForce = attackForce;
                    blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                    blastAttack.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility);
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                }
                Util.PlaySound("Play_moonBrother_spawn", base.gameObject);

                var effectData = new EffectData()
                {
                    origin = dropPosition,
                    scale = 7f
                };
                EffectManager.SpawnEffect(firstAttackEffect, effectData, true);
                attackFired = true;
            }

            if(!secondAttackFired && modelAnimator.GetFloat(secondAttackParamName) > 0.9f)
            {
                var position = dropPosition;
                if (removeSwordMuzzle) 
                {
                    position = removeSwordMuzzle.position;
                }
                if (isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = blastAttackRadius;
                    blastAttack.procCoefficient = 0f;
                    blastAttack.position = position;
                    blastAttack.attacker = characterBody.gameObject;
                    blastAttack.crit = RollCrit();
                    blastAttack.baseDamage = secondAttackDamage * damageStat;
                    blastAttack.canRejectForce = false;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    blastAttack.baseForce = attackForce;
                    blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
                    blastAttack.damageType = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility);
                    blastAttack.attackerFiltering = AttackerFiltering.Default;
                    blastAttack.Fire();
                }
                var secondEffectData = new EffectData()
                {
                    scale = 3f,
                    origin = position,
                    rotation = Quaternion.identity
                };
                EffectManager.SpawnEffect(secondAttackEffect, secondEffectData, false);
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
            if (swordRenderer && swordPropertyBlock != null)
            {
                swordPropertyBlock.SetFloat("_EmPower", originalEmissionPower);
                swordRenderer.SetPropertyBlock(swordPropertyBlock);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Vehicle;
        }

    }
}
