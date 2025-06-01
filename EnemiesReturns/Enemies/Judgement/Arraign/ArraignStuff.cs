using R2API;
using RoR2;
using RoR2.Audio;
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

        public GameObject SetupWaveProjectile(GameObject prefab)
        {
            var projectileController = prefab.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWaveGhost.prefab").WaitForCompletion();
            projectileController.flightSoundLoop = Addressables.LoadAssetAsync<LoopSoundDef>("RoR2/Base/Brother/lsdBrotherShockwave.asset").WaitForCompletion();

            prefab.GetComponent<ProjectileOverlapAttack>().impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFX.prefab").WaitForCompletion();
            return prefab;
        }

        public GameObject CreateArmorBreakEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/BrittleDeath.prefab").WaitForCompletion().InstantiateClone("ArraignArmorBreakEffect", false);
            
            // TODO: replace sound effect
            prefab.GetComponent<EffectComponent>().applyScale = true;

            return prefab;
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

            //UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Fire").gameObject);
            //UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Decal").gameObject);
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

        public GameObject SetupPreBeamIndicatorEffect(GameObject prefab)
        {
            prefab.transform.Find("TeamAreaIndicator").GetComponent<Renderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matTeamAreaIndicatorIntersectionMonster.mat").WaitForCompletion();
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

        public GameObject CreateBeamEffect()
        {
            var prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabSpinBeamVFX.prefab").WaitForCompletion().InstantiateClone("ArraignBeamEffect", false);

            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("Billboards").gameObject);
            UnityEngine.Object.DestroyImmediate(prefab.transform.Find("SwirlyTrails").gameObject);

            var firstBeamTransform = prefab.transform.Find("Mesh, Additive");
            firstBeamTransform.localScale = new Vector3(1f, 10f, 1f);
            firstBeamTransform.GetComponent<MeshRenderer>().material = ContentProvider.GetOrCreateMaterial("matArraignBeam1", CreateBeam1Material);

            var secondBeamTransform = prefab.transform.Find("Mesh, Additive/Mesh, Transparent");
            secondBeamTransform.localScale = new Vector3(1.5f, 0.1f, 1f);
            secondBeamTransform.GetComponent<MeshRenderer>().material = ContentProvider.GetOrCreateMaterial("matArraignBeam2", CreateBeam2Material);

            var glows = prefab.transform.Find("Glows");
            var main = glows.GetComponent<ParticleSystem>().main;
            main.startColor = new Color(0f, 0.6754f, 1f, 0.0588f);

            var lightMiddle = prefab.transform.Find("Point Light, Middle").GetComponent<Light>();
            lightMiddle.color = new Color(0.3254f, 0.6999f, 0.9137f, 1f);
            lightMiddle.intensity = 8f;
            lightMiddle.range = 10f;
            prefab.transform.Find("Point Light, End").GetComponent<Light>().color = new Color(0.3254f, 0.6227f, 0.9137f, 1f);

            var muzzleRay = prefab.transform.Find("MuzzleRayParticles");
            muzzleRay.localScale = new Vector3(1f, 1f, 1f);
            muzzleRay.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matArraignBeamMuzzleRay", CreateBeamMuzzleRayMaterial);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "AreaIndicator";
            if(cube.TryGetComponent<Collider>(out var collider))
            {
                UnityEngine.Object.Destroy(collider);
            }

            cube.transform.parent = prefab.transform;
            cube.transform.localScale = new Vector3(2.3f, 1f, 20f);
            cube.transform.localPosition = new Vector3(0f, -0.5f, 9.6f);

            cube.GetComponent<Renderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matTeamAreaIndicatorIntersectionMonster.mat").WaitForCompletion();

            return prefab;
        }

        public Material CreateBeamMuzzleRayMaterial()
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamBillboard2.mat").WaitForCompletion());
            newMaterial.name = "matArraignBeamMuzzleRay";

            newMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampHuntressSoft2.png").WaitForCompletion());
            newMaterial.SetFloat("_AlphaBias", 0.294f);

            return newMaterial;
        }

        public Material CreateBeam1Material()
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamCylinder2.mat").WaitForCompletion());
            newMaterial.name = "matArraignBeam1";
            newMaterial.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));

            newMaterial.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudDifferenceBW2.png").WaitForCompletion());
            newMaterial.SetTextureScale("_MainTex", new Vector2(5f, 5f));

            newMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLightning2.png").WaitForCompletion());

            newMaterial.SetFloat("_Boost", 1.493053f);
            newMaterial.SetFloat("_AlphaBoost", 0.5258f);

            newMaterial.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudCrackedIceInverted.png").WaitForCompletion());
            newMaterial.SetTexture("_Cloud2Tex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/VFX/ParticleMasks/texAlphaGradient5Mask.png").WaitForCompletion());

            newMaterial.SetVector("_CutoffScroll", new Vector4(0f, 44f, 0f, -444f));

            return newMaterial;
        }

        public Material CreateBeam2Material()
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamCylinder1.mat").WaitForCompletion());
            newMaterial.name = "matArraignBeam2";

            newMaterial.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudDifferenceBW2.png").WaitForCompletion());
            newMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampLightning.png").WaitForCompletion());

            newMaterial.SetFloat("_InvFade", 0.318f);
            newMaterial.SetFloat("_Boost", 6.35f);
            newMaterial.SetFloat("_AlphaBoost", 0.57f);
            newMaterial.SetFloat("_AlphaBias", 0.298f);

            newMaterial.SetInt("_CloudOffsetOn", 0);

            newMaterial.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudIce.png").WaitForCompletion());
            newMaterial.SetTextureScale("_Cloud1Tex", Vector2.one);

            newMaterial.SetVector("_CutoffScroll", new Vector4(0f, 40f, 0f, -44f));

            return newMaterial;
        }
    }
}
