using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
//using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class FontInjector : MonoBehaviour
    {
        public TMP_Text textMeshPro;

        public string fontPath;

        private void Awake()
        {
            if (textMeshPro && !string.IsNullOrEmpty(fontPath))
            {
                //textMeshPro.font = Addressables.LoadAssetAsync<TMP_FontAsset>(fontPath).WaitForCompletion();
            }
        }
    }
}
