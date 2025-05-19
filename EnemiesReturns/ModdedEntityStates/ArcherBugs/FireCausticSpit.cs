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
    internal class FireCausticSpit : BaseState
    {
        public static float baseDuration = 2f;
        public static string targetMuzzle = "AbdomenMuzzle";
        public static GameObject projectilePrefab;
        public static float projectileSpeed = 55f;
        public static float minimumDistance = 0f;
        public static float maximumDistance = 200f;
        public static float timeToTarget => projectileSpeed;
        public static float damageCoefficient = 2f;
        private float duration;
        public static float projectileForce => 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, 2f, false);
            Util.PlaySound("ER_Spiiter_Spit_Play", gameObject);

            if (isAuthority)
            {              
                Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                int shotCount = 3;
                float spread = 20f;   //Bandit is 2, experiment to find a good value for this. Higher = wider

                float angle = 0f;
                float num2 = 0f;
                num2 = spread;
                angle = num2 / (shotCount - 1);

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
        }


        public override void FixedUpdate()
        {

            if (base.isAuthority && fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
