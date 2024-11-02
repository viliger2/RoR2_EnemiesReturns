using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class MoreGeometry : MonoBehaviour
    {
        public Transform moveVector;

        public Transform lookVector;

        public Vector3 axis;

        public Transform testCube;

        private void FixedUpdate()
        {
            var angle = Vector3.SignedAngle(lookVector.position, moveVector.position, axis);
            testCube.localPosition = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * 3, testCube.localPosition.y, Mathf.Sin(angle * Mathf.Deg2Rad));
        }
    }
}
