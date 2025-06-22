using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    [RegisterEntityState]
    public class FireCausticSpit : BaseState
    {
        public static float baseDuration = 2f;

        public static string targetMuzzle = "AbdomenMuzzle";

        public static float damageCoefficient = 2f;

        public static GameObject projectilePrefab;

        public static float projectileForce => 3f;

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
            Util.PlaySound("ER_Spiiter_Spit_Play", gameObject);
            StartAimMode(GetAimRay(), 2f, false);
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

            int shotCount = 3;
            float spread = 20f;   //Bandit is 2, experiment to find a good value for this. Higher = wider

            var num2 = spread;
            var angle = num2 / (shotCount - 1);

            Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
            Quaternion rotation = Quaternion.AngleAxis(angle, axis);
            Ray aimRay2 = new Ray(aimRay.origin, direction);
            for (int i = 0; i < shotCount; i++)
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
