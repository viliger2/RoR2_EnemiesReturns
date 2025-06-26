using RoR2;
using RoR2BepInExPack.Utilities;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(BossGroup))]
    public class BossGroupHealthColorOverride : MonoBehaviour
    {
        public Color healthBarColorOverride;

        public BossGroup bossGroup;

        public readonly static Color defaultHealthBarColor = new Color(206f / 255f, 3f / 255f, 3f / 255f);

        public static FixedConditionalWeakTable<BossGroup, BossGroupHealthColorOverride> overrideDictionary = new FixedConditionalWeakTable<BossGroup, BossGroupHealthColorOverride>();

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

        private void OnDisable()
        {
            if (bossGroup)
            {
                overrideDictionary.Remove(bossGroup);
            }
        }

        public static void ReplaceColor(RoR2.UI.HUDBossHealthBarController self)
        {
            if (!self || !self.currentBossGroup)
            {
                return;
            }
            if (self.fillRectImage)
            {
                if (overrideDictionary.TryGetValue(self.currentBossGroup, out var component))
                {
                    self.fillRectImage.color = component.healthBarColorOverride;
                }
                else
                {
                    self.fillRectImage.color = defaultHealthBarColor;
                }
            }
        }
    }
}
