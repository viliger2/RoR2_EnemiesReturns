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
            Vector3 rhs = Vector3.Cross(Vector3.up, lookVector.localPosition);
            float x = Vector3.Dot(moveVector.localPosition, lookVector.localPosition);
            float y = Vector3.Dot(moveVector.localPosition, rhs);
            Vector2 to = new Vector2(x, y);

            // var moveVector2 = new Vector2(moveVector.position.x, moveVector.position.z);
            // var lookVector2 = new Vector2(lookVector.position.x, lookVector.position.z);

            // var angle = Vector2.SignedAngle(moveVector2, lookVector2);
            // Debug.Log(angle);
            testCube.localPosition = new Vector3(Mathf.Cos(to.x * (MathF.PI / 180f)), testCube.localPosition.y, Mathf.Sin(to.y * (MathF.PI / 180f)));
        }
    }
}
