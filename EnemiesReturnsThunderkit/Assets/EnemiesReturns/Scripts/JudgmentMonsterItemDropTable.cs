using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Judgement
{
    // I hecking love copypasting code because Hopoo couldn't be fucking arsed to do a simple null check
    [CreateAssetMenu(menuName = "EnemiesReturns/DropTables/JudgmentMonsterItemDropTable")]
    public class JudgmentMonsterItemDropTable : PickupDropTable
    {
        public ItemTag[] requiredItemTags;

        public ItemTag[] bannedItemTags;

        public float tier1Weight = 0.8f;

        public float tier2Weight = 0.2f;

        public float tier3Weight = 0.01f;

        public float bossWeight;

        public float lunarItemWeight;

        public float voidTier1Weight;

        public float voidTier2Weight;

        public float voidTier3Weight;

        public float voidBossWeight;

        private readonly WeightedSelection<PickupIndex> selector = new WeightedSelection<PickupIndex>();

        private void Add(List<PickupIndex> sourceDropList, float chance)
        {
            if (chance <= 0f || sourceDropList.Count == 0)
            {
                return;
            }
            foreach (PickupIndex sourceDrop in sourceDropList)
            {
                if (PassesFilter(sourceDrop))
                {
                    selector.AddChoice(sourceDrop, chance);
                }
            }
        }

        private void GenerateWeightedSelection(Run run)
        {
            selector.Clear();
            Add(run.availableTier1DropList, tier1Weight);
            Add(run.availableTier2DropList, tier2Weight);
            Add(run.availableTier3DropList, tier3Weight);
            Add(run.availableBossDropList, bossWeight);
            Add(run.availableLunarItemDropList, lunarItemWeight);
            Add(run.availableVoidTier1DropList, voidTier1Weight);
            Add(run.availableVoidTier2DropList, voidTier2Weight);
            Add(run.availableVoidTier3DropList, voidTier3Weight);
            Add(run.availableVoidBossDropList, voidBossWeight);
        }

        public override PickupIndex GenerateDropPreReplacement(Xoroshiro128Plus rng)
        {
            GenerateWeightedSelection(Run.instance);
            return PickupDropTable.GenerateDropFromWeightedSelection(rng, selector);
        }

        public override int GetPickupCount()
        {
            return selector.Count;
        }

        public override PickupIndex[] GenerateUniqueDropsPreReplacement(int maxDrops, Xoroshiro128Plus rng)
        {
            return PickupDropTable.GenerateUniqueDropsFromWeightedSelection(maxDrops, rng, selector);
        }

        private bool PassesFilter(PickupIndex pickupIndex)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef.itemIndex != ItemIndex.None)
            {
                ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
                if (JudgementMissionController.instance && JudgementMissionController.instance.inventory.GetItemCount(itemDef.itemIndex) > 0)
                {
                    return false;
                }
                ItemTag[] array = requiredItemTags;
                foreach (ItemTag value in array)
                {
                    if (Array.IndexOf(itemDef.tags, value) == -1)
                    {
                        return false;
                    }
                }
                array = bannedItemTags;
                foreach (ItemTag value2 in array)
                {
                    if (Array.IndexOf(itemDef.tags, value2) != -1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
