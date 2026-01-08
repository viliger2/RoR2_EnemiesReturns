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

        public static GameObject ghostEffect;

        //public static float projectileTime => Configuration.General.ProvidenceP1PrimaryProjectileTime.Value;
        public static float projectileTime => 0.1f;

        public static float ghostDelay = 0.2f;

        public static int ghostCount = 2;

        public override float swingDamageCoefficient => 2f;

        public override float swingProcCoefficient => 1f;

        public override float swingForce => 0f;

        public override GameObject hitEffect => null;

        public override string swingSoundEffect => "";

        private float ghostTimer;

        private int ghostsFired;

        private ChildLocator modelChildLocator;

        private Transform muzzleFloor;

        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            modelChildLocator = GetModelChildLocator();
            muzzleFloor = FindModelChild("MuzzleFloor");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > projectileTime && !hasFired)
            {
                FireProjectileAuthority();
                hasFired = true;
                ghostTimer = ghostDelay;
            }
            if (hasFired)
            {
                if(ghostTimer < 0f && ghostsFired < ghostCount)
                {
                    SpawnGhostEffect();
                    FireProjectileAuthority();
                    ghostsFired++;
                    ghostTimer += ghostDelay;
                }
                ghostTimer -= GetDeltaTime();
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

            EffectManager.SpawnEffect(ghostEffect, effectData, false);
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
