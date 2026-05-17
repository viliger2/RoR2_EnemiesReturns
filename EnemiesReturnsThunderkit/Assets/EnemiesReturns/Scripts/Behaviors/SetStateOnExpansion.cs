using RoR2;
using RoR2.ExpansionManagement;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors
{
    public class SetStateOnExpansion : MonoBehaviour
    {
        public AssetReferenceT<ExpansionDef> expansionReference;

        public bool state;

        private void Awake()
        {
            var expansionDef = expansionReference.LoadAssetAsync().WaitForCompletion();
            if (expansionDef)
            {
                if (Run.instance.IsExpansionEnabled(expansionDef))
                {
                    base.gameObject.SetActive(state);
                }
                else
                {
                    base.gameObject.SetActive(!state);
                }
            }
        }
    }
}
