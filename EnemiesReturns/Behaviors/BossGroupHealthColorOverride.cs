using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    // TODO: add scene component that just does on hooks for that specific scene
    [RequireComponent(typeof(BossGroup))]
    public class BossGroupHealthColorOverride : MonoBehaviour
    {
        public Color healthBarColorOverride;

        public BossGroup bossGroup;

        public readonly static Color defaultHealthBarColor = new Color(206f/255f, 3f/255f, 3f/255f);

        public static ConditionalWeakTable<BossGroup, BossGroupHealthColorOverride> overrideDictionary = new ConditionalWeakTable<BossGroup, BossGroupHealthColorOverride>();

        private void OnEnable()
        {
            if (!bossGroup)
            {
                bossGroup = GetComponent<BossGroup>();
            }
            overrideDictionary.Add(bossGroup, this);
        }

        private void OnDisable()
        {
            if (bossGroup)
            {
                overrideDictionary.Remove(this.gameObject.GetComponent<BossGroup>());
            }
        }

        public static void Hooks()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
        }

        private static void HUDBossHealthBarController_LateUpdate(On.RoR2.UI.HUDBossHealthBarController.orig_LateUpdate orig, RoR2.UI.HUDBossHealthBarController self)
        {
            orig(self);
            if(self.currentBossGroup && self.fillRectImage)
            {
                if(overrideDictionary.TryGetValue(self.currentBossGroup, out var component))
                {
                    self.fillRectImage.color = component.healthBarColorOverride;
                } else
                {
                    self.fillRectImage.color = defaultHealthBarColor;
                }
            }
        }
    }
}
