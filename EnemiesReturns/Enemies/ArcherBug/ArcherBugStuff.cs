using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.ArcherBug
{
    public class ArcherBugStuff
    {
        public GameObject CreateCausticSpitProjectile()
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Croco.CrocoSpit_prefab).WaitForCompletion().InstantiateClone("ArcherBugCausticSpitProjectile", true);
            clonedProjectile.GetComponent<ProjectileImpactExplosion>().blastRadius = 2f;

            ProjectileDamage projectileDamage = clonedProjectile.GetComponent<ProjectileDamage>();
            projectileDamage.damageType.damageSource = DamageSource.Primary;
            projectileDamage.damageType.damageType = DamageTypeCombo.GenericPrimary;

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_BeetleQueen.BeetleQueenSpitGhost_prefab).WaitForCompletion();
            
            return clonedProjectile;
        }
       
        public GameObject CreateDeathEffect()
        {
            var deathEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.RoR2_DLC1_AcidLarva.AcidLarvaDeath_prefab).WaitForCompletion().InstantiateClone("ArcherBugDeathEffect", false);
            deathEffectPrefab.GetComponent<EffectComponent>().applyScale = true;

            foreach (var system in deathEffectPrefab.GetComponentsInChildren<ParticleSystem>())
            {
                var main = system.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            return deathEffectPrefab;
        }
    }
}
