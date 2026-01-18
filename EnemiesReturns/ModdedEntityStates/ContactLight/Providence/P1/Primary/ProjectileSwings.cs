using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseProjectilePrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary
{
    [RegisterEntityState]
    public class ProjectileSwings : BasePrimaryWeaponSwing
    {
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Merc.EvisProjectile_prefab).WaitForCompletion();

        public static float projectileTime => Configuration.General.ProvidenceP1PrimaryProjectileTime.Value;

        public override float swingDamageCoefficient => 2f;

        public override float swingProcCoefficient => 1f;

        public override float swingForce => 0f;

        public override GameObject hitEffect => null;

        public override string swingSoundEffect => "";

        private bool hasFired;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > projectileTime && !hasFired)
            {
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
                hasFired = true;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
