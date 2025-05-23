using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace EnemiesReturns.Enemies.Judgement.Arraign
{
    public class ArraignStuff
    {
        public static GameObject P1BodyPrefab;

        public static GameObject P1MasterPrefab;

        public GameObject SetupLightningStrikePrefab(GameObject projectilePrefab)
        {
            var fxTransform = projectilePrefab.transform.Find("TeamIndicator");

            var teamIndicator = UnityEngine.GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, GroundOnly.prefab").WaitForCompletion());
            teamIndicator.transform.parent = fxTransform;
            teamIndicator.transform.localPosition = Vector3.zero;
            teamIndicator.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            teamIndicator.transform.localScale = Vector3.one;
            teamIndicator.GetComponent<TeamAreaIndicator>().teamFilter = projectilePrefab.GetComponent<TeamFilter>();

            var impactExplosion = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.childrenProjectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricOrbProjectile.prefab").WaitForCompletion();

            return projectilePrefab;
        }

        public GameObject CreateArraignSwingEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("ArraigSwordSlashEffect", false);

            prefab.transform.Find("SwingTrail").localScale = new Vector3(3f, 3f, 3f);

            return prefab;
        }

        public GameObject CreateArraignSwingComboEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSwing1.prefab").WaitForCompletion().InstantiateClone("ArraignSwingComboEffect", false);

            return prefab;
        }

        public Material CreatePreSlashWeaponOverlayMaterial()
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/CritOnUse/matFullCrit.mat").WaitForCompletion());
            newMaterial.name = "matArraignPreSlashOverlay";
            newMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f));
            newMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLightning2.png").WaitForCompletion());
            newMaterial.SetFloat("_IntersectionStrength", 0.6701357f);

            return newMaterial;
        }

        public GameObject CreateSlash3ExplosionEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone("ArraignSlash3ExplosionEffect", false);

            prefab.GetComponent<EffectComponent>().applyScale = true;

            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Fire").gameObject);
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Decal").gameObject);
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Spikes, Small").gameObject);

            return prefab;
        }

        public GameObject CreateSkyLeapRemoveSwordEffect(GameObject prefab)
        {
            prefab.transform.Find("Particles/Debris, 3D").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/Props/matDetailRock.mat").WaitForCompletion();

            prefab.transform.Find("Particles/Debris").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Parent/matParentWaxFlecks.mat").WaitForCompletion();

            var dustTransform = prefab.transform.Find("Particles/Dust");
            dustTransform.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLarge.mat").WaitForCompletion();
            var main = dustTransform.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(0.1450f, 0.7114f, 0.8196f, 0.7607f);

            var dust3Dtransform = prefab.transform.Find("Particles/Dust, Directional");
            dust3Dtransform.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();
            main = dust3Dtransform.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(0.1450f, 0.7114f, 0.8196f, 0.7607f);

            var flash = prefab.transform.Find("Particles/Flash");
            flash.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion();
            main = flash.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(0f, 0.0937f, 0.1411f, 1f);

            prefab.transform.Find("Particles/Sparks").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion();

            prefab.transform.Find("Particles/Point Light").GetComponent<Light>().color = new Color(0.1372f, 0.3942f, 1f);

            var novaSphere = prefab.transform.Find("Particles/Nova Sphere");
            novaSphere.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matCryoCanisterSphere.mat").WaitForCompletion();
            main = novaSphere.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(0.2039f, 0.3705f, 0.8784f, 1f);

            prefab.transform.Find("Particles/Flames, Radial").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matArraignSkyLeapRadialFlames", CreateSkyLeapRemoveSwordFlamesRadialMaterial);

            return prefab;
        }

        public Material CreateSkyLeapRemoveSwordFlamesRadialMaterial() 
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Parent/matParentNovaPillar.mat").WaitForCompletion());
            newMaterial.name = "matArraignSkyLeapRadialFlames";
            newMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLunarWardDecal.png").WaitForCompletion());

            return newMaterial;
        }

        public GameObject CreateSkyLeapDropPositionEffect(GameObject prefab)
        {
            var markPoint = prefab.transform.Find("MarkAttachmentPoint");

            var markVisual = CreateSkyLeapMarktempVisualEffect();
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<TemporaryVisualEffect>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<ObjectScaleCurve>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<Rigidbody>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<DestroyOnTimer>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<AkEvent>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<AkGameObj>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<OnEnableEvent>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<OnEnableEvent>());
            UnityEngine.Object.DestroyImmediate(markVisual.GetComponent<EventFunctions>());

            markVisual.transform.SetParent(markPoint.transform, false);
            markVisual.transform.localPosition = Vector3.zero;
            markVisual.transform.localRotation = Quaternion.identity;

            return prefab;
        }

        public GameObject CreateSkyLeapMarktempVisualEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercExposeEffect.prefab").WaitForCompletion().InstantiateClone("ArraignMarkEffect", false);
            prefab.transform.Find("Visual, On/PulseEffect, Ring").GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matArraignMark", CreateSkyLeapMarkMaterial);
            return prefab;
        }

        public Material CreateSkyLeapMarkMaterial()
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matMercExposed.mat").WaitForCompletion());
            newMaterial.name = "matArraignMark";

            newMaterial.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/BossDamageBonus/texBossDamageMask.png").WaitForCompletion());

            return newMaterial;
        }
    }
}
