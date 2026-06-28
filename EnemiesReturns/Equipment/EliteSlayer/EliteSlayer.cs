using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.EquipmentSlot;

namespace EnemiesReturns.Equipment.EliteSlayer
{
    public class EliteSlayer
    {
        public static GameObject EliteSlayerIndicator;

        public static GameObject eliteSlayerProjectilePrefab;

        public static void UpdateTargets(EquipmentSlot self)
        {
            self.ConfigureTargetFinderForEnemies();
            HurtBox source = null;
            foreach (var target in self.targetFinder.GetResults())
            {
                if (target && target.healthComponent && target.healthComponent.body && target.healthComponent.body.isElite)
                {
                    source = target;
                    break;
                }
            };
            self.currentTarget = new UserTargetInfo(source);
            var hasTarget = self.currentTarget.transformToIndicateAt;
            if (hasTarget)
            {
                self.targetIndicator.visualizerPrefab = EliteSlayerIndicator;
            }
            self.targetIndicator.active = hasTarget;
            self.targetIndicator.targetTransform = (hasTarget ? self.currentTarget.transformToIndicateAt : null);
        }

        public static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            self.UpdateTargets(Content.Equipment.EliteSlayer.equipmentIndex, true);
            HurtBox hurtBox = self.currentTarget.hurtBox;
            if (hurtBox)
            {
                var info = new FireProjectileInfo
                {
                    projectilePrefab = eliteSlayerProjectilePrefab,
                    position = hurtBox.transform.position,
                    target = hurtBox.gameObject,
                    damage = 1f,
                    owner = self.gameObject,
                    useFuseOverride = true,
                    fuseOverride = 0.5f
                };

                ProjectileManager.instance.FireProjectile(info);

                // TODO: replace with empty equipment, removing for now
                if (self.characterBody && self.characterBody.inventory)
                {
                    self.characterBody.inventory.RemoveEquipment(Content.Equipment.EliteSlayer.equipmentIndex);
                }
                self.InvalidateCurrentTarget();
                return true;
            }

            return false;
        }
    }
}
