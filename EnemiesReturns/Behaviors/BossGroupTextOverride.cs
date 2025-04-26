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

        public static void ReplaceNames(RoR2.UI.HUDBossHealthBarController self)
        {
            if (!self || !self.currentBossGroup)
            {
                return;
            }
            if(overrideDictionary.TryGetValue(self.currentBossGroup, out var component))
            {
                if (!string.IsNullOrEmpty(component.nameTokenOverride))
                {
                    self.bossNameLabel.SetText(RoR2.Language.GetString(component.nameTokenOverride));
                }
                if (!string.IsNullOrEmpty(component.subtitleTokenOverride))
                {
                    self.bossSubtitleLabel.SetText(RoR2.Language.GetString(component.subtitleTokenOverride));
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
