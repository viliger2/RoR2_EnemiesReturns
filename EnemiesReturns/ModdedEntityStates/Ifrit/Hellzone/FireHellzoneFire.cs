using EntityStates;
using RoR2;
using RoR2.Projectile;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Hellzone
{
    public class FireHellzoneFire : BaseState
    {
        public static float baseDuration = 2f;

        public static float baseChargeTime = 0.5f;

        public static string targetMuzzle = "MuzzleMouth";

        public static GameObject projectilePrefab;

        public static float projectileSpeed = EnemiesReturnsConfiguration.Ifrit.HellzoneProjectileSpeed.Value;

        public static float damageCoefficient = EnemiesReturnsConfiguration.Ifrit.HellzoneDamage.Value;

        public static float force = EnemiesReturnsConfiguration.Ifrit.HellzoneForce.Value;

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
            if (fixedAge <= chargeTime)
            {
                return;
            }

            if (!hasFired && isAuthority)
            {
                FireProjectile();
                hasFired = true;
            }

            if (fixedAge >= duration && isAuthority)
            {
                outer.SetNextState(new FireHellzoneEnd());
            }

        }

        private void FireProjectile()
        {
            var rotation = Quaternion.LookRotation(fireballAimHelper.position - muzzleMouth.position, Vector3.up);

            var colliders = Physics.OverlapSphere(muzzleMouth.position + Vector3.forward * 30f, 30f, LayerIndex.entityPrecise.mask, QueryTriggerInteraction.Ignore);
            if (colliders.Length > 0)
            {
                // finding closest collider 
                float distance = 30f;
                int index = -1;
                for (int i = 0; i < colliders.Count(); i++)
                {
                    var hurtbox = colliders[i].GetComponent<HurtBox>();
                    if (hurtbox && hurtbox.healthComponent)
                    {
                        if (hurtbox.healthComponent.body == this.characterBody)
                        {
                            continue;
                        }
                        float currentDistance = Mathf.Abs(Vector3.Distance(colliders[i].transform.position, muzzleMouth.position));
                        if (currentDistance < distance)
                        {
                            distance = currentDistance;
                            index = i;
                        }
                    }
                }

                if (index > -1)
                {
                    var collider = colliders[index];
                    if (Physics.Raycast(collider.transform.position, Vector3.down, out var result, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                    {
                        rotation = Quaternion.LookRotation(result.point - muzzleMouth.position, Vector3.up);
                    }
                }
            }

            ProjectileManager.instance.FireProjectile(projectilePrefab, muzzleMouth.position, rotation, gameObject, damageStat * damageCoefficient, force, RollCrit(), RoR2.DamageColorIndex.Default, null, projectileSpeed);
        }

        public override void OnExit()
        {
            PlayCrossfade("Gesture,Override", "BufferEmpty", 0.1f);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
