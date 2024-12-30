using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Projectiles;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritStuff
    {
        public static DeployableSlot PylonDeployable;

        public static R2API.ModdedProcType PillarExplosion;

        public static void Hooks()
        {
            PylonDeployable = R2API.DeployableAPI.RegisterDeployableSlot(GetPylonCount);
        }

        private static int GetPylonCount(CharacterMaster master, int countMultiplier)
        {
            return EnemiesReturns.Configuration.Ifrit.PillarMaxInstances.Value;
        }

        public GameObject CreateDeathEffect()
        {
            var effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBruiserDeathImpact.prefab").WaitForCompletion().InstantiateClone("IfritDeathEffect", false);
            effect.GetComponent<EffectComponent>().applyScale = true;

            foreach(var system in effect.GetComponentsInChildren<ParticleSystem>())
            {
                var main = system.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            return effect;
        }

        public GameObject CreateHellzoneProjectile()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("IfritHellzoneProjectile", true);

            var controller = gameObject.GetComponent<ProjectileController>();
            controller.ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/MegaFireballGhost.prefab").WaitForCompletion();
            controller.startSound = "Play_lemurianBruiser_m1_shoot";
            controller.flightSoundLoop = Addressables.LoadAssetAsync<LoopSoundDef>("RoR2/Base/LemurianBruiser/lsdLemurianBruiserFireballFlight.asset").WaitForCompletion();

            gameObject.GetComponent<ProjectileDamage>().damageType.damageType = DamageType.IgniteOnHit;

            if (gameObject.TryGetComponent<ProjectileImpactExplosion>(out var component))
            {
                component.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/OmniExplosionVFXLemurianBruiserFireballImpact.prefab").WaitForCompletion();
                component.fireChildren = false;
                component.blastRadius = EnemiesReturns.Configuration.Ifrit.HellzoneRadius.Value;
                component.blastDamageCoefficient = 1f; // leave it at 1 so projectile itself deals full damage
            }

            return gameObject;
        }

        public GameObject CreateHellzonePredictionProjectile(GameObject dotZone, Texture2D texLavaCrack)
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanPreFistProjectile.prefab").WaitForCompletion().InstantiateClone("IfritHellzonePreProjectile", true);

            var controller = gameObject.GetComponent<ProjectileController>();
            controller.ghostPrefab = null;
            controller.startSound = "ER_Ifrit_Hellzone_Spawn_Play";

            var scale = EnemiesReturns.Configuration.Ifrit.HellzoneRadius.Value;
            gameObject.transform.Find("TeamAreaIndicator, GroundOnly").transform.localScale = new Vector3(scale, scale, scale);

            var beetleQueen = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenAcid.prefab").WaitForCompletion();
            var beetleQueenDecal = beetleQueen.transform.Find("FX/Decal");
            var decalGameObject = UnityEngine.GameObject.Instantiate(beetleQueenDecal.gameObject);

            decalGameObject.SetActive(true);
            decalGameObject.transform.parent = gameObject.transform;
            decalGameObject.transform.localPosition = Vector3.zero;
            decalGameObject.transform.localRotation = Quaternion.identity;
            decalGameObject.transform.localScale = new Vector3(20f, 20f, 20f);

            var decal = decalGameObject.GetComponent<Decal>();
            decal.Material = ContentProvider.GetOrCreateMaterial("matIfritHellzoneDecalLavaCrack", CreatePreditionDecalMaterial, texLavaCrack);

            var impactExplosion = gameObject.GetComponent<ProjectileImpactExplosion>();
            impactExplosion.impactEffect = null;
            impactExplosion.blastDamageCoefficient = 0.1f;
            impactExplosion.lifetime = 2f;

            impactExplosion.fireChildren = true;
            impactExplosion.childrenProjectilePrefab = dotZone;
            impactExplosion.childrenDamageCoefficient = 1f;
            impactExplosion.minAngleOffset = Vector3.zero;
            impactExplosion.maxAngleOffset = Vector3.zero;

            return gameObject;
        }

        public GameObject CreateHellfireDotZoneProjectile(GameObject pillarPrefab, GameObject volcanoEffectPrefab, Texture2D texLavaCrack, NetworkSoundEventDef nsedChildSpawnSound)
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenAcid.prefab").WaitForCompletion().InstantiateClone("IfritHellzoneDoTZoneProjectile", true);

            var lifetime = EnemiesReturns.Configuration.Ifrit.HellzoneDoTZoneLifetime.Value
                + EnemiesReturns.Configuration.Ifrit.HellzonePillarCount.Value * EnemiesReturns.Configuration.Ifrit.HellzonePillarDelay.Value;
            gameObject.GetComponent<ProjectileDotZone>().lifetime = lifetime;

            var controller = gameObject.GetComponent<ProjectileController>();
            controller.ghostPrefab = null;
            controller.startSound = "ER_Ifrit_Volcano_Play";

            gameObject.GetComponent<ProjectileDamage>().damageType.damageType = DamageType.IgniteOnHit;

            var fxTransform = gameObject.transform.Find("FX");
            var fxScale = EnemiesReturns.Configuration.Ifrit.HellzoneRadius.Value;
            fxTransform.localScale = new Vector3(fxScale, fxScale, fxScale);
            fxTransform.localRotation = Quaternion.identity;

            UnityEngine.GameObject.DestroyImmediate(gameObject.transform.Find("FX/Spittle").gameObject);
            UnityEngine.GameObject.DestroyImmediate(gameObject.transform.Find("FX/Gas").gameObject);

            var light = gameObject.transform.Find("FX/Point Light");
            light.gameObject.SetActive(true);
            light.localPosition = new Vector3(0f, 0.1f, 0f);

            var lightComponent = light.GetComponent<Light>();
            lightComponent.range = EnemiesReturns.Configuration.Ifrit.HellzoneRadius.Value;
            lightComponent.color = new Color(1f, 0.54f, 0.172f);

            gameObject.transform.Find("FX/Hitbox").transform.localScale = new Vector3(1.5f, 0.33f, 1.5f);

            var teamIndicator = UnityEngine.GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, GroundOnly.prefab").WaitForCompletion());
            teamIndicator.transform.parent = fxTransform;
            teamIndicator.transform.localPosition = Vector3.zero;
            teamIndicator.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            teamIndicator.transform.localScale = Vector3.one;
            teamIndicator.GetComponent<TeamAreaIndicator>().teamFilter = gameObject.GetComponent<TeamFilter>();

            var volcano = UnityEngine.GameObject.Instantiate(volcanoEffectPrefab);
            volcano.transform.parent = fxTransform;
            volcano.transform.localPosition = Vector3.zero;
            volcano.transform.localRotation = Quaternion.identity;
            volcano.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // 0.5 works, since we attach it to projectile and then it scales of main projectile scaling

            var particleSystem = volcano.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.startLifetime = lifetime;
            main.duration = lifetime;

            var particleRenderer = volcano.GetComponentInChildren<ParticleSystemRenderer>();
            particleRenderer.material = Addressables.LoadAssetAsync<Material>("RoR2/DLC2/Scorchling/matScorchlingBreachPile.mat").WaitForCompletion();

            var spawnChildrenComponent = gameObject.AddComponent<ProjectileSpawnChildrenInRowsWithDelay>();
            spawnChildrenComponent.radius = EnemiesReturns.Configuration.Ifrit.HellzoneRadius.Value;
            spawnChildrenComponent.numberOfRows = EnemiesReturns.Configuration.Ifrit.HellzonePillarCount.Value;
            spawnChildrenComponent.childrenDamageCoefficient = EnemiesReturns.Configuration.Ifrit.HellzonePillarDamage.Value;
            spawnChildrenComponent.delayEachRow = EnemiesReturns.Configuration.Ifrit.HellzonePillarDelay.Value;
            spawnChildrenComponent.childPrefab = pillarPrefab;
            spawnChildrenComponent.soundEventDef = nsedChildSpawnSound;

            return gameObject;
        }

        public GameObject CreateHellzonePillarProjectile(GameObject gameObject, GameObject ghostPrefab)
        {
            var hitboxTransform = gameObject.transform.Find("Hitbox");
            if (!hitboxTransform)
            {
                Log.Error("Projectile " + gameObject.name + " doesn't have a hitbox.");
                return gameObject;
            }

            var hitbox = hitboxTransform.gameObject.AddComponent<HitBox>();

            gameObject.AddComponent<NetworkIdentity>().localPlayerAuthority = true;

            var projectileController = gameObject.AddComponent<ProjectileController>();
            projectileController.ghostPrefab = CreateHellzonePillarProjectileGhost(ghostPrefab);
            projectileController.cannotBeDeleted = true;
            projectileController.canImpactOnTrigger = false;
            projectileController.allowPrediction = false;
            projectileController.procCoefficient = 1f;
            projectileController.startSound = "";

            var networkTransform = gameObject.AddComponent<ProjectileNetworkTransform>();
            networkTransform.positionTransmitInterval = 0.03f;
            networkTransform.interpolationFactor = 1f;
            networkTransform.allowClientsideCollision = false;

            var projectileDamage = gameObject.AddComponent<ProjectileDamage>();
            projectileDamage.damageType.damageType = DamageType.IgniteOnHit;
            projectileDamage.useDotMaxStacksFromAttacker = false;

            gameObject.AddComponent<TeamFilter>();

            var hitboxGroup = gameObject.AddComponent<HitBoxGroup>();
            hitboxGroup.name = "Hitbox";
            hitboxGroup.hitBoxes = new HitBox[] { hitbox };

            var projectileOverlapAttack = gameObject.AddComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.damageCoefficient = 1f;
            projectileOverlapAttack.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileExplosionVFX.prefab").WaitForCompletion();
            projectileOverlapAttack.forceVector = new Vector3(0f, EnemiesReturns.Configuration.Ifrit.HellzonePillarForce.Value, 0f);
            projectileOverlapAttack.overlapProcCoefficient = 1f;
            projectileOverlapAttack.maximumOverlapTargets = 100;
            projectileOverlapAttack.fireFrequency = 0.001f;
            projectileOverlapAttack.resetInterval = -1f;

            var projectileSimple = gameObject.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 1f;
            projectileSimple.lifetimeExpiredEffect = null;
            projectileSimple.desiredForwardSpeed = 0f;
            projectileSimple.updateAfterFiring = false;
            projectileSimple.enableVelocityOverLifetime = false;
            projectileSimple.oscillate = false;

            gameObject.RegisterNetworkPrefab();
            return gameObject;
        }

        public GameObject CreateHellzonePillarProjectileGhost(GameObject gameObject)
        {
            gameObject.AddComponent<ProjectileGhostController>().inheritScaleFromProjectile = true;

            var vfxAttributes = gameObject.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;

            gameObject.AddComponent<EffectManagerHelper>();

            var sparksTransform = gameObject.transform.Find("FX/Sparks");
            var psSparks = sparksTransform.gameObject.GetComponent<Renderer>();
            psSparks.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion();

            var rocksTransform = gameObject.transform.Find("FX/Rocks");
            var psRocks = rocksTransform.gameObject.GetComponent<Renderer>();
            psRocks.material = Addressables.LoadAssetAsync<Material>("RoR2/DLC2/helminthroost/Assets/matHRRocks.mat").WaitForCompletion();

            var fireballTransform = gameObject.transform.Find("FX/MainFireball");
            var psFireball = fireballTransform.gameObject.GetComponent<Renderer>();
            psFireball.material = Addressables.LoadAssetAsync<Material>("RoR2/DLC2/helminthroost/Assets/matHRLava.mat").WaitForCompletion();

            var steamedHamsTransform = gameObject.transform.Find("FX/MainFireball/Smoke");
            var psSmoke = steamedHamsTransform.GetComponent<Renderer>();
            psSmoke.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/dampcave/matEnvSteam.mat").WaitForCompletion();

            return gameObject;
        }

        public Material CreatePreditionDecalMaterial(Texture2D texLavaCrack)
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetleQueenAcidDecal.mat").WaitForCompletion());
            material.name = "matIfritHellzoneDecalLavaCrack";
            material.SetTexture("_MaskTex", texLavaCrack);
            material.SetColor("_Color", new Color(255f / 255f, 103f / 255f, 127f / 255f));
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texBehemothRamp.png").WaitForCompletion());
            material.SetFloat("_AlphaBoost", 0.9f);
            material.SetInt("_DecalSrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetInt("_DecalDstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            material.SetTextureScale("_Cloud1Tex", Vector2.zero);
            material.SetTextureScale("_Cloud2Tex", Vector2.zero);
            material.SetVector("_CutoffScroll", new Vector4(0f, 0f, 0f, 0f));

            return material;
        }

        public GameObject CreateFlameBreath()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FlamebreathEffect.prefab").WaitForCompletion().InstantiateClone("IfritFlameBreathEffect", false);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<ScaleParticleSystemDuration>());

            var components = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.loop = true;
                component.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            }

            return gameObject;
        }

        public GameObject CreateBreathParticle()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenScream.prefab").WaitForCompletion().InstantiateClone("IfritScream", false);

            var components = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.duration = 1f;
                if (component.gameObject.name == "Spit")
                {
                    main.startColor = new Color(1f, 0.4895f, 0f);
                }
            }

            return gameObject;
        }

        public GameObject CreateSpawnEffect(GameObject gameObject, AnimationCurveDef ppCurve)
        {
            var effectComponent = gameObject.AddComponent<EffectComponent>();
            effectComponent.positionAtReferencedTransform = true;
            effectComponent.soundName = "ER_IFrit_Portal_Spawn_Play";

            var vfxAttributes = gameObject.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.DoNotPool = true; // it breaks alligment on another spawn

            var destroyOnEnd = gameObject.AddComponent<DestroyOnParticleEnd>();

            var allignToNormal = gameObject.AddComponent<AlignToNormal>();
            allignToNormal.maxDistance = 15f;
            allignToNormal.offsetDistance = 3f;

            gameObject.transform.Find("Billboard").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matIfritSpawnBillboard", CreateSpawnBillboardMaterial);
            gameObject.transform.Find("NoiseTrails").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matIfritSpawnTrails", CreateSpawnTrailsMaterial);

            var worm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
            var ppvolume = worm.transform.Find("ModelBase/mdlMagmaWorm/WormArmature/Head/PPVolume").gameObject.InstantiateClone("PPVolume", false);
            ppvolume.transform.parent = effectComponent.transform;
            ppvolume.transform.localScale = Vector3.one;
            ppvolume.transform.position = Vector3.zero;
            ppvolume.transform.rotation = Quaternion.identity;
            ppvolume.layer = LayerIndex.postProcess.intVal;

            ppvolume.gameObject.GetComponent<PostProcessVolume>().blendDistance = 30f;

            var components = ppvolume.GetComponents<PostProcessDuration>();
            for (int i = components.Length; i > 0; i--)
            {
                var component = components[i - 1];
                if (!component.enabled)
                {
                    UnityEngine.GameObject.Destroy(component);
                    continue;
                }

                component.ppWeightCurve = ppCurve.curve;
                component.maxDuration = 2f;
            }

            return gameObject;
        }

        public Material CreateSpawnBillboardMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffect.mat").WaitForCompletion());
            material.name = "matIfritSpawnBillboard";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaserTypeB.png").WaitForCompletion());
            material.SetColor("_TintColor", new Color(255f / 255f, 150f / 255f, 0));
            material.SetFloat("_InvFade", 1.260021f);
            material.SetFloat("_Boost", 3.98255f);
            material.SetFloat("_AlphaBoost", 3.790471f);
            material.SetFloat("_AlphaBias", 0.0766565f);

            return material;
        }

        public Material CreateSpawnTrailsMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion());
            material.name = "matIfritSpawnTrails";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaserTypeB.png").WaitForCompletion());

            return material;
        }
    }
}
