using EnemiesReturns.PrefabAPICompat;
using EnemiesReturns.Projectiles;
using RoR2;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.Spitter
{
    public class SpitterStuff
    {

        public GameObject CreateBiteEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion().InstantiateClone("SpitterBiteEffect", false);

            var particleSystem = clonedEffect.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.startRotationX = new ParticleSystem.MinMaxCurve(0f, 0f);
            main.startRotationY = new ParticleSystem.MinMaxCurve(140f, 140f);

            particleSystem.gameObject.transform.localScale = new Vector3(2f, 2f, 2f);

            return clonedEffect;
        }

        public GameObject CreateNormalSpitProjectile()
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab").WaitForCompletion().InstantiateClone("SpitterSimpleSpitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 50f;

            return clonedProjectile;
        }

        public GameObject CreateChargedSpitProjectile(GameObject bigDotZone, GameObject chunk)
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 55f;

            if (clonedProjectile.TryGetComponent<ProjectileImpactExplosion>(out var component))
            {
                UnityEngine.Object.DestroyImmediate(component);
            };

            if (clonedProjectile.TryGetComponent<ProjectileSingleTargetImpact>(out var component2))
            {
                UnityEngine.Object.DestroyImmediate(component2);
            }

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = GetRecoloredSpitProjectileGhost();

            #region MainSpitZone

            var explosion = clonedProjectile.AddComponent<ProjectileImpactExplosionWithChildrenArray>();
            explosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            explosion.blastRadius = 6.5f * EnemiesReturns.Configuration.Spitter.ChargedProjectileLargeDoTZoneScale.Value;
            explosion.blastDamageCoefficient = 1f;
            explosion.blastProcCoefficient = 1f;
            explosion.blastAttackerFiltering = AttackerFiltering.Default;
            explosion.bonusBlastForce = new Vector3(0f, 0f, 0f);
            explosion.canRejectForce = false;
            explosion.projectileHealthComponent = null;
            explosion.explosionEffect = null;

            explosion.fireChildren = true;
            explosion.childrenProjectilePrefab = chunk;
            explosion.childrenCount = 3;
            explosion.childrenDamageCoefficient = 1f;
            explosion.childredMinRollDegrees = 0f;
            explosion.childrenRangeRollDegrees = 360f;
            explosion.childrenMinPitchDegrees = 40f;
            explosion.childrenRangePitchDegrees = 150f;

            explosion.fireDoTZone = true;
            explosion.dotZoneProjectilePrefab = bigDotZone;
            explosion.dotZoneDamageCoefficient = 1f;
            explosion.dotZoneMinRollDegrees = 0f;
            explosion.dotZoneRangeRollDegrees = 0f;
            explosion.dotZoneMinPitchDegrees = 0f;
            explosion.dotZoneRangePitchDegrees = 0f;

            explosion.applyDot = false;

            explosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleSpitExplosion.prefab").WaitForCompletion();
            explosion.lifetimeExpiredSound = null;
            explosion.offsetForLifetimeExpiredSound = 0f;
            explosion.destroyOnEnemy = true;
            explosion.destroyOnWorld = true;
            explosion.impactOnWorld = true;
            explosion.timerAfterImpact = true;
            explosion.lifetime = 10f;
            explosion.lifetimeAfterImpact = 0f;
            explosion.lifetimeRandomOffset = 0f;
            explosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            #endregion

            return clonedProjectile;
        }

        public GameObject CreateChargedSpitSplitProjectile(GameObject smallPool)
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitSplitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 15f;

            clonedProjectile.layer = LayerIndex.debris.intVal; // no collision with entityprecise or itself, but with world 

            if (clonedProjectile.TryGetComponent<ProjectileImpactExplosion>(out var component))
            {
                UnityEngine.Object.DestroyImmediate(component);
            };

            if (clonedProjectile.TryGetComponent<ProjectileSingleTargetImpact>(out var component2))
            {
                UnityEngine.Object.DestroyImmediate(component2);
            }

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = GetRecoloredSpitProjectileGhost();

            var explosion = clonedProjectile.AddComponent<ProjectileImpactExplosion>();
            explosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            explosion.blastRadius = 6.5f * EnemiesReturns.Configuration.Spitter.ChargedProjectileSmallDoTZoneScale.Value;
            explosion.blastDamageCoefficient = 1f;
            explosion.blastProcCoefficient = 1f;
            explosion.blastAttackerFiltering = AttackerFiltering.Default;
            explosion.bonusBlastForce = new Vector3(0f, 0f, 0f);
            explosion.canRejectForce = false;
            explosion.projectileHealthComponent = null;
            explosion.explosionEffect = null;

            explosion.fireChildren = true;
            explosion.childrenProjectilePrefab = smallPool;
            explosion.childrenCount = 1;
            explosion.childrenDamageCoefficient = 1f;
            explosion.minRollDegrees = 0f;
            explosion.rangeRollDegrees = 0;
            explosion.minPitchDegrees = 0;
            explosion.rangePitchDegrees = 0;

            explosion.applyDot = false;

            explosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleSpitExplosion.prefab").WaitForCompletion();
            explosion.lifetimeExpiredSound = null;
            explosion.offsetForLifetimeExpiredSound = 0f;
            explosion.destroyOnEnemy = false;
            explosion.destroyOnWorld = true;
            explosion.impactOnWorld = true;
            explosion.timerAfterImpact = true;
            explosion.lifetime = 10f;
            explosion.lifetimeAfterImpact = 0f;
            explosion.lifetimeRandomOffset = 0f;
            explosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            return clonedProjectile;
        }

        public GameObject CreateChargedSpitDoTZone()
        {
            var child = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitDoTZone", true);
            var dotZone = child.GetComponent<ProjectileDotZone>();
            dotZone.damageCoefficient = EnemiesReturns.Configuration.Spitter.ChargedProjectileLargeDoTZoneDamage.Value;

            var value = EnemiesReturns.Configuration.Spitter.ChargedProjectileLargeDoTZoneScale.Value;

            var projectileDamage = child.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            var decal = child.GetComponentInChildren<Decal>();
            if (decal)
            {
                decal.Material = ContentProvider.GetOrCreateMaterial("matSpitterAcidDecal", SetupDoTZoneDecalMaterial);
            }

            child.transform.localScale = new Vector3(value, value, value);

            return child;
        }

        public GameObject CreatedChargedSpitSmallDoTZone()
        {
            var child = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitSmallDoTZone", true);
            var dotZone = child.GetComponent<ProjectileDotZone>();
            dotZone.damageCoefficient = EnemiesReturns.Configuration.Spitter.ChargedProjectileSmallDoTZoneDamage.Value;

            var value = EnemiesReturns.Configuration.Spitter.ChargedProjectileSmallDoTZoneScale.Value;

            var projectileDamage = child.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;


            var decal = child.GetComponentInChildren<Decal>();
            if (decal)
            {
                decal.Material = ContentProvider.GetOrCreateMaterial("matSpitterAcidDecal", SetupDoTZoneDecalMaterial);
            }

            child.transform.localScale = new Vector3(value, value, value);

            return child;
        }

        private GameObject GetRecoloredSpitProjectileGhost()
        {
            var projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpitGhost.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitProjectileGhost", false);
            var particle = projectileGhost.GetComponentInChildren<ParticleSystem>();
            if (particle)
            {
                var renderer = particle.gameObject.GetComponent<Renderer>();
                if (renderer)
                {
                    renderer.material = ContentProvider.GetOrCreateMaterial("matSpitterSpit", CreateRecoloredSpitMaterial);
                }
            }

            return projectileGhost;

        }

        public Material CreateRecoloredSpitMaterial()
        {
            var newMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetleSpitLarge.mat").WaitForCompletion());
            newMaterial.name = "matSpitterSpit";
            newMaterial.SetColor("_TintColor", new Color(1f, 0.1764f, 0f));

            return newMaterial;
        }

        public Material SetupDoTZoneDecalMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetleQueenAcidDecal.mat").WaitForCompletion());
            material.name = "matSpitterAcidDecal";
            material.SetColor("_Color", new Color(1f, 140f / 255f, 0f));

            return material;
        }

    }
}
