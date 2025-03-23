using EntityStates;
using RoR2.Projectile;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.ParticleSystem.PlaybackState;
//using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.ThreeHitCombo
{
    public class FireHomingProjectiles : GenericCharacterMain
    {
        public static float baseDuration = 2f;

        public static float baseInitialDelay = 0.68f;

        public static float baseFireDuration = 0.64f;

        public static int orbCount = 3;

        //public static GameObject projectilePrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarWisp/LunarWispTrackingBomb.prefab").WaitForCompletion();

        public static float damageCoefficient = 2f;

        private Transform origin;

        private float duration;

        private float initialDelay;

        private float fireDuration;

        private int orbsFired;

        private int leftOrbCount;

        private int forwardOrbCount;

        private int rightOrbCount;

        private float timer;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            initialDelay = baseInitialDelay / attackSpeedStat;
            fireDuration = baseFireDuration / attackSpeedStat;

            var remainder = orbCount % 3;
            if(remainder != 0)
            {
                leftOrbCount = orbCount / 3;
                forwardOrbCount = orbCount / 3 + remainder;
                rightOrbCount = orbCount / 3;
            } else
            {
                leftOrbCount = orbCount / 3;
                forwardOrbCount = orbCount / 3;
                rightOrbCount = orbCount / 3;
            }

            origin = FindModelChild("HandR");
            if (!origin)
            {
                origin = base.transform;
            }

            PlayCrossfade("Gesture", "OrbEnterRight", "orb.playbackRate", initialDelay, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            timer += GetDeltaTime();

            if(fixedAge < initialDelay)
            {
                return;
            }

            if(timer >= fireDuration)
            {
                if(orbsFired < rightOrbCount)
                {
                    FireOrb("OrbFireRight");
                } else if(orbsFired < rightOrbCount + forwardOrbCount)
                {
                    FireOrb("OrbFireForward");
                }
                else if(orbsFired < rightOrbCount + forwardOrbCount + leftOrbCount)
                {
                    FireOrb("OrbFireLeft");
                }
                orbsFired++;
                timer = 0f;
            }

            if(fixedAge > duration && isAuthority && orbsFired >= orbCount)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "OrbExitLeft", 0.1f);
        }

        private void FireOrb(string animation)
        {
            PlayCrossfade("Gesture", animation, "orb.playbackRate", fireDuration, 0.1f);
            if (isAuthority)
            {
                var info = new FireProjectileInfo
                {
                    crit = RollCrit(),
                    damage = damageStat * damageCoefficient,
                    force = 0f,
                    owner = base.gameObject,
                    position = origin.position,
                    rotation = Util.QuaternionSafeLookRotation(origin.forward),
                    //projectilePrefab = projectilePrefab,
                    damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.Primary),
                };
                ProjectileManager.instance.FireProjectile(info);
            }
        }

    }
}
