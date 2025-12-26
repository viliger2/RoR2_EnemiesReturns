using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class PredictorNoWorldHit
    {
        private enum ExtrapolationType
        {
            None,
            Linear,
            Polar
        }

        private Transform bodyTransform;

        private Transform targetTransform;

        private Vector3 targetPosition0;

        private Vector3 targetPosition1;

        private Vector3 targetPosition2;

        private int collectedPositions;

        public bool hasTargetTransform => targetTransform;

        public bool isPredictionReady => collectedPositions > 2;

        public PredictorNoWorldHit(Transform bodyTransform)
        {
            this.bodyTransform = bodyTransform;
        }

        private void PushTargetPosition(Vector3 newTargetPosition)
        {
            targetPosition2 = targetPosition1;
            targetPosition1 = targetPosition0;
            targetPosition0 = newTargetPosition;
            collectedPositions++;
        }

        public void SetTargetTransform(Transform newTargetTransform)
        {
            targetTransform = newTargetTransform;
            targetPosition2 = (targetPosition1 = (targetPosition0 = newTargetTransform.position));
            collectedPositions = 1;
        }

        public Transform GetTargetTransform()
        {
            return targetTransform;
        }

        public void Update()
        {
            if ((bool)targetTransform)
            {
                PushTargetPosition(targetTransform.position);
            }
        }

        public bool GetPredictedTargetPosition(float time, out Vector3 predictedPosition)
        {
            Vector3 vector = targetPosition1 - targetPosition2;
            Vector3 vector2 = targetPosition0 - targetPosition1;
            vector.y = 0f;
            vector2.y = 0f;
            ExtrapolationType extrapolationType;
            if (vector == Vector3.zero || vector2 == Vector3.zero)
            {
                extrapolationType = ExtrapolationType.None;
            }
            else
            {
                Vector3 normalized = vector.normalized;
                Vector3 normalized2 = vector2.normalized;
                extrapolationType = ((Vector3.Dot(normalized, normalized2) > 0.98f) ? ExtrapolationType.Linear : ExtrapolationType.Polar);
            }
            float num = 1f / Time.deltaTime;
            predictedPosition = targetPosition0;
            switch (extrapolationType)
            {
                case ExtrapolationType.Linear:
                    predictedPosition = targetPosition0 + vector2 * (time * num);
                    break;
                case ExtrapolationType.Polar:
                    {
                        Vector3 position = bodyTransform.position;
                        Vector3 vector3 = Util.Vector3XZToVector2XY(targetPosition2 - position);
                        Vector3 vector4 = Util.Vector3XZToVector2XY(targetPosition1 - position);
                        Vector3 vector5 = Util.Vector3XZToVector2XY(targetPosition0 - position);
                        float magnitude = vector3.magnitude;
                        float magnitude2 = vector4.magnitude;
                        float magnitude3 = vector5.magnitude;
                        float num2 = Vector2.SignedAngle(vector3, vector4) * num;
                        float num3 = Vector2.SignedAngle(vector4, vector5) * num;
                        float num4 = (magnitude2 - magnitude) * num;
                        float num5 = (magnitude3 - magnitude2) * num;
                        float num6 = (num2 + num3) * 0.5f;
                        float num7 = (num4 + num5) * 0.5f;
                        float num8 = magnitude3 + num7 * time;
                        if (num8 < 0f)
                        {
                            num8 = 0f;
                        }
                        Vector2 vector6 = Util.RotateVector2(vector5, num6 * time);
                        vector6 *= num8 * magnitude3;
                        predictedPosition = position;
                        predictedPosition.x += vector6.x;
                        predictedPosition.z += vector6.y;
                        break;
                    }
            }
            return true;
        }
    }
}
