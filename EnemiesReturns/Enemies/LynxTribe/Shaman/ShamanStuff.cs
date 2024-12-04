using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman
{
    public class ShamanStuff
    {
        public GameObject CreateSummonStormParticles(GameObject prefab)
        {
            prefab.GetComponentInChildren<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Croco/matCrocoDiseaseTrail.mat").WaitForCompletion();
            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/PassiveHealing/matWoodSpriteFlare.mat").WaitForCompletion();
            return prefab;
        }

        public GameObject SetupShamanMaskMaterials(GameObject prefab)
        {
            prefab.GetComponentInChildren<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGhostEffect.mat").WaitForCompletion();
            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGhostParticleReplacement.mat").WaitForCompletion();
            return prefab;
        }
    }
}
