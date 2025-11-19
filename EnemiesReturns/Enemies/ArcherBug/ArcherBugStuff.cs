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
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Croco.CrocoSpit_prefab).WaitForCompletion().InstantiateClone("ArcherBugCausticSpitProjectile", true);
            var impactExplosion = clonedProjectile.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.blastRadius = Configuration.ArcherBug.CausticSpitBlastRadius.Value;
            impactExplosion.blastProcCoefficient = Configuration.ArcherBug.CausticSpitProcCoefficient.Value;

            ProjectileDamage projectileDamage = clonedProjectile.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageTypeCombo.GenericPrimary;

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_BeetleQueen.BeetleQueenSpitGhost_prefab).WaitForCompletion();

            return clonedProjectile;
        }
        public GameObject CreateDeathEffect()
        {
            var deathEffectPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC1_AcidLarva.AcidLarvaDeath_prefab).WaitForCompletion().InstantiateClone("ArcherBugDeathEffect", false);
            var effectComponent = deathEffectPrefab.GetComponent<EffectComponent>();
            effectComponent.applyScale = true;
            effectComponent.soundName = "ER_ArcherBug_Death_Play";

            foreach (var system in deathEffectPrefab.GetComponentsInChildren<ParticleSystem>())
            {
                var main = system.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            return deathEffectPrefab;
        }

        public GameObject CreateCausticSpitChargeEffect()
        {
            var clonedPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_BeetleQueen.BeetleQueenSpitGhost_prefab).WaitForCompletion().InstantiateClone("ArcherBugCausticSpitChargeEffect", false);
            UnityEngine.Object.DestroyImmediate(clonedPrefab.GetComponent<ProjectileGhostController>());

            var effectComponent = clonedPrefab.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.parentToReferencedTransform = true;

            clonedPrefab.AddComponent<DestroyOnTimer>().duration = 0.5f;

            return clonedPrefab;
        }
    }
}
