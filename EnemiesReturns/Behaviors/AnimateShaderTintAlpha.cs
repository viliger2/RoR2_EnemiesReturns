using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

namespace EnemiesReturns.Behaviors
{
    public class AnimateShaderTintAlpha : MonoBehaviour
    {
        public AnimationCurve alphaCurve;

        private Renderer targetRenderer;

        private MaterialPropertyBlock _propBlock;

        public float timeMax = 5f;

        public bool pauseTime;

        public bool destroyOnEnd;

        public bool disableOnEnd;

        public bool disableObjectOnEnd;

        public bool resetOnEnable;

        [HideInInspector]
        public float time;

        [HideInInspector]
        public bool initialyEnabled;

        [HideInInspector]
        public UnityEvent OnFinished;

        public bool continueExistingAfterTimeMaxIsReached;

        private bool initialised;

        private Color initialColor;

        private static int shaderKey = -1;

        private void Start()
        {
            Initialise();
        }

        public void Restart()
        {
            time = 0f;
            base.enabled = true;
        }

        private void OnEnable()
        {
            if (resetOnEnable)
            {
                time = 0f;
            }
        }

        private void Initialise()
        {
            if (!initialised)
            {
                if (shaderKey == -1)
                {
                    shaderKey = Shader.PropertyToID("_TintColor");
                }
                targetRenderer = GetComponent<Renderer>();
                if ((bool)targetRenderer)
                {
                    initialColor = targetRenderer.material.GetColor(shaderKey);
                    _propBlock = new MaterialPropertyBlock();
                    targetRenderer.GetPropertyBlock(_propBlock);
                    _propBlock.SetColor(shaderKey, initialColor);
                    targetRenderer.SetPropertyBlock(_propBlock);
                    initialised = true;
                }

            }
        }

        public void Reset()
        {
            Initialise();
            time = 0f;
            targetRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor(shaderKey, initialColor);
            targetRenderer.SetPropertyBlock(_propBlock);
            base.enabled = initialyEnabled;
        }

        private void OnDestroy()
        {
            OnFinished.RemoveAllListeners();
        }

        private void Update()
        {
            if (!pauseTime)
            {
                time = Mathf.Min(timeMax, time + Time.deltaTime);
            }
            float num = alphaCurve.Evaluate(time / timeMax);

            targetRenderer.GetPropertyBlock(_propBlock);
            var color = _propBlock.GetColor(shaderKey);
            color.a = num;
            _propBlock.SetColor(shaderKey, color);
            targetRenderer.SetPropertyBlock(_propBlock);

            if (time >= timeMax)
            {
                OnFinished?.Invoke();
                if (disableOnEnd)
                {
                    base.enabled = false;
                }
                if (disableObjectOnEnd)
                {
                    base.gameObject.SetActive(value: false);
                }
                if (destroyOnEnd)
                {
                    UnityEngine.Object.Destroy(base.gameObject);
                }
            }
        }
    }
}
