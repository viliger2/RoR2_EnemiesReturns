using System;
using TMPro;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class CustomCostHologramContent : MonoBehaviour
    {
        public float displayValue;

        public TextMeshPro targetTextMesh;

        private float oldDisplayValue;

        private void FixedUpdate()
        {
            if (targetTextMesh && (displayValue != oldDisplayValue))
            {
                oldDisplayValue = displayValue;
                var duration = TimeSpan.FromSeconds(displayValue);
                targetTextMesh.SetText(duration.ToString(@"mm\:ss\.ff"));
                targetTextMesh.color = Color.white;
            }
        }
    }
}
