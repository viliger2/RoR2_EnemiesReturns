using EntityStates;
using RoR2;
using UnityEngine;

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

        private bool attackFired = false;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            PlayAnimation(layerName, animationStateName, playbackRateParams, baseDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!attackFired && modelAnimator.GetFloat(animatorAttackParam) > 0.9f)
            {
                if (isAuthority)
                {
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
