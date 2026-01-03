using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Orbs
{
    //[RegisterEntityState]
    public class FireSingleOrb : BaseState
    {
        public static GameObject projectilePrefab;

        public static float damageCoefficient = 2f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                var projectileInfo = new FireProjectileInfo()
                {
                    crit = RollCrit(),
                    owner = base.gameObject,
                    position = GetAimRay().origin,
                    projectilePrefab = projectilePrefab,
                    rotation = Util.QuaternionSafeLookRotation(GetAimRay().direction),
                    damage = damageStat * damageCoefficient,
                    damageTypeOverride = DamageTypeCombo.Generic
                };

                ProjectileManager.instance.FireProjectile(projectileInfo);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > 0.5f & isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

    }
}
