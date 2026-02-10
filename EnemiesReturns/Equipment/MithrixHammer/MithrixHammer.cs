using EnemiesReturns.Configuration.Judgement;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Equipment.MithrixHammer
{
    public class MithrixHammer
    {
        public static GameObject MithrixHammerController;

        public static float aeonianHammerDamageModifier => Judgement.MithrixHammerAeonianBonusDamage.Value;

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.General.EnableJudgement.Value)
            {
                if (NetworkServer.active)
                {
                    if (body && body.inventory)
                    {
                        var result = body.AddItemBehavior<MithrixHammerOnDamageDealtServerReciever>(body.inventory.HasEquipment(Content.Equipment.MithrixHammer) ? 1 : 0);
                    }
                }
            }
        }

        public static bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            var hammerController = UnityEngine.Object.Instantiate(MithrixHammerController);
            hammerController.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.characterBody.gameObject, "Base");
            return true;
        }

        public static void SetupEquipmentConfigValues(EquipmentDef equipment)
        {
            equipment.cooldown = Judgement.MithrixHammerCooldown.Value;
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
