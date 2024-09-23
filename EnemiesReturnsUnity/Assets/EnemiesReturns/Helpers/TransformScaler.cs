using UnityEngine;

namespace EnemiesReturns.Helpers
{
    public class TransformScaler : MonoBehaviour
    {
        public Transform[] transforms;

        public Vector3 defaultValue;

        private Vector3 target;

        private Vector3 from;

        private float duration;

        private float timer;

        private bool resetAfter;

        private bool active;

        private void Update()
        {
            if (active)
            {
                timer += Time.deltaTime;
                Vector3 scale = duration == 0 ? target : Vector3.Lerp(from, target, timer / duration);
                foreach (Transform t in transforms)
                {
                    t.localScale = scale;
                }
                if (timer >= duration)
                {
                    active = false;
                    if (resetAfter)
                    {
                        ResetScale();
                    }
                }
            }
        }

        public void SetScaling(Vector3 target, float time, bool resetAfter = false)
        {
            SetScaling(target, time, defaultValue, resetAfter);
        }

        public void SetScaling(Vector3 target, float time, Vector3 from, bool resetAfter = false)
        {
            this.target = target;
            duration = time;
            this.from = from;
            this.resetAfter = resetAfter;
            timer = 0f;
            active = true;
        }

        public void ResetScale()
        {
            foreach (Transform t in transforms)
            {
                if (t)
                {
                    t.localScale = defaultValue;
                }
            }
        }

    }
}
