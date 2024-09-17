using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class RememberingGeometry : MonoBehaviour
    {
        public float largeRadius = 9f;

        public int numberOfRows = 3;

        private void Awake()
        {
            var smallRadius = (largeRadius / ((numberOfRows - 1) + 0.5f));
            var mainSphere = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mainSphere.transform.parent = gameObject.transform;
            mainSphere.transform.localScale = new Vector3(largeRadius * 2, 0.1f, largeRadius * 2);
            mainSphere.transform.localPosition = Vector3.zero;
            for(int i = 0; i< numberOfRows; i++)
            {
                float fromCentre = smallRadius * i;
                if(i == 0)
                {
                    var smallSphere = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    smallSphere.transform.parent = gameObject.transform;
                    smallSphere.transform.localScale = new Vector3(smallRadius, smallRadius, smallRadius);
                    smallSphere.transform.localPosition = Vector3.zero;
                } else
                {
                    var angleCos = (Mathf.Pow(fromCentre, 2f) + Mathf.Pow(fromCentre, 2f) - Mathf.Pow(smallRadius, 2f)) / (2 * fromCentre * fromCentre);
                    var angle = Mathf.Acos(angleCos) / (MathF.PI / 180);
                    int rockCount = (int)(360f / angle);
                    float newAngle = 360f / rockCount;
                    Debug.Log("angle: " + angle.ToString() + ", rockCount: " + rockCount.ToString() + ", newAngle: " + newAngle.ToString());
                    for(int k = 0; k < rockCount; k++)
                    {
                        var x = fromCentre * Mathf.Cos(newAngle * k * Mathf.Deg2Rad);
                        var z = fromCentre * Mathf.Sin(newAngle * k * Mathf.Deg2Rad);

                        var smallSphere = UnityEngine.GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        smallSphere.transform.parent = gameObject.transform;
                        smallSphere.transform.localPosition = new Vector3(x, 0f, z);
                        smallSphere.transform.localScale = new Vector3(smallRadius, smallRadius, smallRadius);
                    }
                }

            }
        }

    }
}
