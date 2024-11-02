using EntityStates;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{
    internal class Fire : BaseState
    {
        public static int numberOfShots => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotShots.Value;

        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab").WaitForCompletion(); // TODO

        public static string soundString = ""; // TODO

        public static float damageCoefficient => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotDamage.Value;

        public static float force = 0f;

        public static float baseDuration = 1f;

        public static float baseDelay => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotDelayBetween.Value;

        public static float minSpread = 0f;

        public static float maxSpread = 0f;

        public static float projectilePitchBonus = -1f;

        public static float distanceToTarget = 65f; // distance check to current target so we can continue firing instead of closing the hatch, 5m more than AI state check

        private float delay;

        private float duration;

        private float totalDuration;

        private float delayStopwatch;

        private int shotsFired;

        public override void OnEnter()
        {
            base.OnEnter();
            delay = baseDelay / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            totalDuration = duration + delay * (numberOfShots - 1);
            PlayAnimation("Gesture, Additive", "Fire", "Fire.playbackRate", duration);
            FireProjectile();
            shotsFired = 1;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            delayStopwatch += Time.fixedDeltaTime;
            if (delayStopwatch >= delay && numberOfShots > shotsFired)
            {
                PlayAnimation("Gesture, Additive", "Fire", "Fire.playbackRate", duration);
                if (isAuthority)
                {
                    FireProjectile();
                }
                delayStopwatch -= delay;
                shotsFired++;
            }

            if(fixedAge >= totalDuration && isAuthority)
            {
                foreach(var ai in characterBody.master.aiComponents)
                {
                    if(ai.currentEnemy.characterBody)
                    {
                        if(Vector3.Distance(ai.currentEnemy.characterBody.transform.position, base.transform.position) <= distanceToTarget)
                        {
                            outer.SetNextState(new ChargeFire());
                            return;
                        }
                    }
                }

                outer.SetNextState(new CloseHatch());
            }
        }

        private void FireProjectile()
        {
            if (isAuthority)
            {
                Ray aimRay = GetAimRay();
                aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damageStat * damageCoefficient, force, Util.CheckRoll(critStat, base.characterBody.master));
            }
            Util.PlayAttackSpeedSound(soundString, gameObject, attackSpeedStat);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
