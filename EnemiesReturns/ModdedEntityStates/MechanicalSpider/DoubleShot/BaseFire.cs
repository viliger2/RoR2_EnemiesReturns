using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{

    public abstract class BaseFire : BaseState
    {
        public abstract int baseNumberOfShots { get; }

        public abstract string soundString { get; }

        public abstract float damageCoefficient { get; }

        public abstract float baseDelay { get; }

        public abstract float baseDuration { get; }

        public static GameObject projectilePrefab;

        public static float minSpread => Configuration.MechanicalSpider.DoubleShotMinSpread.Value;

        public static float maxSpread => Configuration.MechanicalSpider.DoubleShotMaxSpread.Value;

        public static float projectileSpeed => Configuration.MechanicalSpider.DoubleShotProjectileSpeed.Value;

        public static float force = 0f;


        public static float projectilePitchBonus = -1f;

        public static float distanceToTarget = 65f; // distance check to current target so we can continue firing instead of closing the hatch, 5m more than AI state check

        private float delay;

        private float duration;

        private float totalDuration;

        private float delayStopwatch;

        private int numberOfShots;

        private int shotsFired;

        public override void OnEnter()
        {
            base.OnEnter();
            delay = baseDelay / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            numberOfShots = baseNumberOfShots;
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
                FireProjectile();
                delayStopwatch -= delay;
                shotsFired++;
            }

            if (fixedAge >= totalDuration && isAuthority && characterBody && characterBody.master && characterBody.master.aiComponents != null)
            {
                foreach (var ai in characterBody.master.aiComponents)
                {
                    if (ai.currentEnemy.characterBody)
                    {
                        if (Vector3.Distance(ai.currentEnemy.characterBody.transform.position, transform.position) <= distanceToTarget)
                        {
                            outer.SetNextState(GetNextFiringState());
                            return;
                        }
                    }
                }

                outer.SetNextState(GetNextCloseHatch());
            }
        }

        public abstract EntityState GetNextFiringState();

        public abstract EntityState GetNextCloseHatch();

        private void FireProjectile()
        {
            if (isAuthority)
            {
                Ray aimRay = GetAimRay();
                aimRay.direction = Util.ApplySpread(aimRay.direction, minSpread, maxSpread, 1f, 1f, 0f, projectilePitchBonus);
                if (ModCompats.AdvancedPredictionCompat.enabled)
                {
                    aimRay = ModCompats.AdvancedPredictionCompat.GetPredictAimRay(aimRay, characterBody, projectilePrefab);
                }
                ProjectileManager.instance.FireProjectile(
                    projectilePrefab,
                    aimRay.origin,
                    Util.QuaternionSafeLookRotation(aimRay.direction),
                    gameObject,
                    damageStat * damageCoefficient,
                    force,
                    Util.CheckRoll(critStat, characterBody.master),
                    DamageColorIndex.Default,
                    null,
                    projectileSpeed,
                    DamageSource.Primary);
            }
            Util.PlayAttackSpeedSound(soundString, gameObject, attackSpeedStat);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
