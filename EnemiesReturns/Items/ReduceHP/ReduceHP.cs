using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Items.ReduceHP
{
    internal class ReduceHP
    {
        public static readonly float healthReductionFraction = 0.01f;

        public static void Hooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            if(sender && sender.inventory && sender.inventory.GetItemCountPermanent(Content.Items.ReduceHP) > 0)
            {
                args.healthTotalMult -= (sender.inventory.GetItemCountPermanent(Content.Items.ReduceHP) * healthReductionFraction);
            }
        }

        public ItemDef CreateItem()
        {
            var itemDef = ScriptableObject.CreateInstance<ItemDef>();
            (itemDef as ScriptableObject).name = "ReduceHP";
            itemDef.tier = ItemTier.NoTier;
#pragma warning disable CS0618 // Type or member is obsolete
            itemDef.deprecatedTier = ItemTier.NoTier;
            itemDef.name = "ReduceHP";
            itemDef.nameToken = "ENEMIES_RETURNS_REDUCEHP_NAME";
            itemDef.pickupToken = "ENEMIES_RETURNS_REDUCEHP_PICKUP";
            itemDef.descriptionToken = "ENEMIES_RETURNS_REDUCEHP_DESCRIPTION";
            itemDef.loreToken = "ENEMIES_RETURNS_REDUCEHP_LORE";
#pragma warning restore CS0618 // Type or member is obsolete
            itemDef.canRemove = false;
            itemDef.tags = new ItemTag[] { ItemTag.Healing };

            return itemDef;
        }

    }
}
