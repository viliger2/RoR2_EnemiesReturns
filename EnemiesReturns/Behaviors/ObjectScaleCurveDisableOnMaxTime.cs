using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(ObjectScaleCurve))]
    public class ObjectScaleCurveDisableOnMaxTime : MonoBehaviour
    {
        public ObjectScaleCurve curve;

        private void Awake()
        {
            curve.maxTimeReachedDelegate += Disable;
        }

        private void Disable()
        {
            curve.enabled = false;
        }
    }
}
