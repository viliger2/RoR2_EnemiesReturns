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
        public static float baseDuration = 2.7f;

        public static float baseFireAttack = 1f;

        public static string soundString;

        public static float attackDamage = 3f;

        public static float attackForce = 1000f;

        public Vector3 dropPosition;

        private float duration;

        private float fireAttack;

        private bool attackFired;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            fireAttack = baseFireAttack / attackSpeedStat;
            Util.PlaySound(soundString, base.gameObject);
            PlayAnimation("Gesture, Override", "ExitSkyLeap", "SkyLeap.playbackRate", duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!attackFired && base.fixedAge > fireAttack)
            {
                if (isAuthority)
                {
                    BlastAttack blastAttack = new BlastAttack();
                    blastAttack.radius = 15f;
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
                attackFired = true;
            }

            if(isAuthority && base.fixedAge > duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Stun;
        }

    }
}
