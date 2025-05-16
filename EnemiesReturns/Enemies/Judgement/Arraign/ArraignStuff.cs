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

    }
}
