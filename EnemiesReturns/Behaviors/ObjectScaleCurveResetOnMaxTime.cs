using RoR2;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(ObjectScaleCurve))]
    public class ObjectScaleCurveResetOnMaxTime : MonoBehaviour
    {
        public ObjectScaleCurve curve;

        private void Awake()
        {
            curve.maxTimeReachedDelegate += curve.Reset;
        }
    }
}
