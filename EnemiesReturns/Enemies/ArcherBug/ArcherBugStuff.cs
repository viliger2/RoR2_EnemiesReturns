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
            clonedProjectile.GetComponent<ProjectileImpactExplosion>().blastRadius = 2f;

            ProjectileDamage projectileDamage = clonedProjectile.GetComponent<ProjectileDamage>();
            projectileDamage.damageType.damageSource = DamageSource.Primary;
            projectileDamage.damageType.damageType = DamageTypeCombo.GenericPrimary;

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
