using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(BossGroup))]
    public class BossGroupTextOverride : MonoBehaviour
    {
        public string nameTokenOverride;

        public string subtitleTokenOverride;

        public BossGroup bossGroup;

        public static ConditionalWeakTable<BossGroup, BossGroupTextOverride> overrideDictionary = new ConditionalWeakTable<BossGroup, BossGroupTextOverride>();

        private void OnEnable()
        {
            if (!bossGroup)
            {
                bossGroup = GetComponent<BossGroup>();
            }
            if (bossGroup)
            {
                overrideDictionary.Add(bossGroup, this);
            }
        }

        public static void BossGroup_UpdateObservations(On.RoR2.BossGroup.orig_UpdateObservations orig, BossGroup self, ValueType memory)
        {
            orig(self, memory);

            if (overrideDictionary.TryGetValue(self, out var component))
            {
                if (!string.IsNullOrEmpty(component.nameTokenOverride))
                {
                    self.bestObservedName = component.nameTokenOverride;
                }
                if (!string.IsNullOrEmpty(component.subtitleTokenOverride))
                {
                    self.bestObservedSubtitle = "<sprite name=\"CloudLeft\" tint=1> " + component.subtitleTokenOverride + "<sprite name=\"CloudRight\" tint=1>";
                }
            }
        }

        private void OnDisable()
        {
            if (bossGroup)
            {
                overrideDictionary.Remove(bossGroup);
            }
        }
    }
}
