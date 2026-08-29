using UnityEngine;

namespace EnemiesReturns.Behaviors.ContactLight
{
    public class SceneSpecificHooks : MonoBehaviour
    {
        private void OnEnable()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate += HUDBossHealthBarController_LateUpdate;
            On.RoR2.UI.RunTimerUIController.Start += RunTimerUIController_Start;
        }

        private void RunTimerUIController_Start(On.RoR2.UI.RunTimerUIController.orig_Start orig, RoR2.UI.RunTimerUIController self)
        {
            orig(self);
            var component = self.gameObject.AddComponent<VisualOnlyRunTimerAdjuster>();
            component.timerOffsetMin = -70000f;
            component.timerOffsetMax = -65000f;
            component.RecalculateOffset();
        }

        private void HUDBossHealthBarController_LateUpdate(On.RoR2.UI.HUDBossHealthBarController.orig_LateUpdate orig, RoR2.UI.HUDBossHealthBarController self)
        {
            orig(self);
            //BossGroupHealthColorOverride.ReplaceColor(self);
            BossGroupTextOverride.ReplaceNames(self);
        }

        private void OnDisable()
        {
            On.RoR2.UI.HUDBossHealthBarController.LateUpdate -= HUDBossHealthBarController_LateUpdate;
            On.RoR2.UI.RunTimerUIController.Start -= RunTimerUIController_Start;
        }
    }
}
