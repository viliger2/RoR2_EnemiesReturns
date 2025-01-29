using RoR2;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class FlickerEmission : MonoBehaviour
    {
        public Renderer renderer;

        public Wave[] sinWaves;

        public string soundName;

        public float soundRepeatThreshold = 0.25f; // controls how frequently the sound can play

        public float soundEmissionValue = 6.5f;

        private float initialEmissionPower;

        private float workingEmissionPower;

        private float stopwatch;

        private float soundTimer;

        private MaterialPropertyBlock propertyBlock;

        private void Start()
        {
            if (renderer)
            {
                propertyBlock = new MaterialPropertyBlock();
                initialEmissionPower = renderer.material.GetFloat("_EmPower");
                workingEmissionPower = initialEmissionPower;
                propertyBlock.SetFloat("_EmPower", initialEmissionPower);
                renderer.SetPropertyBlock(propertyBlock);

                var randomPhase = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
                for (int i = 0; i < sinWaves.Length; i++)
                {
                    sinWaves[i].cycleOffset += randomPhase;
                }
            }
        }

        private void OnEnable()
        {
            workingEmissionPower = initialEmissionPower;
        }

        private void Update()
        {
            stopwatch += Time.deltaTime;
            soundTimer += Time.deltaTime;
            if (renderer)
            {
                float num = workingEmissionPower;
                for (int i = 0; i < sinWaves.Length; i++)
                {
                    num *= 1f + sinWaves[i].Evaluate(stopwatch);
                }
                propertyBlock.SetFloat("_EmPower", num);
                renderer.SetPropertyBlock(propertyBlock);
                if (num > soundEmissionValue && soundRepeatThreshold < soundTimer)
                {
                    Util.PlaySound(soundName, this.gameObject);
                    soundTimer = 0f;
                }
            }
        }
    }
}
