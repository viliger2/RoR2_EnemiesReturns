using RoR2;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface IEquipmentSlot
    {
        protected bool NeedToAddEquipmentSlot();

        internal EquipmentSlot AddEquipmentSlot(GameObject bodyPrefab)
        {
            EquipmentSlot slot = null;
            if (NeedToAddEquipmentSlot())
            {
                slot = bodyPrefab.GetOrAddComponent<EquipmentSlot>();
            }
            return slot;
        }
    }
}
