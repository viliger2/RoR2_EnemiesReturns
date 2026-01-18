using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    [RegisterEntityState]
    public class FireCausticSpit : BaseState
    {
        public static float baseDuration = 2f;

        public static string targetMuzzle = "AbdomenMuzzle";

        public static float damageCoefficient => EnemiesReturns.Configuration.ArcherBug.CausticSpitDamage.Value;

        public static GameObject projectilePrefab;

        public static float projectileForce => Configuration.ArcherBug.CausticSpitForce.Value;

        public static int projectileCount => Configuration.ArcherBug.CausitcSpitProjectileCount.Value;

        public static float projectileSpread => Configuration.ArcherBug.CausticSpitProjectileSpread.Value;

        public static GameObject chargeEffect;

        public float baseDelay = 0.5f;

        public bool hasFired;

        private float duration;

        private float delay;

        public override void OnEnter()
        {
            base.OnEnter();
            delay = baseDelay / attackSpeedStat;
            duration = baseDuration / attackSpeedStat;
            PlayAnimation("Gesture", "FireCausticSpit", "FireCausticSpit.playbackRate", duration);
            Util.PlaySound("ER_ArcherBug_Shoot_Play", gameObject);
            StartAimMode(GetAimRay(), 2f, false);
            EffectManager.SimpleMuzzleFlash(chargeEffect, base.gameObject, "BugButt", false);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > delay && !hasFired)
            {
                if (isAuthority)
                {
                    FireAttackAuthority();
                }
                hasFired = true;
            }

            if (base.isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        public void FireAttackAuthority()
        {
            var aimRay = GetAimRay();
            Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
            Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

            var angle = projectileSpread / (projectileCount - 1);

            Vector3 direction = Quaternion.AngleAxis(-projectileSpread * 0.5f, axis) * aimRay.direction;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Ray aimRay2 = new Ray(aimRay.origin, direction);
            for (int i = 0; i < projectileCount; i++)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay2.origin, Util.QuaternionSafeLookRotation(aimRay2.direction), gameObject, damageStat * damageCoefficient, projectileForce, RollCrit(), DamageColorIndex.Default, null, 50f, DamageTypeCombo.GenericPrimary);

                //Adjust aimray for the next shot
                aimRay2.direction = rotation * aimRay2.direction;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!hasFired && isAuthority)
            {
                FireAttackAuthority();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
