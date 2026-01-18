using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class LightRangeScale : MonoBehaviour
    {
        public Light light;

        public AnimationCurve lightRangeCurve;

        public float maxDuration;

        public bool destroyOnEnd;

        private float stopwatch;

        private float initialRange;

        private void Awake()
        {
            if (!light)
            {
                light = GetComponent<Light>();
            }

            if (!light)
            {
                this.enabled = false;
            }

            initialRange = light.range;
        }

        private void OnEnable()
        {
            stopwatch = 0f;
        }

        private void Update()
        {
            stopwatch += Time.deltaTime;
            float num = Mathf.Clamp01(stopwatch / maxDuration);
            light.range = initialRange * lightRangeCurve.Evaluate(num);
            if (num == 1f && destroyOnEnd)
            {
                UnityEngine.Object.Destroy(light.gameObject);
            }
        }


    }
}
