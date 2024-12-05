using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class FlickerEmission : MonoBehaviour
    {
        public Renderer renderer;

        public Wave[] sinWaves;

        private float initialEmissionPower;

        private float workingEmissionPower;

        private float stopwatch;

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
                for(int i = 0; i < sinWaves.Length; i++)
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
            float num = workingEmissionPower;
            for (int i = 0; i < sinWaves.Length; i++)
            {
                num *= 1f + sinWaves[i].Evaluate(stopwatch);
            }
            propertyBlock.SetFloat("_EmPower", num);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
