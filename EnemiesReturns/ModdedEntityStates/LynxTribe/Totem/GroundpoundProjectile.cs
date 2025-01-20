using EnemiesReturns.Behaviors;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    // TODO: some stones flying to shaking
    public class GroundpoundProjectile : BaseState
    {
        public static float baseDuration = 4.1f;

        public static float baseAttackDuration = 1.7f;

        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundDamage.Value;

        public static float procCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundProcCoefficient.Value;

        public static float force => EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundForce.Value;

        public static GameObject groundpoundProjectile;

        public static GameObject shakeEffect;

        public static GameObject poundEffect;

        public static string hitboxGroupName = "Groundpound";

        private float duration;

        private float attackDuration;

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
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(fixedAge > attackDuration && !hasFired)
            {
                if (shakeEffectTransform && poundEffect)
                {
                    if (isAuthority)
                    {
                        var projectileInfo = new FireProjectileInfo
                        {
                            damage = damageCoefficient * damageStat,
                            crit = RollCrit(),
                            projectilePrefab = groundpoundProjectile,
                            position = shakeEffectTransform.position,
                            rotation = shakeEffectTransform.rotation,
                            owner = gameObject
                        };
                        ProjectileManager.instance.FireProjectile(projectileInfo);
                    };
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
