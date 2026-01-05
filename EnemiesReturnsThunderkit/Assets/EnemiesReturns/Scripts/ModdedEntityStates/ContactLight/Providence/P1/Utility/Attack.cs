using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Utility
{
    [RegisterEntityState]
    public class Attack : BaseState
    {
        public static float baseDuration = 3.5f;

        private bool attackFired = false;

        private Animator modelAnimator;

        public override void OnEnter()
        {
            base.OnEnter();
            modelAnimator = GetModelAnimator();
            PlayAnimation("Gesture, Override", "ExitSkyLeap", "SkyLeap.playbackRate", baseDuration);
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
            return InterruptPriority.PrioritySkill;
        }
    }
}
