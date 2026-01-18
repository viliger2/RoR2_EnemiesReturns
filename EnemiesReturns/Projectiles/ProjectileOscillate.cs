using UnityEngine;

namespace EnemiesReturns.Projectiles
{
    public class ProjectileOscillate : MonoBehaviour
    {
        public bool oscillateX = false;

        public bool oscillateY = false;

        public bool oscillateZ = false;

        public float oscillateMagnitude = 20f;

        public float oscillateSpeed = 1f;

        public bool separateValues = false;

        public float oscillateMagnitudeX = 20f;

        public float oscillateSpeedX = 1f;

        public float oscillateMagnitudeY = 20f;

        public float oscillateSpeedY = 1f;

        public float oscillateMagnitudeZ = 20f;

        public float oscillateSpeedZ = 1f;

        public bool randomStartingOffset = false;

        private Rigidbody rigidBody;

        private float oscillationStopwatch = 0f;

        private void Awake()
        {
            rigidBody = GetComponent<Rigidbody>();
            if (randomStartingOffset)
            {
                oscillationStopwatch += UnityEngine.Random.Range(0, Mathf.PI * 2);
            }
        }

        private void FixedUpdate()
        {
            if (rigidBody)
            {
                rigidBody.velocity += CalculateOscillation();
            }
            oscillationStopwatch += Time.fixedDeltaTime;
        }

        private Vector3 CalculateOscillation()
        {
            Vector3 result = Vector3.zero;

            if (separateValues)
            {
                if (oscillateX)
                {
                    result += Vector3.right * Mathf.Sin(oscillationStopwatch * oscillateSpeedX);
                }
                if (oscillateY)
                {
                    result += Vector3.up * Mathf.Sin(oscillationStopwatch * oscillateSpeedY);
                }
                if (oscillateZ)
                {
                    result += Vector3.forward * Mathf.Sin(oscillationStopwatch * oscillateSpeedZ);
                }
            }
            else
            {
                var delta = Mathf.Sin(oscillationStopwatch * oscillateSpeed);
                if (oscillateX)
                {
                    result += Vector3.right * (delta * oscillateMagnitude);
                }
                if (oscillateY)
                {
                    result += Vector3.up * (delta * oscillateMagnitude);
                }
                if (oscillateZ)
                {
                    result += Vector3.forward * (delta * oscillateMagnitude);
                }
            }

            return result;
        }

    }
}
