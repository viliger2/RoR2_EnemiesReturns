using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Helpers
{
    public class SetEliteRampOnShader : NetworkBehaviour
    {
        [SyncVar(hook = "OnChangeEliteRamp")]
        public int m_eliteRampIndex;

        public Renderer[] renderers;

        private MaterialPropertyBlock propertyStorage;

        private void Awake()
        {
            propertyStorage = new MaterialPropertyBlock();
            if (renderers == null)
            {
                renderers = GetComponentsInChildren<Renderer>();
            }
        }

        public void SetEliteRampIndex(int eliteRampIndex)
        {
            m_eliteRampIndex = eliteRampIndex;
            SetEliteRamp(eliteRampIndex);
        }

        public void OnChangeEliteRamp(int eliteRamp)
        {
            SetEliteRamp(eliteRamp);
        }

        private void SetEliteRamp(int eliteRamp)
        {
            foreach (var renderer in renderers)
            {
                renderer.GetPropertyBlock(propertyStorage);
                propertyStorage.SetFloat(CommonShaderProperties._EliteIndex, eliteRamp + 1);
                renderer.SetPropertyBlock(propertyStorage);
            }
        }
    }
}
