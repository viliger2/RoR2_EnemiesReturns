using EnemiesReturns.Behaviors;
using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.Colossus
{
    public class ColossusStuff
    {
        public GameObject CreateSpawnEffect()
        {
            var cloneEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanSpawnEffect.prefab").WaitForCompletion().InstantiateClone("ColossusSpawnEffect", false);

            var shakeEmitter = cloneEffect.GetComponent<ShakeEmitter>();
            shakeEmitter.duration = 5f;
            shakeEmitter.radius = 100f;

            var components = cloneEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            var light = cloneEffect.GetComponentInChildren<Light>();
            if (light)
            {
                light.range = 20f;
            }

            cloneEffect.transform.localScale = new Vector3(2f, 2f, 2f);

            return cloneEffect;
        }

        public GameObject CreateDeath2Effect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanRechargeRocksEffect.prefab").WaitForCompletion().InstantiateClone("ColossusDeathEffect", false);

            var shakeEmitter = clonedEffect.GetComponent<ShakeEmitter>();
            shakeEmitter.duration = 2f;
            shakeEmitter.radius = 100f;

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            var light = clonedEffect.GetComponentInChildren<Light>();
            if (light)
            {
                light.range = 20f;
            }

            clonedEffect.transform.localScale = new Vector3(3f, 3f, 3f);

            return clonedEffect;
        }

        public GameObject CreateDeathFallEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGuard/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("ColossusStompEffect", false);

            var shakeEmitter = clonedEffect.GetComponent<ShakeEmitter>();
            shakeEmitter.duration = 4f;
            shakeEmitter.radius = 100f;
            shakeEmitter.wave.amplitude = 0.3f;
            shakeEmitter.wave.frequency = 200f;

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Large").gameObject);
            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Small").gameObject);

            clonedEffect.transform.localScale = new Vector3(4f, 4f, 4f);

            return clonedEffect;
        }

        public GameObject CreateStompEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGuard/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("ColossusStompEffect", false);

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            clonedEffect.transform.localScale = new Vector3(2f, 2f, 2f);

            return clonedEffect;
        }

        public GameObject CreateClapEffect()
        {
            GameObject clapEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolemDeath.prefab").WaitForCompletion().InstantiateClone("ColossusClapEffect", false);
            var components = clapEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            // scaling size of default values
            // 4f is default effect scale (actually not, this is the value due to separated bulletattacks)
            // 12f is default damage radius scale
            var radius = 4f / 12f * EnemiesReturns.Configuration.Colossus.RockClapRadius.Value;

            clapEffect.transform.localScale = new Vector3(radius, radius, radius);
            return clapEffect;
        }

        public GameObject CreateStompProjectile()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGuard/Sunder.prefab").WaitForCompletion().InstantiateClone("ColossusStompProjectile", true);
            var clonedEffectGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleGuard/SunderGhost.prefab").WaitForCompletion().InstantiateClone("ColossusStompProjectileGhost", false);

            var components = clonedEffectGhost.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            clonedEffect.GetComponent<ProjectileCharacterController>().lifetime = EnemiesReturns.Configuration.Colossus.StompProjectileLifetime.Value;

            var shakeEmiiter = clonedEffectGhost.GetComponent<ShakeEmitter>();
            shakeEmiiter.radius = 30;
            shakeEmiiter.duration = 0.15f;
            shakeEmiiter.wave.amplitude = 10f;
            shakeEmiiter.wave.frequency = 30f;
            shakeEmiiter.wave.cycleOffset = 0f;

            var ghostAnchor = new GameObject();
            ghostAnchor.name = "Anchor";
            ghostAnchor.transform.parent = clonedEffect.transform;
            ghostAnchor.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            ghostAnchor.transform.localScale = new Vector3(1f, 1f, 1f);

            var projectileController = clonedEffect.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = clonedEffectGhost;
            projectileController.ghostTransformAnchor = ghostAnchor.transform;

            var hitbox = clonedEffect.transform.Find("Hitbox");
            hitbox.transform.localScale = new Vector3(1f, 1.2f, 1.5f);
            hitbox.transform.localPosition = new Vector3(0f, 0.1f, 0f);

            var scale = EnemiesReturns.Configuration.Colossus.StompProjectileScale.Value;

            clonedEffect.transform.localScale = new Vector3(scale, scale, scale);
            clonedEffectGhost.transform.localScale = new Vector3(scale, scale, scale);

            return clonedEffect;
        }

        public GameObject CreateFlyingRocksGhost()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandparentMiniBoulderGhost.prefab").WaitForCompletion().InstantiateClone("ColossusFlyingRockGhost", false);
            clonedEffect.transform.localScale = new Vector3(2f, 2f, 2f); // for future reference: ProjectileController does not scale ghost to its size, unless ghost has a flag for it

            return clonedEffect;
        }

        public GameObject CreateFlyingRockProjectile(GameObject rockGhost)
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Grandparent/GrandparentMiniBoulder.prefab").WaitForCompletion().InstantiateClone("ColossusFlyingRockProjectile", true);
            var projectileController = clonedEffect.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = rockGhost;
            projectileController.allowPrediction = true;
            projectileController.flightSoundLoop = null;

            clonedEffect.transform.localScale = new Vector3(2f, 2f, 2f);

            var projectileSimple = clonedEffect.GetComponent<ProjectileSimple>();
            projectileSimple.updateAfterFiring = false;
            projectileSimple.lifetime = 5f;

            var impactExplosion = clonedEffect.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.blastRadius = EnemiesReturns.Configuration.Colossus.RockClapProjectileBlastRadius.Value;
            impactExplosion.childrenInheritDamageType = true;

            clonedEffect.AddComponent<ProjectileTargetComponent>();

            var targetFinder = clonedEffect.AddComponent<ProjectileSphereTargetFinder>();
            targetFinder.lookRange = EnemiesReturns.Configuration.Colossus.RockClapHomingRange.Value;
            targetFinder.targetSearchInterval = 0.25f;
            targetFinder.onlySearchIfNoTarget = true;
            targetFinder.allowTargetLoss = false;
            targetFinder.testLoS = false;
            targetFinder.ignoreAir = false;
            targetFinder.flierAltitudeTolerance = float.PositiveInfinity;

            var projectileMover = clonedEffect.AddComponent<ProjectileMoveTowardsTarget>();
            projectileMover.speed = EnemiesReturns.Configuration.Colossus.RockClapHomingSpeed.Value;

            var networkTransform = clonedEffect.AddComponent<ProjectileNetworkTransform>();
            networkTransform.positionTransmitInterval = 0.033f;
            networkTransform.interpolationFactor = 1f;

            return clonedEffect;
        }

        public GameObject CreateLaserBarrageProjectile()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanRockProjectile.prefab").WaitForCompletion().InstantiateClone("ColossusLaserBarrageProjectile", true);

            clonedEffect.layer = LayerIndex.debris.intVal;

            GameObject shardEffect = CreateLaserBarrageShard();

            shardEffect.transform.parent = clonedEffect.transform;
            shardEffect.transform.localPosition = Vector3.zero;
            shardEffect.transform.localRotation = Quaternion.identity;
            shardEffect.transform.localScale = Vector3.one;

            shardEffect.SetActive(false);

            clonedEffect.GetComponent<Rigidbody>().useGravity = true;

            clonedEffect.GetComponent<ProjectileSimple>().updateAfterFiring = false;

            UnityEngine.Object.DestroyImmediate(clonedEffect.GetComponent<ProjectileSteerTowardTarget>());
            UnityEngine.Object.DestroyImmediate(clonedEffect.GetComponent<ProjectileDirectionalTargetFinder>());
            UnityEngine.Object.DestroyImmediate(clonedEffect.GetComponent<ProjectileTargetComponent>());

            var hitbox = new GameObject();
            hitbox.name = "Hitbox";
            hitbox.layer = LayerIndex.projectile.intVal;
            hitbox.transform.parent = clonedEffect.transform;
            hitbox.transform.localScale = Vector3.one;
            hitbox.transform.localPosition = Vector3.zero;
            var hitboxComponent = hitbox.gameObject.AddComponent<HitBox>();

            var hibboxGroup = clonedEffect.AddComponent<HitBoxGroup>();
            hibboxGroup.groupName = "Hitbox";
            hibboxGroup.hitBoxes = new HitBox[] { hitboxComponent };

            var overlapAttack = clonedEffect.AddComponent<ProjectileOverlapAttack>();
            overlapAttack.maximumOverlapTargets = 100;
            overlapAttack.overlapProcCoefficient = 1f;
            overlapAttack.damageCoefficient = 1f;

            var stickOnImpact = clonedEffect.AddComponent<ProjectileStickOnImpact>();
            stickOnImpact.ignoreCharacters = true;
            stickOnImpact.ignoreWorld = false;
            stickOnImpact.alignNormals = true;

            var impactExplosion = clonedEffect.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            impactExplosion.blastRadius = EnemiesReturns.Configuration.Colossus.LaserBarrageExplosionRadius.Value;
            impactExplosion.blastDamageCoefficient = EnemiesReturns.Configuration.Colossus.LaserBarrageExplosionDamage.Value;
            impactExplosion.blastProcCoefficient = 1f;
            impactExplosion.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            impactExplosion.canRejectForce = true;
            impactExplosion.explosionEffect = CreateLaserBarrageExplosion();
            impactExplosion.childrenInheritDamageType = true;

            impactExplosion.destroyOnEnemy = false;
            impactExplosion.destroyOnWorld = false;
            impactExplosion.impactOnWorld = true;
            impactExplosion.timerAfterImpact = true;
            impactExplosion.lifetime = 15f;
            impactExplosion.lifetimeAfterImpact = EnemiesReturns.Configuration.Colossus.LaserBarrageExplosionDelay.Value;
            impactExplosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            var dumbHelper = clonedEffect.AddComponent<DumbProjectileStickHelper>();
            dumbHelper.shardEffect = shardEffect;
            dumbHelper.stickOnImpact = stickOnImpact;
            dumbHelper.overlapAttack = overlapAttack;
            dumbHelper.controller = clonedEffect.GetComponent<ProjectileController>();
            dumbHelper.childToRotateTo = "LaserInitialPoint";

            return clonedEffect;
        }

        private GameObject CreateLaserBarrageExplosion()
        {
            var explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion().InstantiateClone("ColossusLaserBarrageExplosion", false);

            var components = explosionEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            var scale = 2f * (EnemiesReturns.Configuration.Colossus.LaserBarrageExplosionRadius.Value / 5f); // 5f is the value it was scaled to
            explosionEffect.transform.localScale = new Vector3(scale, scale, scale);

            return explosionEffect;
        }

        private GameObject CreateLaserBarrageShard()
        {
            var shardEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/LunarShardGhost.prefab").WaitForCompletion().InstantiateClone("ColossusLaserBarrageImpactShard", false);

            UnityEngine.Object.DestroyImmediate(shardEffect.GetComponent<ProjectileGhostController>());

            shardEffect.transform.Find("Mesh").GetComponent<MeshRenderer>().material = ContentProvider.GetOrCreateMaterial("matColossusLaserBarrageShardMesh", CreateLaserBarrageShardMeshMaterial);
            shardEffect.transform.Find("Trail").GetComponent<TrailRenderer>().material = ContentProvider.GetOrCreateMaterial("matColossusLaserBarrageShardTrail", CreateLaserBarrageShardTrailMaterial);
            shardEffect.transform.Find("PulseGlow").GetComponent<ParticleSystem>().GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matColossusLaserBarrageShardPulse", CreateLaserBarrageShardPulseMaterial);

            return shardEffect;
        }

        public Material CreateLaserBarrageShardMeshMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matLunarInfection.mat").WaitForCompletion());
            material.name = "matColossusLaserBarrageShardMesh";
            material.SetColor("_Color", new Color(181 / 255, 0, 0));
            material.SetTexture("_MainTex", Texture2D.redTexture);
            material.SetColor("_EmColor", new Color(255 / 255, 115 / 255, 115 / 255));

            return material;
        }

        public Material CreateLaserBarrageShardTrailMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matLunarShardTrail.mat").WaitForCompletion());
            material.name = "matColossusLaserBarrageShardTrail";
            material.SetColor("_TintColor", Color.red); // I mean

            return material;
        }

        public Material CreateLaserBarrageShardPulseMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matGlowItemPickup.mat").WaitForCompletion());
            material.name = "matColossusLaserBarrageShardPulse";
            material.SetColor("_TintColor", Color.red); // I mean

            return material;
        }

        public GameObject CreateLaserEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/VoidRaidCrabSpinBeamVFX.prefab").WaitForCompletion().InstantiateClone("ColossusEyeLaserEffect", false);

            var radius = 7.5f;
            clonedEffect.transform.localScale = new Vector3(radius, radius, clonedEffect.transform.localScale.z);

            // coloring book
            clonedEffect.transform.Find("Mesh, Additive").GetComponent<MeshRenderer>().material = ContentProvider.GetOrCreateMaterial("matColossusSpinBeamCylinder2", CreateSpinBeamCylinder2Material);
            clonedEffect.transform.Find("Mesh, Additive/Mesh, Transparent").GetComponent<MeshRenderer>().material = ContentProvider.GetOrCreateMaterial("matColossusSpinBeamCylinder1", CreateSpinBeamCylinder1Material);
            clonedEffect.transform.Find("Billboards").GetComponent<ParticleSystem>().GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matColossusSpinBeamBillboard1", CreateSpinBeamBillboard1Material);
            clonedEffect.transform.Find("SwirlyTrails").GetComponent<ParticleSystem>().GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matColossusSpinBeamBillboard2", CreateSpinBeamBillboard2Material);
            clonedEffect.transform.Find("MuzzleRayParticles").GetComponent<ParticleSystem>().GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matColossusSpinBeamBillboard2", CreateSpinBeamBillboard2Material);

            clonedEffect.transform.Find("Point Light, Middle").GetComponent<Light>().color = Color.red;
            clonedEffect.transform.Find("Point Light, End").GetComponent<Light>().color = Color.red;

            return clonedEffect;
        }

        public Material CreateSpinBeamCylinder2Material()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamCylinder2.mat").WaitForCompletion());
            material.name = "matColossusSpinBeamCylinder2";
            material.SetColor("_TintColor", Color.red); // I mean

            return material;
        }

        public Material CreateSpinBeamCylinder1Material()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamCylinder1.mat").WaitForCompletion());
            material.name = "matColossusSpinBeamCylinder1";
            material.SetColor("_TintColor", Color.red); // I mean

            return material;
        }

        public Material CreateSpinBeamBillboard1Material()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamBillboard1.mat").WaitForCompletion());
            material.name = "matColossusSpinBeamBillboard1";
            material.SetColor("_TintColor", Color.red); // I mean

            return material;
        }

        public Material CreateSpinBeamBillboard2Material()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabSpinBeamBillboard2.mat").WaitForCompletion());
            material.name = "matColossusSpinBeamBillboard2";
            material.SetColor("_TintColor", Color.red); // I mean

            return material;
        }

    }
}
