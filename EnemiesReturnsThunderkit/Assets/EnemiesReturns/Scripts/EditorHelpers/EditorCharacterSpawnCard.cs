using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Bindings;

namespace EnemiesReturns.EditorHelpers
{
    [CreateAssetMenu(menuName = "EnemiesReturns/SpawnCards/EditorCharacterSpawnCard")]
    public class EditorCharacterSpawnCard : CharacterSpawnCard
    {
        public AssetReferenceT<GameObject> masterReference;

        public ItemNameCountPair[] itemNamesToGrant = Array.Empty<ItemNameCountPair>();

        public string[] equipmentNamesToGrant = Array.Empty<string>();

        [Serializable]
        public struct ItemNameCountPair
        {
            public string itemName;

            public int count;
        }

        public override void SetupSummonedInventory(MasterSummon masterSummon, Inventory summonedInventory)
        {
            for(int i = 0; i < itemNamesToGrant.Length; i++)
            {
                var itemDef = ItemCatalog.GetItemDef(ItemCatalog.FindItemIndex(itemNamesToGrant[i].itemName));
                if (itemDef)
                {
                    summonedInventory.GiveItemPermanent(itemDef, itemNamesToGrant[i].count);
                }
            }

            for(int i = 0; i < equipmentNamesToGrant.Length; i++)
            {
                var equipmentIndex = EquipmentCatalog.FindEquipmentIndex(equipmentNamesToGrant[i]);
                if (equipmentIndex != EquipmentIndex.None)
                {
                    var state = summonedInventory.GetEquipment((uint)i, 0u);
                    summonedInventory.SetEquipment(new EquipmentState(equipmentIndex, state.chargeFinishTime, state.charges), (uint)i, 0u);
                }
            }
        }

        public override void Spawn(Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest, ref SpawnResult result)
        {
            this.prefab = masterReference.LoadAssetAsync().WaitForCompletion();
            base.Spawn(position, rotation, directorSpawnRequest, ref result);
        }
    }
}
