using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;
using static EnemiesReturns.Achievements.EscapedIntoThePastAchievement;

namespace EnemiesReturns.Achievements
{
    // TODO: remove 1 with full release
    [RegisterAchievement("EnemiesReturnsSlainProvidence", "Items.AdrenalineCore.0", null, 5u, typeof(SlainProvidenceServerAchievement))]
    public class SlainProvidenceAchievement : BaseAchievement
    {
        public class SlainProvidenceServerAchievement : BaseServerAchievement
        {

        }
    }
}
