using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileExplosionComboLimit : ProjectileExplosion
    {
        [Header("Combo Limit Additions")]
        public bool adjustSpawnPoint;

        public bool fireBullet;

        public Vector3 bullerDirection = Vector3.up;

        public GameObject bulletAttackEffect => Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Lightning.LightningStrikeImpact_prefab).WaitForCompletion();

        public float bulletAttackRadius;

        public float bulletAttackDamageCoefficient;

        public override void FireChild()
        {
            if(projectileController.combo == 0)
            {
                return;
            }
            base.FireChild();
        }

        public override void DetonateServer()
        {
            base.DetonateServer();
            if (fireBullet)
            {
                FireBulletServer();
            }
        }

        public void FireBulletServer()
        {
            var bulletAttack = new BulletAttack
            {
                aimVector = Vector3.up,
                bulletCount = 1,
                damage = projectileDamage.damage * bulletAttackDamageCoefficient,
                falloffModel = BulletAttack.FalloffModel.None,
                damageType = DamageSource.Utility,
                hitMask = LayerIndex.entityPrecise.mask,
                stopperMask = -1,
                maxDistance = 1000f,
                owner = projectileController.owner,
                minSpread = 0f,
                maxSpread = 0f,
                radius = bulletAttackRadius,
                origin = base.transform.position,
                isCrit = projectileDamage.crit
            };
            bulletAttack.Fire();
            if (bulletAttackEffect)
            {
                EffectManager.SimpleEffect(bulletAttackEffect, base.transform.position, Quaternion.identity, true);
            }
        }


        public override void InitializeChild(GameObject childProjectileInstance)
        {
            base.InitializeChild(childProjectileInstance);
            var controller = childProjectileInstance.GetComponent<ProjectileController>();
            if (controller)
            {
                controller.Networkcombo = (byte)(projectileController.combo - 1);
            }
            if (adjustSpawnPoint)
            {
                var sphereCollider = childProjectileInstance.GetComponent<SphereCollider>();
                if (sphereCollider)
                {
                    childProjectileInstance.transform.position = childProjectileInstance.transform.position + Vector3.up * (sphereCollider.radius * 2);
                }
            }
        }
    }
}
