using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Items.SpawnPillarOnChampionKill
{
    internal class SpawnPillarOnChampionKillFactory
    {
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
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.Boss;
            itemDef.name = "SpawnPillarOnChampionKill";
            itemDef.nameToken = "ENEMIES_RETURNS_ITEM_SPAWN_PILLAR_ON_CHAMPION_KILL_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_ITEM_SPAWN_PILLAR_ON_CHAMPION_KILL_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_SPAWN_PILLAR_ON_CHAMPION_KILL_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_ITEM_SPAWN_PILLAR_ON_CHAMPION_KILL_LORE";
            itemDef.pickupModelPrefab = prefab;
#pragma warning restore CS0618 // Type or member is obsolete
            itemDef.canRemove = true;
            itemDef.pickupIconSprite = icon;
            itemDef.tags = new ItemTag[] { ItemTag.Damage, ItemTag.CannotCopy, ItemTag.OnKillEffect, ItemTag.CanBeTemporary };

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
        }

        public static void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (Configuration.Ifrit.ItemEnabled.Value)
            {
                if (NetworkServer.active)
                {
                    if (body && body.inventory)
                    {
                        body.AddItemBehavior<PillarItemBehavior>(body.inventory.GetItemCountEffective(Content.Items.SpawnPillarOnChampionKill));
                    }
                }
            }
        }

        private static void Language_onCurrentLangaugeChanged(RoR2.Language language, List<KeyValuePair<string, string>> output)
        {
            string tokenValue = "";
            if (Configuration.Ifrit.SpawnPillarOnChampionKillEliteKills.Value)
            {
                tokenValue = "ENEMIES_RETURNS_SPAWN_PILLAR_ON_CHAMPION_KILL_DESCRIPTION_ELITES";
            } else
            {
                tokenValue = "ENEMIES_RETURNS_SPAWN_PILLAR_ON_CHAMPION_KILL_DESCRIPTION_NO_ELITES";
            }

            var keyPair = output.Find(item => item.Key == tokenValue);
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
