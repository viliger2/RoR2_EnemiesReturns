using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Judgement
{
    public class SceneSpecificHooks : MonoBehaviour
    {
        private void Start()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += BossGroupHealthColorOverride.HUDBossHealthBarController_LateUpdate;
            On.RoR2.BossGroup.UpdateObservations += BossGroupTextOverride.BossGroup_UpdateObservations;
        }

        private void OnDestroy()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate -= BossGroupHealthColorOverride.HUDBossHealthBarController_LateUpdate;
            On.RoR2.BossGroup.UpdateObservations -= BossGroupTextOverride.BossGroup_UpdateObservations;
        }

    }
}
