using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.BodyComponents
{
    public interface IKeyItemSlot
    {
        protected bool NeedToAddKeyItemSlot();

        internal KeyItemSlot AddKeyItemSlot(GameObject bodyPrefab)
        {
            KeyItemSlot slot = null;
            if (NeedToAddKeyItemSlot())
            {
                slot = bodyPrefab.GetOrAddComponent<KeyItemSlot>();
            }
            return slot;
        }
    }
}
