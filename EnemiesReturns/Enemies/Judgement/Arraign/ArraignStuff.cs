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

    }
}
