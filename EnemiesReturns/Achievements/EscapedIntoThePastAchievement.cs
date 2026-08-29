using Assets.RoR2.Scripts.Platform;
using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Achievements
{
    // TODO: remove 1 with full release
    [RegisterAchievement("EnemiesReturnsEscapedIntoThePast", "Interactables.ER_Wardrobe.0", null, 3u)]
    public class EscapedIntoThePastAchievement : BaseEndingAchievement
    {
        public override bool ShouldGrant(RunReport runReport)
        {
            if (runReport.gameEnding == Content.GameEndings.EscapeIntoPast)
            {
                return true;
            }
            return false;
        }

        public override void TryToCompleteActivity()
        {
            bool flag = base.localUser.id == LocalUserManager.GetFirstLocalUser().id;
            if (shouldGrant && flag)
            {
                BaseActivitySelector baseActivitySelector = new BaseActivitySelector();
                baseActivitySelector.activityAchievementID = "EnemiesReturnsEscapedIntoThePast";
                PlatformSystems.activityManager.TryToCompleteActivity(baseActivitySelector);
            }
        }
    }
}
