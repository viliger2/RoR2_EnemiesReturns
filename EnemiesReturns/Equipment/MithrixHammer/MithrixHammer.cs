using EnemiesReturns.Components;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Equipment.MithrixHammer
{
    public class MithrixHammer
    {
        public static GameObject MithrixHammerController;

        public static void Hooks()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if(equipmentDef.equipmentIndex == Content.Equipment.MithrixHammer.equipmentIndex)
            {
                var hammerController = UnityEngine.Object.Instantiate(MithrixHammerController);
                hammerController.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.characterBody.gameObject, "Base");
                return true;
            }
            return orig(self, equipmentDef);
        }

        public static GameObject SetupPickupDisplay(GameObject mithrixHammer)
        {
            mithrixHammer.transform.Find("equipBrotherWeapon/BrotherHammerConcrete").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherHammer.mat").WaitForCompletion();
            mithrixHammer.transform.Find("equipBrotherWeapon/BrotherHammerStib").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherStib.mat").WaitForCompletion();

            return mithrixHammer;
        }

        public static GameObject SetupEffectMaterials(GameObject swingEffect)
        {
            swingEffect.transform.Find("equipBrotherWeaponAnimation/BrotherHammerConcrete").GetComponent<SkinnedMeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherHammer.mat").WaitForCompletion();
            swingEffect.transform.Find("equipBrotherWeaponAnimation/BrotherHammerStib").GetComponent<SkinnedMeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherStib.mat").WaitForCompletion();
            return swingEffect;
        }
    }
}
