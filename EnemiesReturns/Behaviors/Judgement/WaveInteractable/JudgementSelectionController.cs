using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace EnemiesReturns.Behaviors.Judgement.WaveInteractable
{
    public class JudgementSelectionController : NetworkBehaviour
    {
        public struct MinMaxCount
        {
            public int minCount;
            public int maxCount;
        }

        private Dictionary<ItemTier, MinMaxCount> tierDropCounts = new Dictionary<ItemTier, MinMaxCount>
        {
            { ItemTier.Tier1, new MinMaxCount{ minCount = 25, maxCount = 35 } },
            { ItemTier.Tier2, new MinMaxCount{ minCount = 7, maxCount = 12 } },
            { ItemTier.Tier3, new MinMaxCount{ minCount = 1, maxCount = 3 } },
            { ItemTier.Boss, new MinMaxCount{ minCount = 2, maxCount = 5 } },
        };

        public int listCount = 10;

        public PickupDropTable[] tierDropTables;

        public PickupPickerController pickupPickerController;

        public Inventory inventory;

        private int currentList = 0;

        private PickupIndex[][] itemLists;

        private void Awake()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            GenerateItemList();
        }

        public void SetOptionsForMonsters(Interactor activator)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!activator)
            {
                return;
            }

            if (!inventory)
            {
                return;
            }

            List<PickupPickerController.Option> options = new List<PickupPickerController.Option>();
            var currentList = itemLists[this.currentList];
            for (int i = 0; i < currentList.Length; i++)
            {
                options.Add(new PickupPickerController.Option
                {
                    available = true,
                    pickupIndex = currentList[i]
                });
            }
            pickupPickerController.SetOptionsServer(options.ToArray());
        }

        public void GiveItemsToMonsters(int intPickupIndex)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            PickupDef pickupDef = PickupCatalog.GetPickupDef(new PickupIndex(intPickupIndex));
            if (pickupDef != null)
            {
                if (tierDropCounts.TryGetValue(pickupDef.itemTier, out var count))
                {
                    inventory.GiveItem(pickupDef.itemIndex, UnityEngine.Random.Range(count.minCount, count.maxCount + 1));
                }
            }

            currentList++;
        }

        private void GenerateItemList()
        {
            List<PickupIndex> takenList = new List<PickupIndex>();
            itemLists = new PickupIndex[listCount][];
            for (int i = 0; i < listCount; i++)
            {
                itemLists[i] = new PickupIndex[tierDropTables.Length];
                for (int k = 0; k < tierDropTables.Length; k++)
                {
                    var itemIndex = tierDropTables[k].GenerateDrop(Run.instance.stageRng);
                    while (takenList.Contains(itemIndex))
                    {
                        itemIndex = tierDropTables[k].GenerateDrop(Run.instance.stageRng);
                    }
                    takenList.Add(itemIndex);
                    itemLists[i][k] = itemIndex;
                }
            }
        }

    }
}
