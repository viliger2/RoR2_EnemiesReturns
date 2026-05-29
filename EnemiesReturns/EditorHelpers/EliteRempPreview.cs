using EnemiesReturns.Behaviors;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.EditorHelpers
{
    public class EliteRempPreview : MonoBehaviour
    {
        public Texture2D eliteRamp;

        public Renderer renderer;

        private MaterialPropertyBlock propertyBlock;

        public void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        private void FixedUpdate()
        {
            SetProperties();
        }

        private void SetProperties()
        {
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetTexture(SetEliteRampOnShader.EliteRampPropertyID, eliteRamp);
            propertyBlock.SetFloat(CommonShaderProperties._EliteIndex, 1);
            renderer.SetPropertyBlock(propertyBlock);
        }
    }
}
