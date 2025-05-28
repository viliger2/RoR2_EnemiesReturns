using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Equipment.MithrixHammer
{
    public class MithrixHammer
    {
        public static GameObject MithrixHammerController;

        public static float aeonianHammerDamageModifier => Configuration.Judgement.MithrixHammerAeonianBonusDamage.Value;

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

        public static void ModifyDamageOnAeonianElitesFromHammer(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            if (self.body)
            {
                if(self.body.HasBuff(Content.Buffs.AffixAeoninan) && (damageInfo.damageType.damageSource & DamageSource.Equipment) == DamageSource.Equipment && damageInfo.damageType.HasModdedDamageType(Content.DamageTypes.EndGameBossWeapon))
                {
                    damageInfo.damage *= aeonianHammerDamageModifier;
                }
            }

            orig(self, damageInfo);
        }

        public static void SetupEquipmentConfigValues(EquipmentDef equipment)
        {
            equipment.cooldown = Configuration.Judgement.MithrixHammerCooldown.Value;
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
