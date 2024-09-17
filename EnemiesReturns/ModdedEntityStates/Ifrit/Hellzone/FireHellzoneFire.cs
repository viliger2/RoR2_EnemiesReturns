using EntityStates;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    public class FireHellzoneFire : BaseState
    {
        public static float baseDuration = 2f;

        public static float baseChargeTime = 0.5f;

        public static string targetMuzzle = "MuzzleMouth";

        public static GameObject projectilePrefab;

        public static float projectileSpeed = 50f;

        public static float damageCoefficient = 10f;

        public static float force = 0f;

        private float duration;

        private float chargeTime;

        private bool hasFired;

        private Transform muzzleMouth;

        private Transform fireballAimHelper;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            chargeTime = baseChargeTime / attackSpeedStat;
            muzzleMouth = FindModelChild("MuzzleMouth");
            fireballAimHelper = FindModelChild("FireballAimHelper");
            PlayCrossfade("Gesture,Override", "FireballFire", "FireFireball.playbackRate", duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge <= chargeTime)
            {
                return;
            }

            if(!hasFired && isAuthority)
            {
                FireProjectile();
                hasFired = true;
            }

            if(fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new FireHellzoneEnd());
            }

        }

        private void FireProjectile()
        {
            var rotation = Quaternion.LookRotation(fireballAimHelper.position - muzzleMouth.position, Vector3.up);
            ProjectileManager.instance.FireProjectile(projectilePrefab, muzzleMouth.position, rotation, gameObject, damageStat * damageCoefficient, force, RollCrit(), RoR2.DamageColorIndex.Default, null, projectileSpeed);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
