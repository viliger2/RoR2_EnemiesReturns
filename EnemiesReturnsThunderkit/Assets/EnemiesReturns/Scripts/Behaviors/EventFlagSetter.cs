using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class EventFlagSetter : MonoBehaviour
    {
        public string[] eventFlags;

        public bool runOnEnable;

        public void OnEnable()
        {
            if (runOnEnable)
            {
                SetRunFlags();
            }
        }

        public void SetRunFlags()
        {
            if (!RoR2.Run.instance)
            {
                return;
            }

            foreach(var flag in eventFlags)
            {
                RoR2.Run.instance.SetEventFlag(flag);
            }
        }
    }
}
