using EntityStates;
using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;
//using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.ThreeHitCombo
{
    public class FireHomingProjectiles : BaseState
    {
        public static float baseInitialDelay = 0.68f;

        public static float baseDuration = 1.5f;

        public static float baseFireRate = 0.25f;

        //public static GameObject projectilePrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab").WaitForCompletion();

        public static float damageCoefficient = 2f;

        private Transform origin;

        private Transform fireRotationHelper;

        private float fireRate;

        private float duration;

        private float initialDelay;

        private float timer;

        public override void OnEnter()
        {
            base.OnEnter();
            initialDelay = baseInitialDelay / attackSpeedStat;
            fireRate = baseFireRate / attackSpeedStat;
            duration = baseDuration + initialDelay;

            origin = FindModelChild("HandR");
            if (!origin)
            {
                origin = base.transform;
            }
            fireRotationHelper = FindModelChild("OrbFireRotationHelper");
            PlayCrossfade("HomingOrbs", "EnterFireOrb", "orb.playbackRate", initialDelay, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge < initialDelay)
            {
                return;
            }

            timer += GetDeltaTime();

            if (timer >= fireRate)
            {
                PlayCrossfade("FireHomingOrbs", "FireHomingOrb", 0.1f);
                if (isAuthority)
                {
                    // Quaternion lookRotation;
                    // if (fireRotationHelper)
                    // {
                    //     lookRotation = Util.QuaternionSafeLookRotation(fireRotationHelper.position - origin.position, Vector3.up);
                    // } else
                    // {
                    //     lookRotation = Util.QuaternionSafeLookRotation(origin.forward);
                    // }
                    // var info = new FireProjectileInfo
                    // {
                    //     crit = RollCrit(),
                    //     damage = damageStat * damageCoefficient,
                    //     force = 0f,
                    //     owner = base.gameObject,
                    //     position = origin.position,
                    //     rotation = lookRotation,
                    //     projectilePrefab = projectilePrefab,
                    //     damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary),
                    // };
                    // ProjectileManager.instance.FireProjectile(info);
                }
                timer -= fireRate;
            }

            if(fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayAnimation("FireHomingOrbs", "BufferEmpty");
            PlayCrossfade("HomingOrbs", "ExitHomingOrb", 0.1f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
