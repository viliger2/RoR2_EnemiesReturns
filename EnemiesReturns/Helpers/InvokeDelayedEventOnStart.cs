using RoR2.EntityLogic;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class InvokeDelayedEventOnStart : MonoBehaviour
    {
        public DelayedEvent delayedEvent;

        public float timer;

        private void OnEnable()
        {
            delayedEvent.CallDelayed(timer);
        }

    }
}
