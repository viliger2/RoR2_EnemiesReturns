using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static RoR2.BossGroup;

namespace EnemiesReturns.Behaviors.Judgement
{
    public class SceneSpecificHooks : MonoBehaviour
    {
        private void Start()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
        }

        private void HUDBossHealthBarController_LateUpdate(On.RoR2.UI.HUDBossHealthBarController.orig_LateUpdate orig, RoR2.UI.HUDBossHealthBarController self)
        {
            orig(self);
            BossGroupHealthColorOverride.ReplaceColor(self);
            BossGroupTextOverride.ReplaceNames(self);
        }

        private void OnDestroy()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate -= HUDBossHealthBarController_LateUpdate;
        }

    }
}
