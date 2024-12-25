using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.BlastAttack;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class PushBack : BaseState
    {
        public static float baseDuration = 1.5f;

        public static float baseAttackDelay = 1f;

        public static float radius => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackRadius.Value;

        public static float damage => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackDamage.Value;

        public static float force => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackForce.Value;

        public static float procCoef => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackProcCoefficient.Value;

        public static GameObject summonPrefab;

        public static GameObject explosionPrefab;

        private float duration;

        private float attackDelay;

        private BlastAttack blastAttack;

        private bool isAttackFired;

        private Transform attackOrigin;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            attackDelay = baseAttackDelay / attackSpeedStat;
            PrepareAttack();
            attackOrigin = FindModelChild("StaffLowerPoint");
            if (!attackOrigin)
            {
                attackOrigin = base.transform;
            }
            PlayCrossfade("Gesture, Override", "CastTeleportFull", "CastTeleport.playbackRate", duration, 0.1f);
            Util.PlayAttackSpeedSound(EnemiesReturns.Configuration.General.ShamanVoices.Value ? "ER_Shaman_SummonPushBack_Play" : "ER_Shaman_SummonPushBack_No_Voice_Play", base.gameObject, attackSpeedStat);
            if (summonPrefab)
            {
                var summonEffectOrigin = FindModelChild("Base");
                if(!summonEffectOrigin)
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
            if(fixedAge > attackDelay && !isAttackFired)
            {
                if (NetworkServer.active)
                {
                    blastAttack.position = attackOrigin.position;
                    var result = blastAttack.Fire();
                    if (force > 0f)
                    {
                        for (int i = 0; i < result.hitPoints.Length; i++)
                        {
                            Vector3 direction = (result.hitPoints[i].hitPosition - attackOrigin.position).normalized * force;
                            result.hitPoints[i].hurtBox.healthComponent.TakeDamageForce(direction, true, false);
                        }
                    }
                }
                if (explosionPrefab)
                {
                    EffectManager.SpawnEffect(explosionPrefab, new EffectData
                    {
                        origin = attackOrigin.position,
                        scale = 4f
                    }, false);
                }
                isAttackFired = true;
            }

            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
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
        }
    }
}
