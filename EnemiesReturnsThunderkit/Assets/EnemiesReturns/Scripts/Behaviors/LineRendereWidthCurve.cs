using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendereWidthCurve : MonoBehaviour
    {
        public AnimationCurve widthCurve;

        public float time;

        public bool useFixedTime;

        public bool resetOnEnable;

        private LineRenderer lineRenderer;

        private float stopwatch;

        private float origMultiplier;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            origMultiplier = lineRenderer.widthMultiplier;
        }

        private void OnEnable()
        {
            if (resetOnEnable)
            {
                lineRenderer.widthMultiplier = origMultiplier;
                stopwatch = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (!useFixedTime)
            {
                return;
            }

            if(stopwatch > time)
            {
                return;
            }

            stopwatch += Time.fixedDeltaTime;
            ScaleWidth();
        }

        private void Update()
        {
            if (useFixedTime)
            {
                return;
            }

            if(stopwatch > time)
            {
                return;
            }

            stopwatch += Time.deltaTime;
            ScaleWidth();
        }

        private void ScaleWidth()
        {
            lineRenderer.widthMultiplier = origMultiplier * widthCurve.Evaluate(stopwatch / time);
        }


    }
}
