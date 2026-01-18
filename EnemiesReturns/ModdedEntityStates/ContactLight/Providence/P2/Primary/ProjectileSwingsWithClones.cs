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

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Primary
{
    [RegisterEntityState]
    public class ProjectileSwingsWithClones : BasePrimaryWeaponSwing
    {
        public static GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Merc.EvisProjectile_prefab).WaitForCompletion();

        public static GameObject cloneEffect;

        public static float projectileTime => Configuration.General.ProvidenceP1PrimaryProjectileTime.Value;

        public static float cloneDelay = 0.2f;

        public static int minCloneCount = 1;
            
        public static int maxCloneCount = 3;

        public override float swingDamageCoefficient => 2f;

        public override float swingProcCoefficient => 1f;

        public override float swingForce => 0f;

        public override GameObject hitEffect => null;

        public override string swingSoundEffect => "";

        private float cloneTimer;

        private int clonesFired;

        private int cloneCount;

        private ChildLocator modelChildLocator;

        private Transform muzzleFloor;

        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            modelChildLocator = GetModelChildLocator();
            muzzleFloor = FindModelChild("MuzzleFloor");
            cloneCount = (int)Mathf.Min(maxCloneCount, Util.Remap(healthComponent.health, healthComponent.fullHealth * 0.25f, healthComponent.fullHealth, (float)maxCloneCount, (float)minCloneCount));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > projectileTime && !hasFired)
            {
                FireProjectileAuthority();
                hasFired = true;
                cloneTimer = cloneDelay;
            }
            if (hasFired)
            {
                if(cloneTimer < 0f && clonesFired < cloneCount)
                {
                    SpawnGhostEffect();
                    FireProjectileAuthority();
                    clonesFired++;
                    cloneTimer += cloneDelay;
                }
                cloneTimer -= GetDeltaTime();
            }
        }

        private void SpawnGhostEffect()
        {
            var effectData = new EffectData()
            {
                rootObject = base.gameObject,
                modelChildIndex = (short)modelChildLocator.FindChildIndex(muzzleFloor),
                origin = muzzleFloor.position
            };

            EffectManager.SpawnEffect(cloneEffect, effectData, false);
        }

        private void FireProjectileAuthority()
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
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
