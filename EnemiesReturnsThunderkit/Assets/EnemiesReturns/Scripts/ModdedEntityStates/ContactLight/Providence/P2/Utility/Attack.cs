using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Utility
{
    [RegisterEntityState]
    public class Attack : BaseState
    {
        //public static float baseDuration => Configuration.General.ProvidenceP1UtilityAttackDuraion.Value;
        public static float baseDuration => 2f;

        //public static float earlyExit => Configuration.General.ProvidenceP1UtilityEarlyExit.Value;
        public static float earlyExit => 2f;

        private bool attackFired = false;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            PlayAnimation("Gesture", "Thundercall", "SkyLeap.playbackRate", baseDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!attackFired && modelAnimator.GetFloat("SkyLeap.firstAttack") > 0.9f)
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
                        hitMask = BulletAttack.defaultHitMask,
                        stopperMask = LayerIndex.world.mask,
                        maxDistance = 1000f,
                        owner = gameObject,
                        minSpread = 0f,
                        maxSpread = 0f,
                        radius = 8f,
                        origin = transform.position,
                        isCrit = RollCrit()
                    };

                    bulletAttack.Fire();
                }

                attackFired = true;
            }
            if(attackFired && fixedAge > earlyExit)
            {
                if(isAuthority && inputBank && skillLocator && skillLocator.secondary.IsReady() && inputBank.skill2.justPressed)
                {
                    skillLocator.secondary.ExecuteIfReady();
                    return;
                }
            }

            if(fixedAge > baseDuration && isAuthority)
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
            if (!attackFired && fixedAge < earlyExit)
            {
                return InterruptPriority.PrioritySkill;
            }
            return InterruptPriority.Skill;
        }
    }
}
