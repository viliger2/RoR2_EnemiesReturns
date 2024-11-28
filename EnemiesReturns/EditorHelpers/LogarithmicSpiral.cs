using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class LogarithmicSpiral : MonoBehaviour
    {
        public Transform test;

        public float a = 0.05f;

        public float b = 0.05f;

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;
            var angle = Mathf.PI * timer;
            var r = a * Mathf.Pow((float)System.Math.E, b * angle);

            test.transform.localPosition = new Vector3(r * Mathf.Cos(angle), 0, r * Mathf.Sin(angle));
        }

    }
}
