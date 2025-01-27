using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Items.SpawnPillarOnChampionKill
{
    internal class SpawnPillarOnChampionKillFactory
    {
        public static ItemDef ItemDef;

        public ItemDef CreateItem(GameObject prefab, Sprite icon)
        {
            var fire = prefab.transform.Find("mdlIfritItem/Fire");
            fire.gameObject.GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matIfritLanternFire", CreateLanternFireMaterial);

            var modelPanelParameters = prefab.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = prefab.transform.Find("FocusPoint");
            modelPanelParameters.cameraPositionTransform = prefab.transform.Find("CameraPosition");
            modelPanelParameters.modelRotation = Quaternion.identity;
            modelPanelParameters.minDistance = 1f;
            modelPanelParameters.maxDistance = 3f;

            var itemDef = ScriptableObject.CreateInstance<ItemDef>();
            (itemDef as ScriptableObject).name = "SpawnPillarOnChampionKill";
            itemDef.tier = ItemTier.Boss;
            itemDef.deprecatedTier = ItemTier.Boss;
            itemDef.name = "SpawnPillarOnChampionKill";
            itemDef.nameToken = "ENEMIES_RETURNS_ITEM_SPAWN_PILLAR_ON_CHAMPION_KILL_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_ITEM_SPAWN_PILLAR_ON_CHAMPION_KILL_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_SPAWN_PILLAR_ON_CHAMPION_KILL_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_ITEM_SPAWN_PILLAR_ON_CHAMPION_KILL_LORE";
            itemDef.pickupModelPrefab = prefab;
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.Damage, ItemTag.CannotCopy };

            return itemDef;
        }

        private Material CreateLanternFireMaterial()
        {
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC2/helminthroost/Assets/matHRFireStaticRedLArge.mat").WaitForCompletion());
            material.name = "matIfritLanternFire";
            material.SetFloat("_DepthOffset", -10f);

            return material;
        }

        public static void Hooks()
        {
            EnemiesReturns.Language.onCurrentLangaugeChanged += Language_onCurrentLangaugeChanged;
            RoR2.Inventory.onInventoryChangedGlobal += Inventory_onInventoryChangedGlobal;
        }

        private static void Inventory_onInventoryChangedGlobal(Inventory inventory)
        {
            var master = inventory.GetComponent<CharacterMaster>();
            if (master)
            {
                var body = master.GetBody();
                if (body)
                {
                    body.AddItemBehavior<PillarItemBehavior>(inventory.GetItemCount(ItemDef));
                }
            }
        }

        private static void Language_onCurrentLangaugeChanged(RoR2.Language language, List<KeyValuePair<string, string>> output)
        {
            var keyPair = output.Find(item => item.Key == "ENEMIES_RETURNS_SPAWN_PILLAR_ON_CHAMPION_KILL_DESCRIPTION");
            if (!keyPair.Equals(default(KeyValuePair<string, string>)))
            {
                string description = string.Format(
                    keyPair.Value,
                    EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillChargeTime.Value,
                    EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillDamage.Value.ToString("###%"),
                    EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillDamagePerStack.Value.ToString("###%"),
                    EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillRadius.Value,
                    (EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillEliteChance.Value / 100f).ToString("###%")
                    );
                language.SetStringByToken("ENEMIES_RETURNS_SPAWN_PILLAR_ON_CHAMPION_KILL_DESCRIPTION", description);
            }
        }
    }
}
