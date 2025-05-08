using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.ArcherBug
{
    public class ArcherBugStuff
    {

        public GameObject CreateCausticSpitProjectile()
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoSpit.prefab").WaitForCompletion().InstantiateClone("ArcherBugCausticSpitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 10f;

            if (clonedProjectile.TryGetComponent<ProjectileImpactExplosion>(out var component))
            {
                UnityEngine.Object.DestroyImmediate(component);
            };

            if (clonedProjectile.TryGetComponent<ProjectileSingleTargetImpact>(out var component2))
            {
                UnityEngine.Object.DestroyImmediate(component2);
            }

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = GetCausticProjectileGhost();
            
            return clonedProjectile;
        }

        private GameObject GetCausticProjectileGhost()
        {
            var projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpitGhost.prefab").WaitForCompletion().InstantiateClone("ArcherBugCausticSpitProjectileGhost", false);
  
            return projectileGhost;

        }
    }
}
