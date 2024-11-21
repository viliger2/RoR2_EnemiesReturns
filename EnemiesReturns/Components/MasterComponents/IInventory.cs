using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.MasterComponents
{
    public interface IInventory
    {
        protected bool NeedToAddInventory();

        internal Inventory AddInventory(GameObject masterPrefab)
        {
            Inventory inventory = null;
            if (NeedToAddInventory())
            {
                inventory = masterPrefab.GetOrAddComponent<Inventory>();
            }
            return inventory;
        }

    }
}
