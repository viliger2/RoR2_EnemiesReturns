using EnemiesReturns.Configuration.Judgement;
using EnemiesReturns.Equipment.MithrixHammer;
using R2API;
using RoR2;
using RoR2.EntityLogic;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.Equipment.VoidlingWeapon
{
    public class VoidlingWeapon
    {
        public static GameObject VoidlingWeaponController;

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                if (NetworkServer.active)
                {
                    if (body && body.inventory)
                    {
                        body.AddItemBehavior<VoidlingWeaponOnDamageDealtServerReciever>(body.inventory.HasEquipment(Content.Equipment.VoidlingWeapon) ? 1 : 0);
                    }
                }
            }
        }

        public static void SetupEquipmentConfigValues(EquipmentDef equipment)
        {
            equipment.cooldown = Configuration.Judgement.Judgement.VoidlingWeaponCooldown.Value;
        }

        public static GameObject SetupPickupDisplay(GameObject mithrixHammer)
        {
            mithrixHammer.transform.Find("Model/Eye").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_VoidRaidCrab.matVoidRaidCrabEye_mat).WaitForCompletion();

            return mithrixHammer;
        }

        public static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            self.UpdateTargets(Content.Equipment.VoidlingWeapon.equipmentIndex, true);

            if (self.currentTarget.hurtBox)
            {
                var weaponController = UnityEngine.Object.Instantiate(VoidlingWeaponController);
                weaponController.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.characterBody.gameObject, "Base");
                return true;
            }
            return false;
        }
    }
}
