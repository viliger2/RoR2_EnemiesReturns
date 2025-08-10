using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Behaviors.Judgement.WaveInteractable
{
    public class JudgementSelectionController : NetworkBehaviour
    {
        public static GameObject modifiedPickerPanel;

        public struct EnemyItems
        {
            public PickupIndex pickupIndex;
            public int count;
        }

        public struct MinMaxCount
        {
            public int minCount;
            public int maxCount;
        }

        private Dictionary<ItemTier, MinMaxCount> tierDropCounts = new Dictionary<ItemTier, MinMaxCount>
        {
            { ItemTier.Tier1, new MinMaxCount{ minCount = Configuration.Judgement.Judgement.WavesTier1ItemMinCount.Value, maxCount = Configuration.Judgement.Judgement.WavesTier1ItemMaxCount.Value } },
            { ItemTier.Tier2, new MinMaxCount{ minCount = Configuration.Judgement.Judgement.WavesTier2ItemMinCount.Value, maxCount = Configuration.Judgement.Judgement.WavesTier2ItemMaxCount.Value } },
            { ItemTier.Tier3, new MinMaxCount{ minCount = Configuration.Judgement.Judgement.WavesTier3ItemMinCount.Value, maxCount = Configuration.Judgement.Judgement.WavesTier3ItemMaxCount.Value } },
            { ItemTier.Boss, new MinMaxCount{ minCount = Configuration.Judgement.Judgement.WavesTierBossItemMinCount.Value, maxCount = Configuration.Judgement.Judgement.WavesTierBossItemMaxCount.Value } },
            { ItemTier.Lunar, new MinMaxCount{minCount = Configuration.Judgement.Judgement.WavesTier3ItemMinCount.Value, maxCount = Configuration.Judgement.Judgement.WavesTier1ItemMaxCount.Value} }
        };

        public int listCount = 10;

        public JudgmentMonsterItemDropTable[] tierDropTables;

        public PickupPickerController pickupPickerController;

        public Inventory inventory;

        private int currentList = 0;

        private EnemyItems[][] itemLists;

        private Interactor interactor;

        private const uint ITEM_LIST_DIRTY_BIT = 1u;

        private void Awake()
        {
            if (pickupPickerController && modifiedPickerPanel)
            {
                pickupPickerController.panelPrefab = modifiedPickerPanel;
            }
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

            interactor = activator;

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
                    pickupIndex = currentList[i].pickupIndex
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

            var pickupIndex = new PickupIndex(intPickupIndex);

            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef != null)
            {
                var itemCount = GetItemCountFromList(pickupIndex);
                if(itemCount < 0)
                {
                    itemCount = 10;
                }
                inventory.GiveItem(pickupDef.itemIndex, itemCount);

                if (interactor)
                {
                    ScrapperController.CreateItemTakenOrb(interactor.transform.position, this.gameObject, pickupDef.itemIndex);

                    Chat.SendBroadcastChat(new Behaviors.SubjectParamsChatMessage
                    {
                        subjectAsCharacterBody = interactor.GetComponent<CharacterBody>(),
                        baseToken = "ENEMIES_RETURNS_JUDGEMENT_OPTION_SELECTED",
                        paramsTokens = new string[]
                        {
                            Util.GenerateColoredString(RoR2.Language.GetString(pickupDef.nameToken) ?? "???", pickupDef.baseColor),
                            itemCount.ToString()
                        }
                    });
                }
            }

            currentList++;
        }

        public int GetItemCountFromList(PickupIndex pickupIndex)
        {
            for (int i = 0; i < itemLists.Length; i++)
            {
                for (int k = 0; k < itemLists[i].Length; k++)
                {
                    if (itemLists[i][k].pickupIndex == pickupIndex)
                    {
                        return itemLists[i][k].count;
                    }
                }
            }
            return -1;
        }

        private void GenerateItemList()
        {
            HashSet<PickupIndex> takenList = new HashSet<PickupIndex>();
            itemLists = new EnemyItems[listCount][];
            for (int i = 0; i < listCount; i++)
            {
                itemLists[i] = new EnemyItems[tierDropTables.Length];
                for (int k = 0; k < tierDropTables.Length; k++)
                {
                    var itemIndex = tierDropTables[k].GenerateDrop(Run.instance.stageRng);

                    if (tierDropTables[k].GetSelectorCount() >= listCount)
                    {
                        var retryCount = 0;
                        while (takenList.Contains(itemIndex) && retryCount < 3)
                        {
                            itemIndex = tierDropTables[k].GenerateDrop(Run.instance.stageRng);
                            retryCount++;
                        }
                    }
                    takenList.Add(itemIndex);
                    var itemCount = 10;
                    PickupDef pickupDef = PickupCatalog.GetPickupDef(itemIndex);
                    if (tierDropCounts.TryGetValue(pickupDef.itemTier, out var count))
                    {
                        itemCount = UnityEngine.Random.Range(count.minCount, count.maxCount + 1);

                    }
                    itemLists[i][k] = new EnemyItems { count = itemCount, pickupIndex = itemIndex };
                }
            }
            SetDirtyBit(ITEM_LIST_DIRTY_BIT);
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            uint num = base.syncVarDirtyBits;
            if (initialState)
            {
                num = ITEM_LIST_DIRTY_BIT;
            }

            writer.WritePackedUInt32(num);
            if ((num & ITEM_LIST_DIRTY_BIT) != 0)
            {
                writer.WritePackedUInt32((uint)itemLists.Length);
                for (int i = 0; i < itemLists.Length; i++)
                {
                    writer.WritePackedUInt32((uint)itemLists[i].Length);
                    for (int k = 0; k < itemLists[i].Length; k++)
                    {
                        writer.Write(itemLists[i][k].pickupIndex);
                        writer.WritePackedUInt32((uint)(itemLists[i][k].count));    
                    }
                }
            }

            return num != 0;
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            uint num = reader.ReadPackedUInt32();
            if((num & ITEM_LIST_DIRTY_BIT) != 0)
            {
                itemLists = new EnemyItems[reader.ReadPackedUInt32()][];
                for(int i = 0; i < itemLists.Length; i++)
                {
                    itemLists[i] = new EnemyItems[reader.ReadPackedUInt32()];
                    for(int k = 0; k < itemLists[i].Length; k++)
                    {
                        itemLists[i][k] = new EnemyItems { pickupIndex = reader.ReadPickupIndex(), count = (int)reader.ReadPackedUInt32() };
                    }
                }
            }
        }
    }
}
