using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    internal class FireCausticSpit : BaseState
    {
        public static float baseDuration = 2f;
        public static float baseChargeTime = 0.2f;
        public static string targetMuzzle = "AbdomenMuzzle";
        public static GameObject projectilePrefab;
        public static float projectileSpeed = 55f;
        public static float minimumDistance = 0f;
        public static float maximumDistance = 200f;
        public static float timeToTarget => projectileSpeed;
        public static float damageCoefficient = 2f;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            Ray aimRay = GetAimRay();
            StartAimMode(aimRay, 2f, false);
            Util.PlaySound("ER_Spiiter_Spit_Play", gameObject);
            
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
