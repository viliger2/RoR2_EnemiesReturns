using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    [RegisterEntityState]
    public class GroundpoundProjectile : BaseState
    {
        public static float baseDuration = 4.1f;

        public static float baseAttackDuration = 1.7f;

        public static float damageCoefficient => EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundDamage.Value;

        public static GameObject eyeEffect;

        public static GameObject groundpoundProjectilePrefab;

        public static GameObject stoneParticlesEffect;

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
            Util.PlayAttackSpeedSound("ER_Totem_Groundpound_Play", gameObject, attackSpeedStat);
            var childLocator = GetModelChildLocator();
            shakeEffectTransform = childLocator.FindChild("ShakeEffect");
            if (shakeEffectTransform && shakeEffect)
            {
                EffectManager.SimpleEffect(shakeEffect, shakeEffectTransform.position, shakeEffectTransform.rotation, false);
            }

            var stoneParticlesOrigin = childLocator.FindChild("ShakeStoneParticlesOrigin");
            if (stoneParticlesOrigin && stoneParticlesEffect)
            {
                EffectManager.SimpleEffect(stoneParticlesEffect, stoneParticlesOrigin.position, stoneParticlesOrigin.rotation, false);
            }

            if (eyeEffect)
            {
                EffectManager.SpawnEffect(eyeEffect, new EffectData()
                {
                    rootObject = base.gameObject,
                    modelChildIndex = (short)childLocator.FindChildIndex("StoneEyeL")
                }, false);
                EffectManager.SpawnEffect(eyeEffect, new EffectData()
                {
                    rootObject = base.gameObject,
                    modelChildIndex = (short)childLocator.FindChildIndex("StoneEyeR")
                }, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > attackDuration && !hasFired)
            {
                if (shakeEffectTransform && poundEffect)
                {
                    if (isAuthority)
                    {
                        var projectileInfo = new FireProjectileInfo
                        {
                            damage = damageCoefficient * damageStat,
                            crit = RollCrit(),
                            projectilePrefab = groundpoundProjectilePrefab,
                            position = shakeEffectTransform.position,
                            rotation = shakeEffectTransform.rotation,
                            owner = gameObject,
                            damageTypeOverride = DamageSource.Primary
                        };
                        ProjectileManager.instance.FireProjectile(projectileInfo);
                    };
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
