using RoR2;
using UnityEngine;

namespace EnemiesReturns.Items.PartyHat
{
    internal class PartyHatFactory
    {
        public static GameObject PartyHatDisplay;

        public static bool ShouldThrowParty()
        {
            return (Configuration.General.PartyTimeConfig.Value == Configuration.General.PartyTime.Default
                && System.DateTime.Now.Month == 8 && System.DateTime.Now.Day == 4)
                || Configuration.General.PartyTimeConfig.Value == Configuration.General.PartyTime.AllYear;
        }

        public GameObject SetupDisplayPrefab(GameObject displayPrefab)
        {
            var itemDisplay = displayPrefab.AddComponent<ItemDisplay>();
            itemDisplay.rendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo()
                {
                    renderer = displayPrefab.transform.Find("mdlPartyHat/Cone").GetComponent<MeshRenderer>(),
                    defaultMaterial = ContentProvider.MaterialCache["matPartyHat"],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = true,
                    ignoreOverlays = true,
                    ignoresMaterialOverrides = true,
                },
                new CharacterModel.RendererInfo()
                {
                    renderer = displayPrefab.transform.Find("mdlPartyHat/Sphere").GetComponent<MeshRenderer>(),
                    defaultMaterial = ContentProvider.MaterialCache["matPartyHat"],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = true,
                    ignoreOverlays = true,
                    ignoresMaterialOverrides = true,
                },
            };

            return displayPrefab;
        }

        public ItemDef CreateItem()
        {
            var itemDef = ScriptableObject.CreateInstance<ItemDef>();
            (itemDef as ScriptableObject).name = "ReturnsPartyHat";
            itemDef.tier = ItemTier.NoTier;
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.NoTier;
            itemDef.name = "ReturnsPartyHat";
            itemDef.nameToken = "ENEMIES_RETURNS_ITEM_PARTY_HAT_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_ITEM_PARTY_HAT_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_ITEM_PARTY_HAT_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_ITEM_PARTY_HAT_LORE";
#pragma warning restore CS0618 // Type or member is obsolete
            itemDef.canRemove = false;
            itemDef.tags = new ItemTag[] { ItemTag.WorldUnique, ItemTag.AIBlacklist, ItemTag.BrotherBlacklist };

            return itemDef;
        }
    }
}
