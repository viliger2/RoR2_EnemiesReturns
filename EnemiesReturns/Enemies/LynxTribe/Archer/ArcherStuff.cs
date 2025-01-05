using EnemiesReturns.ModdedEntityStates.LynxTribe.Archer;
using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Archer
{
    public class ArcherStuff
    {
        public Material CreateArcherArrowMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressArrow.mat").WaitForCompletion());
            material.name = "matLynxArcherArrow";

            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampHealingVariant.png").WaitForCompletion());

            return material;
        }

        public GameObject CreateArrowProjectileGhost(GameObject prefab2, Material material)
        {
            var prefab = prefab2.InstantiateClone("LynxArcherArrowGhost", false);

            prefab.transform.Find("Arrow/ArrowQuads/Quad1").GetComponent<MeshRenderer>().material = material;
            prefab.transform.Find("Arrow/ArrowQuads/Quad2").GetComponent<MeshRenderer>().material = material;

            var ghostController = prefab.AddComponent<ProjectileGhostController>();

            var vfxAttributes = prefab.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Low;

            prefab.AddComponent<ProjectileGhostRotateTowardsMoveDirection>().ghostController = ghostController;

            return prefab;
        }

        public GameObject CreateArrowImpalePrefab(GameObject prefab2, Material material)
        {
            var prefab = prefab2.InstantiateClone("LynxArcherArrowImpale", false);

            prefab.transform.Find("Arrow/ArrowQuads/Quad1").GetComponent<MeshRenderer>().material = material;
            prefab.transform.Find("Arrow/ArrowQuads/Quad2").GetComponent<MeshRenderer>().material = material;

            UnityEngine.Object.DestroyImmediate(prefab.GetComponent<Rigidbody>());

            var destroyOnTimer = prefab.AddComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 2f;

            prefab.transform.localScale *= 0.5f;

            return prefab;
        }

        public GameObject CreateArrowProjectile(GameObject prefab, GameObject ghost, GameObject impalePrefab)
        {
            prefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            var projectileController = prefab.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = ghost;
            //projectileController.flightSoundLoop = loopSound; // TODO
            projectileController.allowPrediction = true;
            projectileController.procCoefficient = EnemiesReturns.Configuration.LynxTribe.LynxArcher.FireArrowProcCoefficient.Value;

            var projectileNetworkTransform = prefab.AddComponent<ProjectileNetworkTransform>();
            projectileNetworkTransform.positionTransmitInterval = 0.03333334f;
            projectileNetworkTransform.interpolationFactor = 1f;
            projectileNetworkTransform.allowClientsideCollision = false;

            var projectileSimple = prefab.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 5f;
            projectileSimple.desiredForwardSpeed = 100f;
            projectileSimple.updateAfterFiring = false;

            var projectileSingleTarget = prefab.AddComponent<ProjectileSingleTargetImpact>();
            projectileSingleTarget.destroyWhenNotAlive = true;
            projectileSingleTarget.destroyOnWorld = true;
            //projectileSingleTarget.impactEffect = impact; // I don't think we need it with impale thing
            //projectileSingleTarget.hitSoundString = "ER_Shaman_Projectile_Impact_Play";

            prefab.AddComponent<ProjectileDamage>();

            var impaleOnEnemy = prefab.AddComponent<ProjectileImpaleOnEnemyFixed>();
            impaleOnEnemy.impalePrefab = impalePrefab;
            impaleOnEnemy.controller = projectileController;

            prefab.RegisterNetworkPrefab();

            return prefab;
        }

    }
}
