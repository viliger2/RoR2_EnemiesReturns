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

        private void OnEnable()
        {
            if (!bossGroup)
            {
                bossGroup = GetComponent<BossGroup>();
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
                if (self.gameObject.TryGetComponent<BossGroupHealthColorOverride>(out var component))
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
