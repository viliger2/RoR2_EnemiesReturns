using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.EquipmentSlot;

namespace EnemiesReturns.Equipment.VoidlingWeapon
{
    public class VoidlingWeapon
    {
        public static GameObject VoidlingWeaponController;

        public static GameObject VoidlingWeaponIndicator;

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.General.EnableJudgement.Value)
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

        public static void UpdateTargets(EquipmentSlot self)
        {
            self.ConfigureTargetFinderForEnemies();
            var source = self.targetFinder.GetResults().FirstOrDefault();
            self.currentTarget = new UserTargetInfo(source);
            var hasTarget = self.currentTarget.transformToIndicateAt;
            if (hasTarget)
            {
                self.targetIndicator.visualizerPrefab = VoidlingWeaponIndicator;
            }
            self.targetIndicator.active = hasTarget;
            self.targetIndicator.targetTransform = (hasTarget ? self.currentTarget.transformToIndicateAt : null);
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
