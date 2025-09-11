using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class ComponentEnabler : MonoBehaviour
    {
        public MonoBehaviour component;

        public void EnableComponent()
        {
            component.enabled = true;
        }

        public void DisableComponent()
        {
            component.enabled = false;
        }
    }
}
