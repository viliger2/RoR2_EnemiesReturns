using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Achievements
{
    // TODO: remove 1 with full release
    [RegisterAchievement("EnemiesReturnsEscapedIntoThePast", "Interactables.ER_Wardrobe.0", null, 10u, typeof(EscapedIntoThePastServerAchievement))]
    public class EscapedIntoThePastAchievement : BaseAchievement
    {
        public class EscapedIntoThePastServerAchievement : BaseServerAchievement
        {

        }
    }
}
