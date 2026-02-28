using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.ContactLight
{
    [CreateAssetMenu(menuName = "EnemiesReturns/DropTables/CustomExplicitDropTable")]
    public class CustomExplicitDropTable : PickupDropTable
    {
        public string[] entries;

        private readonly WeightedSelection<UniquePickup> weightedSelection = new WeightedSelection<UniquePickup>();

        public override void Regenerate(Run run)
        {
            GenerateWeightedSelection();
        }

        private void GenerateWeightedSelection()
        {
            weightedSelection.Clear();
            for (int i = 0; i < entries.Length; i++)
            {
                var entry = PickupCatalog.FindPickupIndex(entries[i]);
                if(entry != PickupIndex.none)
                {
                    weightedSelection.AddChoice(new UniquePickup(entry), 1f);
                }
            }
        }

        public override UniquePickup GeneratePickupPreReplacement(Xoroshiro128Plus rng)
        {
            return PickupDropTable.GeneratePickupFromWeightedSelection(rng, weightedSelection);
        }

        public override void GenerateDistinctPickupsPreReplacement(List<UniquePickup> dest, int desiredCount, Xoroshiro128Plus rng)
        {
            PickupDropTable.GenerateDistinctFromWeightedSelection(dest, desiredCount, rng, weightedSelection);
        }

        public override int GetPickupCount()
        {
            return weightedSelection.Count;
        }
    }
}
