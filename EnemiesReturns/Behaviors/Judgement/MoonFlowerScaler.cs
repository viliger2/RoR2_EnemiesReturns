using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors.Judgement
{
    public class MoonFlowerScaler : MonoBehaviour
    {
        public ParticleSystem flowers;

        public float scale = 1f;

        void Start()
        {
            if (!flowers)
            {
                return;
            }

            var main = flowers.main;
            main.maxParticles = Mathf.Min((int)(main.maxParticles * scale), 30);

            var shape = flowers.shape;
            shape.scale *= scale;

            flowers.gameObject.SetActive(true);
        }
    }
}
