using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tester2
{
    public class Tester : MonoBehaviour
    {
        public int projectileCount = 4;

        public float angle = 120f;

        public Vector3 aimDirection;

        public Vector3 multiplierVector = Vector3.up;

        private void OnDrawGizmos()
        {
            var angle = this.angle / (projectileCount - 1);

            var normalizerAimDirection = aimDirection.normalized;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, normalizerAimDirection);

            // Gizmos.color = Color.white;
            // rotationHelper.LookAt(whateverThing);
            // var rotationVector = rotationHelper.rotation.eulerAngles.normalized
            var angleFromForward = Vector3.SignedAngle(Vector3.forward, new Vector3(normalizerAimDirection.x, 0f, normalizerAimDirection.z), Vector3.up);
            Debug.Log(angleFromForward);

            Gizmos.color = Color.blue;
            var newRight = Quaternion.AngleAxis(angleFromForward, Vector3.up) * Vector3.right;
            Debug.Log("newRight " + newRight);
            Gizmos.DrawRay(transform.position, newRight);

            Gizmos.color = Color.green;
            //var newVector = Vector3.Cross(Vector3.right, normalizerAimDirection).normalized; 
            var newVector = (Quaternion.AngleAxis(-30, newRight) * normalizerAimDirection).normalized;
            Debug.Log("newVector " + newVector);
            Gizmos.DrawRay(transform.position, newVector);

            Gizmos.color = Color.red;
            var rotatinVector = Vector3.Cross(newRight, newVector);
            Debug.Log("rotatinVEctor " + rotatinVector);
            //var rotatinVector = -newVector;
            Gizmos.DrawRay(transform.position, rotatinVector);

            Gizmos.color = Color.white;
            var startingDirection = Quaternion.AngleAxis(this.angle * 0.5f, rotatinVector) * newVector;
            Debug.Log("startingDirection: " + startingDirection);
            Gizmos.DrawRay(transform.position, startingDirection);
            var rotation = Quaternion.AngleAxis(-angle, rotatinVector);
            Debug.Log("rotation:" + rotation.eulerAngles);
            for (int i = 0; i < projectileCount; i++)
            {
                Debug.Log("i: " + i + ", startindDirection: " + startingDirection);
                Gizmos.DrawCube(transform.position + startingDirection * 5, Vector3.one);
                startingDirection = rotation * startingDirection;
            }
        }
    }
}
