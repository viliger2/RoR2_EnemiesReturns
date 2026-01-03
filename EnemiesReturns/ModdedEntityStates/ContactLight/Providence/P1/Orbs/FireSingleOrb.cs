using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Orbs
{
    [RegisterEntityState]
    public class FireSingleOrb : BaseState
    {
        public static GameObject projectilePrefab;

        public static int orbCount = 4;

        public static float damageCoefficient = 2f;

        private Vector3 startingDirection;

        private Quaternion rotation;

        private bool fired;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Gesture", "Thundercall", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > 1f && isAuthority && !fired)
            {
                FireOrbsAuthority();
                fired = true;
            }

            if(fixedAge > 2f && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        private void FireOrbsAuthority()
        {
            if (!isAuthority)
            {
                return;
            }

            var aimDirection = transform.forward;
            Vector3 direction = Quaternion.AngleAxis(120f * 0.5f, aimDirection) * Vector3.up;
            Quaternion rotation = Quaternion.AngleAxis(120f, aimDirection);

            for (int i = 0; i < orbCount; i++)
            {
                var projectileInfo = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    owner = base.gameObject,
                    position = GetAimRay().origin + direction * 2f,
                    projectilePrefab = projectilePrefab,
                    rotation = Util.QuaternionSafeLookRotation(direction),
                    damage = damageStat * damageCoefficient,
                    damageTypeOverride = DamageTypeCombo.Generic,
                    comboNumber = 0
                };

                ProjectileManager.instance.FireProjectile(projectileInfo);

                direction = rotation * direction;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture", "BufferEmpty", 0.1f);
            skillLocator.secondary = skillLocator.allSkills.First(component => component.skillName == "DashAttack");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

    }
}
