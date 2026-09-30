using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class PositionTransformsViaRaycast : MonoBehaviour
    {
        [Serializable]
        public struct TransformInfo
        {
            public Transform transform;
            public Transform origin;
            public Vector3 direction;
            public LayerMask mask;
        }

        public TransformInfo[] transforms;

        private void LateUpdate()
        {
            foreach(TransformInfo t in transforms)
            {
                if(Physics.Raycast(t.origin.position, t.origin.TransformDirection(t.direction.normalized), out var hit, float.PositiveInfinity, t.mask))
                {
                    t.transform.position = hit.point;
                }
            }
        }
    }
}
