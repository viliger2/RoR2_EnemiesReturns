using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    public class ExitSkyLeap : BaseState
    {
        public static GameObject firstAttackEffect;

        public static GameObject secondAttackEffect;

        public static float baseDuration = 2f;

        public static string soundString;

        public static float attackDamage = 3f;

        public static float attackForce = 1000f;

        public static float blastAttackRadius = 30f;

        public Vector3 dropPosition;

        private float duration;

        private bool secondAttackFired;

        private bool attackFired;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Util.PlaySound(soundString, base.gameObject);
            modelAnimator = GetModelAnimator();
            PlayAnimation("Gesture, Override", "ExitSkyLeap", "SkyLeap.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!attackFired && modelAnimator.GetFloat("SkyLeap.firstAttack") > 0.9f)
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

            if(!secondAttackFired && modelAnimator.GetFloat("SkyLeap.secondAttack") > 0.9f)
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
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }

    }
}
