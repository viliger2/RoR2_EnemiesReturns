using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Rendering;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    internal class MyAmbientLight : MonoBehaviour
    {
        public bool setSkyboxMaterial;

        public bool setAmbientLightColor;

        public Material skyboxMaterial;

        public AmbientMode ambientMode;

        public float ambientIntensity;

        [ColorUsage(true, true)]
        public Color ambientSkyColor = Color.black;

        [ColorUsage(true, true)]
        public Color ambientEquatorColor = Color.black;

        [ColorUsage(true, true)]
        public Color ambientGroundColor = Color.black;
        private void ApplyLighting()
        {
            if (setAmbientLightColor)
            {
                RenderSettings.ambientMode = ambientMode;
                RenderSettings.ambientIntensity = ambientIntensity;
                RenderSettings.ambientSkyColor = ambientSkyColor * ambientIntensity;
                RenderSettings.ambientEquatorColor = ambientEquatorColor * ambientIntensity;
                RenderSettings.ambientGroundColor = ambientGroundColor * ambientIntensity;
            }
            if (setSkyboxMaterial)
            {
                RenderSettings.skybox = skyboxMaterial;
            }
        }
    }
}
