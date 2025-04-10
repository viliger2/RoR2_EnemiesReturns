using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot
{
    [RegisterEntityState]
    internal class Fire : BaseState
    {
        public static int numberOfShots => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotShots.Value;

        public static GameObject projectilePrefab;

        public static string soundString = "ER_Spider_Fire_Play";

        public static string soundStringMinion = "ER_Spider_Fire_Drone_Play";

        public static float damageCoefficient => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotDamage.Value;

        public static float force = 0f;

        public static float baseDuration = 1f;

        public static float baseDelay => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotDelayBetween.Value;

        public static float minSpread => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotMinSpread.Value;

        public static float maxSpread => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotMaxSpread.Value;

        public static float projectilePitchBonus = -1f;

        public static float distanceToTarget = 65f; // distance check to current target so we can continue firing instead of closing the hatch, 5m more than AI state check

        public static float projectileSpeed => EnemiesReturns.Configuration.MechanicalSpider.DoubleShotProjectileSpeed.Value;

        private float delay;

        private float duration;

        private float totalDuration;

        private float delayStopwatch;

        private int shotsFired;

        private bool isMinion = false;

        public override void OnEnter()
        {
            base.OnEnter();
            delay = baseDelay / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            totalDuration = duration + delay * (numberOfShots - 1);
            PlayAnimation("Gesture, Additive", "Fire", "Fire.playbackRate", duration);
            isMinion = characterBody.inventory.GetItemCount(RoR2Content.Items.MinionLeash) > 0;
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
                        if (Vector3.Distance(ai.currentEnemy.characterBody.transform.position, base.transform.position) <= distanceToTarget)
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
                if (ModCompats.AdvancedPredictionCompat.enabled)
                {
                    aimRay = ModCompats.AdvancedPredictionCompat.GetPredictAimRay(aimRay, characterBody, projectilePrefab);
                }
                ProjectileManager.instance.FireProjectile(
                    projectilePrefab,
                    aimRay.origin,
                    Util.QuaternionSafeLookRotation(aimRay.direction),
                    base.gameObject,
                    damageStat * damageCoefficient,
                    force,
                    Util.CheckRoll(critStat, base.characterBody.master),
                    DamageColorIndex.Default,
                    null,
                    projectileSpeed,
                    DamageSource.Primary);
            }
            Util.PlayAttackSpeedSound(isMinion ? soundStringMinion : soundString, gameObject, attackSpeedStat);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
