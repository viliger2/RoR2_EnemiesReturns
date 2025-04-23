using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArraignSkinExample
{
    // Due to fact that we want our skin to be locked behind achievement only if EnemiesReturns is loaded
    // we can't really use RegisterAchievement attribute to automate achievement registration
    // however if you are fine with either being hard dependant on EnemiesReturns or willing
    // to write checks manually instead of just inheriting from EnemiesReturns.Achievements.JudgementClearedAchievement
    // then you can uncomment the following line, renaming ArraignSkinExample to something unique
    // and then comment RoR2.AchievementManager.onAchievementsRegistered in ArraignSkinExample
    //[RegisterAchievement("ArraignSkinExampleBandit2BodyJudgementCleared", "Skins.Bandit2Body.ArraignSkinExample", null, 10u, typeof(EnemiesReturns.Achievements.JudgementClearedAchievement.JudgementClearedServerAchievement))]
    public class Bandit2JudgementClearedAchievement : EnemiesReturns.Achievements.JudgementClearedAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex(ArraignSkinExample.BodyName);
        }
    }
}
