using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseOverheadSmash
{
    public abstract class BaseAttack : BaseState
    {
        public abstract float baseDuration { get; }

        public abstract float earlyExit { get; }

        public abstract string layerName { get; }

        public abstract string animationStateName { get; }

        public abstract string playbackRateParams { get; }

        public abstract string animatorAttackParam { get; }

        public abstract int waveCount { get; }

        public abstract float waveProjectileDamage { get; }

        public abstract byte comboCount { get; }

        public static GameObject waveProjectile;

        public static GameObject landingEffect;

        private bool attackFired = false;

        private Animator modelAnimator;

        private Transform muzzleFloor;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            PlayAnimation(layerName, animationStateName, playbackRateParams, baseDuration);
            muzzleFloor = FindModelChild("MuzzleFloor");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!attackFired && modelAnimator.GetFloat(animatorAttackParam) > 0.9f)
            {
                FireAttackAuthority();
                if (landingEffect && muzzleFloor)
                {
                    EffectManager.SimpleEffect(landingEffect, muzzleFloor.position, Quaternion.identity, false);
                }

                attackFired = true;
            }
            if (attackFired && fixedAge > earlyExit)
            {
                if (isAuthority && inputBank && skillLocator && skillLocator.secondary.IsReady() && inputBank.skill2.justPressed)
                {
                    skillLocator.secondary.ExecuteIfReady();
                    return;
                }
            }

            if (fixedAge > baseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }
        
        public virtual void FireAttackAuthority()
        {
            if (!isAuthority)
            {
                return;
            }

            var bulletAttack = new BulletAttack
            {
                aimVector = Vector3.up,
                bulletCount = 1,
                damage = damageStat * 3f,
                falloffModel = BulletAttack.FalloffModel.None,
                damageType = DamageSource.Utility,
                hitMask = LayerIndex.entityPrecise.mask,
                stopperMask = -1,
                maxDistance = 1000f,
                owner = gameObject,
                minSpread = 0f,
                maxSpread = 0f,
                radius = 8f,
                origin = characterBody.footPosition + Vector3.down * 5f,
                isCrit = RollCrit()
            };

            bulletAttack.Fire();

            FireRingAuthority();
        }

        private void FireRingAuthority()
        {
            float num = 360f / (float)waveCount;
            Vector3 vector = Vector3.ProjectOnPlane(base.inputBank.aimDirection, Vector3.up);
            Vector3 corePosition = base.characterBody.corePosition;
            bool crit = RollCrit();
            for (int i = 0; i < waveCount; i++)
            {
                Vector3 forward = Quaternion.AngleAxis(num * (float)i, Vector3.up) * vector;
                forward = Utils.PitchUp(forward, 45f);
                if (base.isAuthority)
                {
                    var info = new FireProjectileInfo
                    {
                        projectilePrefab = waveProjectile,
                        position = corePosition,
                        rotation = Util.QuaternionSafeLookRotation(forward),
                        owner = base.gameObject,
                        damage = base.characterBody.damage * waveProjectileDamage,
                        damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Utility),
                        crit = crit,
                        comboNumber = comboCount,
                        speedOverride = 20f
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade(layerName, "BufferEmpty", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (!attackFired && fixedAge < earlyExit)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Skill;
        }
    }
}
