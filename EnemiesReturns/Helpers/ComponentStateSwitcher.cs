using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class ComponentStateSwitcher : MonoBehaviour
    {
        public MonoBehaviour component;

        public float delay;

        public bool state;

        private float timer;

        private void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if(timer > delay)
            {
                if(component)
                {
                    component.enabled = state;
                }
            }
        }

    }
}
