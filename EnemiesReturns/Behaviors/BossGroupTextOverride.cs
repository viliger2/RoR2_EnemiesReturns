using RoR2;
using RoR2BepInExPack.Utilities;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(BossGroup))]
    public class BossGroupTextOverride : MonoBehaviour
    {
        public string nameTokenOverride;

        public string subtitleTokenOverride;

        public BossGroup bossGroup;

        private void OnEnable()
        {
            if (!bossGroup)
            {
                bossGroup = GetComponent<BossGroup>();
            }
        }

        public static void ReplaceNames(RoR2.UI.HUDBossHealthBarController self)
        {
            if (!self || !self.currentBossGroup)
            {
                return;
            }

            var textOverride = self.currentBossGroup.gameObject.GetComponent<BossGroupTextOverride>();
            if (textOverride)
            {
                if (!string.IsNullOrEmpty(textOverride.nameTokenOverride))
                {
                    self.bossNameLabel.SetText(RoR2.Language.GetString(textOverride.nameTokenOverride));
                }
                if (!string.IsNullOrEmpty(textOverride.subtitleTokenOverride))
                {
                    self.bossSubtitleLabel.SetText(RoR2.Language.GetString(textOverride.subtitleTokenOverride));
                }
            }
        }
    }
}
