using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
//using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Behaviors.CutsceneHelpers
{
    public class MusicInjector : MonoBehaviour
    {
        public MusicTrackOverride musicTrackOverride;

        public string addressablePath;

        private void Awake()
        {
            if (musicTrackOverride && !string.IsNullOrEmpty(addressablePath))
            {
                //musicTrackOverride.track = Addressables.LoadAssetAsync<MusicTrackDef>(addressablePath).WaitForCompletion();
            }
        }
    }
}
