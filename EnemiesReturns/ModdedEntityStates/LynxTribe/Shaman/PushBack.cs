﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    [RegisterEntityState]
    public class PushBack : BaseState
    {
        public static float baseDuration = 1.5f;

        public static float radius => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackRadius.Value;

        public static float damage => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackDamage.Value;

        public static float force => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackForce.Value;

        public static float procCoef => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackProcCoefficient.Value;

        public static GameObject summonPrefab;

        public static GameObject explosionPrefab;

        private float duration;

        private BlastAttack blastAttack;

        private bool isAttackFired;

        private Transform attackOrigin;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            modelAnimator = GetModelAnimator();
            PrepareAttack();
            attackOrigin = FindModelChild("StaffLowerPoint");
            if (!attackOrigin)
            {
                attackOrigin = base.transform;
            }
            PlayCrossfade("Gesture, Override", "CastTeleportFull", "CastTeleport.playbackRate", duration, 0.1f);
            Util.PlayAttackSpeedSound("ER_Shaman_SummonPushBack_No_Voice_Play", base.gameObject, attackSpeedStat);
            if (summonPrefab)
            {
                var summonEffectOrigin = FindModelChild("Base");
                if (!summonEffectOrigin)
                {
                    summonEffectOrigin = base.transform;
                }

                EffectManager.SpawnEffect(summonPrefab, new EffectData
                {
                    origin = summonEffectOrigin.position + Vector3.up * 0.75f,
                    scale = 2.5f
                }, false);
            }

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (modelAnimator.GetFloat("CastTeleport.attack") > 0.9f && !isAttackFired)
            {
                FireAttack();
                isAttackFired = true;
            }

            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireAttack()
        {
            if (NetworkServer.active)
            {
                blastAttack.position = attackOrigin.position;
                var result = blastAttack.Fire();
                if (force > 0f)
                {
                    for (int i = 0; i < result.hitPoints.Length; i++)
                    {
                        Vector3 direction = (result.hitPoints[i].hitPosition - attackOrigin.position).normalized * force + Vector3.up * force / 4f; // 4 is magic number to get 500 from 2000 default value, basically we apply 1/4th of force as Up force
                        result.hitPoints[i].hurtBox.healthComponent.TakeDamageForce(direction, true, false);
                    }
                }
            }
            if (explosionPrefab)
            {
                EffectManager.SpawnEffect(explosionPrefab, new EffectData
                {
                    origin = attackOrigin.position,
                    scale = radius
                }, false);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            if (!isAttackFired)
            {
                FireAttack();
                isAttackFired = true;
            }
        }

        private void PrepareAttack()
        {
            blastAttack = new BlastAttack();
            blastAttack.radius = radius;
            blastAttack.procCoefficient = procCoef;
            blastAttack.attacker = characterBody.gameObject;
            blastAttack.crit = RollCrit();
            blastAttack.baseDamage = damage * damageStat;
            blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            blastAttack.teamIndex = characterBody.teamComponent.teamIndex;
            blastAttack.attackerFiltering = AttackerFiltering.Default;
            blastAttack.damageType = DamageSource.Secondary;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
