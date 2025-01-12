using EnemiesReturns.Behaviors;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    public class Groundpound : BaseState
    {
        public static float baseDuration = 4.1f;

        public static float baseAttackDuration = 1.7f;

        public static float damageCoefficient = 3f;

        public static float procCoefficient = 1f;

        public static float force = 1500f;

        public static GameObject shakeEffect;

        public static GameObject poundEffect;

        public static string hitboxGroupName = "Groundpound";

        private float duration;

        private float attackDuration;

        private OverlapAttack attack;

        private bool hasFired;

        private Transform shakeEffectTransform;

        public override void OnEnter()
        {
            base.OnEnter();

            duration = baseDuration / attackSpeedStat;
            attackDuration = baseAttackDuration / attackSpeedStat;

            PlayAnimation("Gesture, Override", "Groundpound", "groundpound.playbackDuration", duration);

            shakeEffectTransform = FindModelChild("ShakeEffect");
            if (shakeEffectTransform && shakeEffect)
            {
                EffectManager.SimpleEffect(shakeEffect, shakeEffectTransform.position, shakeEffectTransform.rotation, false);
            }

            var modelTransform = GetModelTransform();
            var hitboxes = modelTransform.GetComponents<HitBoxGroup>();

            attack = new OverlapAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.isCrit = RollCrit();
            attack.hitBoxGroup = Array.Find(hitboxes, (HitBoxGroup element) => element.groupName == hitboxGroupName);
            attack.forceVector = Vector3.up * force;
            //attack.hitEffectPrefab = hitEffectPrefab; // TODO
            attack.procCoefficient = procCoefficient;
            attack.damageType = DamageType.SlowOnHit;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge > attackDuration && !hasFired)
            {
                if (isAuthority)
                {
                    attack.Fire();
                }
                if (shakeEffectTransform && poundEffect)
                {
                    EffectManager.SimpleEffect(poundEffect, shakeEffectTransform.position, shakeEffectTransform.rotation, false);
                }
                hasFired = true;
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
