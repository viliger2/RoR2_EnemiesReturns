using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class PatternRenderer : MonoBehaviour
    {
        public enum Type
        {
            Circle,
            Cross,
            Triangle
        }

        public Type type;

        public int count = 8;

        public float radius = 8f;

        public float prefabRadius = 3.5f;

        public float rotation = 45f;

        public float distanceBetweenRows = 0;

        private class Pattern
        {
            public Vector3 center { get; set; }

            public virtual Vector3[] Generate()
            {
                return new Vector3[] { center };
            }
        }

        private class Circle : Pattern
        {
            public int count = 8;

            public float radius = 11.5f;

            public override Vector3[] Generate()
            {
                if (count <= 0) return new Vector3[0];

                Vector3[] points = new Vector3[count];
                float angleStep = 360f / count;

                for (int i = 0; i < count; i++)
                {
                    float angle = (i * angleStep) * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
                    points[i] = center + offset;
                }

                return points;
            }
        }

        private class Cross : Pattern
        {
            public int pointsPerArm = 2;

            public float distance = 8.5f;

            public float rotation = 45f;

            public override Vector3[] Generate()
            {
                if (pointsPerArm < 0) pointsPerArm = 0;

                //float armLength = lineLength * 0.5f;

                Vector3[] points = new Vector3[1 + pointsPerArm * 4];

                int index = 0;
                points[index++] = center;

                Quaternion rot = Quaternion.AngleAxis(rotation, Vector3.up);
                var axisA = rot * Vector3.right;
                var axisB = rot * Vector3.forward;

                for (int i = 1; i <= pointsPerArm; i++)
                {
                    float d = distance * i;

                    //float d = armLength * i / pointsPerArm;

                    points[index++] = center + axisA * d; // right
                    points[index++] = center - axisA * d; // left
                    points[index++] = center + axisB * d; // forward / up
                    points[index++] = center - axisB * d; // back  / down
                }

                return points;
            }
        }

        private class Triangle : Pattern
        {
            public int rowCount;

            public float distanceBetweenRows;

            public float distanceBetweenPoints;

            public float rotation;

            public override Vector3[] Generate()
            {
                if (rowCount <= 0) return new Vector3[0];

                int total = rowCount * (rowCount + 1) / 2;
                Vector3[] points = new Vector3[total];

                // Half the distance (in row units) from the first row to the last.
                float rowCenterOffset = (rowCount - 1) * 0.5f;

                Quaternion rot = Quaternion.AngleAxis(rotation, Vector3.up);
                var axisA = rot * Vector3.right;
                var axisB = rot * Vector3.forward;

                int index = 0;
                for (int r = 0; r < rowCount; r++)
                {
                    int pointsInRow = r + 1;

                    // Row vertical position: symmetric around v = 0.
                    float v = (r - rowCenterOffset) * distanceBetweenRows;

                    // Horizontal offsets: symmetric around u = 0 within the row.
                    float halfSpan = r * 0.5f;

                    for (int i = 0; i < pointsInRow; i++)
                    {
                        float u = (i - halfSpan) * distanceBetweenPoints;
                        points[index++] = center + axisA * u + axisB * v;
                    }
                }

                return points;
            }
        }

        private void OnDrawGizmos()
        {
            Pattern pattern = null;
            if (type == Type.Circle)
            {
                pattern = new Circle()
                {
                    center = transform.position,
                    count = count,
                    radius = radius
                };
            } else if (type == Type.Cross)
            {
                pattern = new Cross()
                {
                    center = transform.position,
                    distance = radius,
                    pointsPerArm = count,
                    rotation = rotation
                };
            } else if (type == Type.Triangle)
            {
                pattern = new Triangle()
                {
                    center = transform.position,
                    distanceBetweenPoints = radius,
                    distanceBetweenRows = distanceBetweenRows,
                    rotation = rotation,
                    rowCount = count
                };
            }

            if (pattern != null)
            {
                var points = pattern.Generate();
                foreach (var point in points)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(point, prefabRadius);
                }
            }
        } 

    }
}
