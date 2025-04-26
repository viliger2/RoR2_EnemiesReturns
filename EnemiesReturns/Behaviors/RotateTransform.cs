using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class RotateTransform : MonoBehaviour
    {
        public float spinSpeed = 30f;

        public float bobHeight = 0.1f;

        public float frequency = 1f;

        private Vector3 initialPosition;

        private float timer;

        private void Start()
        {
            initialPosition = transform.position;
        }

        private void Update()
        {
            timer += Time.deltaTime;
            transform.Rotate(new Vector3(0f, spinSpeed * Time.deltaTime, 0f));
            transform.position = initialPosition + new Vector3(0f, Mathf.Sin(frequency * timer) * bobHeight, 0f);
        }

    }
}
