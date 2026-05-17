using RoR2;
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
            if (hurtBox && hurtBox.healthComponent && hurtBox.healthComponent.body && hurtBox.healthComponent.body.isElite && hurtBox.healthComponent.body.inventory)
            {
                var inventory = hurtBox.healthComponent.body.inventory;

                var equipment = inventory.GetActiveEquipment();
                if (!equipment.equipmentDef || !equipment.equipmentDef.passiveBuffDef)
                {
                    return false;
                }

                if (hurtBox.healthComponent.body.master)
                {
                    hurtBox.healthComponent.body.master.TrueKill(self.gameObject);
                }

                Vector3 vector = (hurtBox.transform ? hurtBox.transform.position : Vector3.zero);
                Vector3 normalized = (vector - self.characterBody.corePosition).normalized;
                PickupDropletController.CreatePickupDroplet(new UniquePickup(PickupCatalog.FindPickupIndex(equipment.equipmentDef.equipmentIndex)), vector, normalized * 15f, false);

                // TODO: effects

                // TODO: replace with empty equipment, removing for now
                if (self.characterBody && self.characterBody.inventory)
                {
                    self.characterBody.inventory.RemoveEquipment(Content.Equipment.EliteSlayer.equipmentIndex);
                }
                return true;
            }

            return false;
        }
    }
}
