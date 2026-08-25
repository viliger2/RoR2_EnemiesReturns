using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Behaviors
{
    using UnityEngine;

    public class BobbingObject : MonoBehaviour
    {
        [Header("Bobbing Settings")]
        [Tooltip("How far the object bobs in each axis")]
        public Vector3 bobAmount = new Vector3(0f, 0.5f, 0f);

        [Tooltip("How fast the object bobs")]
        public float bobSpeed = 2f;

        [Tooltip("Time offset to desynchronize multiple bobbing objects")]
        public float timeOffset = 0f;

        [Header("Optional Settings")]
        [Tooltip("If true, bobbing will use unscaled time (ignores Time.timeScale)")]
        public bool useUnscaledTime = false;

        [Tooltip("If true, the object will bob around its starting position")]
        public bool bobAroundStartPosition = true;

        private Vector3 startPosition;
        private float currentTime;

        void Start()
        {
            // Store the starting position
            startPosition = transform.position;
            currentTime = timeOffset;
        }

        void Update()
        {
            // Update time based on whether we're using scaled or unscaled time
            if (useUnscaledTime)
            {
                currentTime += Time.unscaledDeltaTime * bobSpeed;
            }
            else
            {
                currentTime += Time.deltaTime * bobSpeed;
            }

            // Calculate the bob offset using sine curve
            float sineValue = Mathf.Sin(currentTime);
            Vector3 bobOffset = bobAmount * sineValue;

            // Apply the bob offset
            if (bobAroundStartPosition)
            {
                transform.position = startPosition + bobOffset;
            }
            else
            {
                // If not bobbing around start position, just add the offset to current position
                transform.position += bobOffset;
            }
        }

        // Optional: Method to reset the bob to its starting position
        public void ResetBob()
        {
            currentTime = timeOffset;
            if (bobAroundStartPosition)
            {
                transform.position = startPosition;
            }
        }

        // Optional: Method to set a new start position
        public void SetStartPosition(Vector3 newStartPosition)
        {
            startPosition = newStartPosition;
            if (bobAroundStartPosition)
            {
                transform.position = startPosition;
            }
        }

        // Optional: Method to update the start position to the current position
        public void UpdateStartPositionToCurrent()
        {
            startPosition = transform.position;
        }

        // Optional: Draw gizmos to visualize the bob range in the editor
        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying)
            {
                startPosition = transform.position;
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startPosition - bobAmount, startPosition + bobAmount);
            Gizmos.DrawWireSphere(startPosition + bobAmount, 0.1f);
            Gizmos.DrawWireSphere(startPosition - bobAmount, 0.1f);
        }
    }
}
