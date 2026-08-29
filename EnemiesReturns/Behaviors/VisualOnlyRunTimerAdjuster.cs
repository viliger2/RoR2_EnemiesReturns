using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    // it won't be networked but who cares honestly
    public class VisualOnlyRunTimerAdjuster : MonoBehaviour
    {
        public float timerOffsetMin;

        public float timerOffsetMax;

        private float timerOffset;

        private void OnEnable()
        {
            RecalculateOffset();

            On.RoR2.UI.RunTimerUIController.Update += RunTimerUIController_Update;
        }

        public void RecalculateOffset()
        {
            timerOffset = UnityEngine.Random.Range(timerOffsetMin, timerOffsetMax);
        }

        private void RunTimerUIController_Update(On.RoR2.UI.RunTimerUIController.orig_Update orig, RoR2.UI.RunTimerUIController self)
        {
            orig(self);
            if (self.runStopwatchTimerTextController)
            {
                self.runStopwatchTimerTextController.seconds = RoR2.Run.instance ? RoR2.Run.instance.GetRunStopwatch() + timerOffset : 0f;
            } else if (self.spriteAsNumberManager)
            {
                self.spriteAsNumberManager.SetTimerValue(RoR2.Run.instance ? (int)(RoR2.Run.instance.GetRunStopwatch() + timerOffset) : 0);
            }

        }

        private void OnDisable()
        {
            On.RoR2.UI.RunTimerUIController.Update -= RunTimerUIController_Update;
        }
    }
}
