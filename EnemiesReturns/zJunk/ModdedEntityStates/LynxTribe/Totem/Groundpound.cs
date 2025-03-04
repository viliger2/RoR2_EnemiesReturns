using EntityStates;
using RoR2;
using System;
using UnityEngine;

namespace EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Totem
{
    public class Groundpound : BaseState
    {
        public static float baseDuration = 4.1f;

        public static float baseAttackDuration = 1.7f;

        public static float damageCoefficient => Configuration.LynxTribe.LynxTotem.GroundpoundDamage.Value;

        public static float procCoefficient => Configuration.LynxTribe.LynxTotem.GroundpoundProcCoefficient.Value;

        public static float force => Configuration.LynxTribe.LynxTotem.GroundpoundForce.Value;

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
            attack.attacker = gameObject;
            attack.inflictor = gameObject;
            attack.teamIndex = GetTeam();
            attack.damage = damageCoefficient * damageStat;
            attack.isCrit = RollCrit();
            attack.hitBoxGroup = Array.Find(hitboxes, (element) => element.groupName == hitboxGroupName);
            attack.forceVector = Vector3.up * force;
            attack.procCoefficient = procCoefficient;
            attack.damageType = DamageType.SlowOnHit;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > attackDuration && !hasFired)
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

            if (fixedAge > duration && isAuthority)
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
