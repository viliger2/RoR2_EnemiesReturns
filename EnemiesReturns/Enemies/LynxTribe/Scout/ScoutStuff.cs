using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Scout
{
    public class ScoutStuff
    {
        public GameObject CreateDoubleSlashClawEffect(GameObject prefab)
        {
            var effectComponent = prefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true;

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;

            prefab.AddComponent<DestroyOnParticleEnd>();

            prefab.GetComponentInChildren<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matLynxScoutDoubleSlashClawEffect", CreateDoubleSlashClawMaterial);

            return prefab;
        }

        public GameObject CreateDoubleSlashLeftHandSwingTrail()
        {
            var effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion().InstantiateClone("LynxScoutLeftSwingTrail", false);

            var particleSystem = effect.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.startSize3D = false;
            main.startSize = 1f;
            main.startRotation3D = false;

            var colorOverLifetime = particleSystem.colorOverLifetime;

            var gradient = new Gradient();
            gradient.mode = GradientMode.Blend;
            gradient.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 255f, time = 0f },
                new GradientAlphaKey{alpha = 21f, time = 1f}
            };
            gradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(0.0680f, 0.5725f, 0.0352f), time = 0f},
                new GradientColorKey{color = new Color(0.0680f, 0.5725f, 0.0352f), time = 1f},
            };

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            effect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            return effect;
        }

        // I love doing this stupid shit because effect system doesn't allow for scaling per axis
        public GameObject CreateDoubleSlashRightHandSwingTrail()
        {
            var effect = CreateDoubleSlashLeftHandSwingTrail();
            effect.name = "LynxScoutRightSwingTrail";

            var swingTrail = effect.transform.Find("SwingTrail");
            swingTrail.localScale = new Vector3(1f, 0.25f, 1f);
            swingTrail.rotation = new Quaternion(0.535170257f, -0.455337405f, -0.461647362f, -0.541426361f);

            effect.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);

            return effect;
        }

        public Material CreateDoubleSlashClawMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matGhostEffect.mat").WaitForCompletion());
            material.name = "matLynxScoutDoubleSlashClawEffect";

            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampVermin.png").WaitForCompletion());

            return material;
        }

    }
}
