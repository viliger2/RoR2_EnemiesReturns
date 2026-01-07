using EnemiesReturns.Behaviors;
using EnemiesReturns.Enemies.MechanicalSpider.Drone;
using R2API;
using RoR2;
using RoR2.Audio;
using RoR2.Hologram;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.MechanicalSpider
{
    public class MechanicalSpiderStuff
    {
        public static LoopSoundDef ProjectileFlightSoundLoop;

        public static GameObject InteractablePrefab;

        public struct SpawnCards
        {
            public static InteractableSpawnCard iscMechanicalSpiderBroken;
        }

        // TODO: should probably make a factory similar to master and bodies but cba at the moment
        public GameObject CreateInteractable(GameObject interactablePrefab, GameObject masterPrefab)
        {
            var meshRendererTransform = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/MechanicalSpider");
            var meshRenderer = meshRendererTransform.GetComponent<SkinnedMeshRenderer>();
            var hologramPivot = interactablePrefab.transform.Find("HologramPivot");
            var modelTransform = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider");
            var modelBase = interactablePrefab.transform.Find("ModelBase");

            #region MechanicalSpiderBroken

            #region NetworkIdentity
            interactablePrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region Highlight
            var highlight = interactablePrefab.AddComponent<Highlight>();
            highlight.targetRenderer = meshRenderer;
            highlight.strength = 1f;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            #endregion

            #region GenericInspectInfoProvider
            var inspectDef = ScriptableObject.CreateInstance<InspectDef>();
            (inspectDef as ScriptableObject).name = "idBrokenMechanicalSpider";
            inspectDef.Info = new RoR2.UI.InspectInfo
            {
                Visual = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texDroneIconOutlined.png").WaitForCompletion(),
                TitleToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_INTERACTABLE_NAME",
                DescriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_INTERACTABLE_DESCRIPTION",
                FlavorToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_BODY_LORE",
                TitleColor = Color.white,
                isConsumedItem = false
            };
            #endregion

            #region SummonMasterBehavior
            var summonMaster = interactablePrefab.AddComponent<SummonMasterBehavior>();
            summonMaster.masterPrefab = masterPrefab;
            summonMaster.callOnEquipmentSpentOnPurchase = false;
            summonMaster.destroyAfterSummoning = false;
            summonMaster.inspectDef = inspectDef;
            #endregion

            #region PurchaseInteraction
            var purchaseInteraction = interactablePrefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_INTERACTABLE_NAME";
            purchaseInteraction.contextToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_CONTEXT";
            purchaseInteraction.available = true;
            purchaseInteraction.costType = CostTypeIndex.Money;
            purchaseInteraction.cost = EnemiesReturns.Configuration.MechanicalSpider.DroneCost.Value;
            purchaseInteraction.solitudeCost = 0;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false;
            purchaseInteraction.requiredUnlockable = "";
            purchaseInteraction.ignoreSpherecastForInteractability = false;
            purchaseInteraction.purchaseStatNames = new string[] { "totalDronesPurchased" };
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            purchaseInteraction.isShrine = false;
            purchaseInteraction.isGoldShrine = false;
            purchaseInteraction.shouldProximityHighlight = true;
            purchaseInteraction.saleStarCompatible = false;
            purchaseInteraction.requiredExpansion = null;
            purchaseInteraction.requiredUnlockable = null;
            #endregion

            #region EventFunctions
            var eventFunctions = interactablePrefab.AddComponent<EventFunctions>();
            #endregion

            #region HologramProjector
            var projector = interactablePrefab.AddComponent<HologramProjector>();
            projector.displayDistance = 15f;
            projector.hologramPivot = hologramPivot;
            projector.disableHologramRotation = false;
            projector.hologramContentInstance = null;
            #endregion

            #region GenericDisplayNameProvider
            interactablePrefab.AddComponent<GenericDisplayNameProvider>().displayToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_BODY_NAME"; // its empty for drones so who knows
            #endregion

            #region ModelLocator
            var modelLocator = interactablePrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelVisibility = null;
            modelLocator.modelBaseTransform = modelBase;
            modelLocator.modelScaleCompensation = 1f;

            modelLocator.autoUpdateModelTransform = false;
            modelLocator.dontDetatchFromParent = true;

            modelLocator.noCorpse = false;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.preserveModel = false;
            modelLocator.forceCulled = false;

            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.1f;
            modelLocator.normalMaxAngleDelta = 90f;
            #endregion

            #region Inventory
            interactablePrefab.AddComponent<Inventory>();
            #endregion

            #region SetEliteRampFromInventory
            var setEliteRamp = interactablePrefab.AddComponent<SetEliteRampOnShader>();
            setEliteRamp.renderers = interactablePrefab.GetComponentsInChildren<Renderer>();
            #endregion

            #region SpiderDroneOnPurchaseEvents
            var purchaseEvetns = interactablePrefab.AddComponent<MechanicalSpiderDroneOnPurchaseEvents>();
            purchaseEvetns.purchaseInteraction = purchaseInteraction;
            purchaseEvetns.eventFunctions = eventFunctions;
            purchaseEvetns.summonMasterBehavior = summonMaster;
            #endregion

            #region ParticleEffects
            var rightFrontLeg = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg1.1/SparkRightFrontLeg");
            var backLeftLeg = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg3.1");

            var brokenMissileDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MissileDroneBroken.prefab").WaitForCompletion();
            var sparkGameObject = brokenMissileDrone.transform.Find("ModelBase/mdlDrone2/BrokenDroneVFX/Damage Point/Small Sparks, Point").gameObject;
            var smallSparksRight = UnityEngine.GameObject.Instantiate(sparkGameObject);
            smallSparksRight.transform.parent = rightFrontLeg;
            smallSparksRight.transform.localPosition = Vector3.zero;
            smallSparksRight.transform.localScale = new Vector3(2f, 2f, 2f);

            var smallSparksLeft = UnityEngine.GameObject.Instantiate(sparkGameObject);
            smallSparksLeft.transform.parent = backLeftLeg;
            smallSparksLeft.transform.localPosition = Vector3.zero;
            smallSparksLeft.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            smallSparksLeft.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            var smoke = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/UpperBody");
            var smokeGameObject = brokenMissileDrone.transform.Find("ModelBase/mdlDrone2/BrokenDroneVFX/Damage Point/Smoke, Point").gameObject;
            var smokeCenter = UnityEngine.GameObject.Instantiate(smokeGameObject);
            smokeCenter.transform.parent = smoke;
            smokeCenter.transform.localPosition = Vector3.zero;
            smokeCenter.transform.localScale = new Vector3(2f, 2f, 2f);
            #endregion

            #endregion

            var flickerEmission = interactablePrefab.AddComponent<FlickerEmission>();
            flickerEmission.renderer = meshRenderer;
            flickerEmission.soundRepeatThreshold = 0.2f;
            flickerEmission.soundEmissionValue = 6.5f;
            flickerEmission.soundName = "ER_Spider_Light_Flicker_Play";
            flickerEmission.sinWaves = new Wave[]
            {
                new Wave()
                {
                    amplitude = 0.12f,
                    frequency = 3.25f,
                    cycleOffset = 1.2f,
                },
                new Wave()
                {
                    amplitude = 0.1f,
                    frequency = 8.9f,
                    cycleOffset = 0f,
                },
                new Wave()
                {
                    amplitude = 0.12f,
                    frequency = 1f,
                    cycleOffset = 2f,
                }
            };

            meshRendererTransform.gameObject.AddComponent<EntityLocator>().entity = interactablePrefab;

            var soa = interactablePrefab.AddComponent<SpecialObjectAttributes>();
            soa.grabbable = true;
            soa.massOverride = 150f;
            soa.damageOverride = -1f;
            soa.damageTypeOverride = new DamageTypeCombo(DamageType.Generic, DamageTypeExtended.Generic, DamageSource.NoneSpecified);
            soa.collisionToDisable = new List<GameObject>() { interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/MechanicalSpider").gameObject };
            soa.renderersToDisable = new List<Renderer>(interactablePrefab.GetComponentsInChildren<Renderer>());
            soa.behavioursToDisable = new List<MonoBehaviour>() { highlight, purchaseInteraction, projector };
            soa.hullClassification = HullClassification.Human;
            soa.maxDurability = 0;
            soa.orientToFloor = true;
            soa.useSkillHighlightRenderers = false;
            soa.isVoid = false;

            interactablePrefab.RegisterNetworkPrefab();

            return interactablePrefab;
        }

        public InteractableSpawnCard CreateInteractableSpawnCard(string name, GameObject prefab)
        {
            var card = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            (card as ScriptableObject).name = name;
            card.sendOverNetwork = true;
            card.prefab = prefab;
            card.hullSize = HullClassification.Human;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoChestSpawn;
            card.directorCreditCost = 0; // does it even matter?
            card.occupyPosition = true;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.orientToFloor = true;
            card.slightlyRandomizeOrientation = false;
            card.skipSpawnWhenSacrificeArtifactEnabled = false;
            card.weightScalarWhenSacrificeArtifactEnabled = 1f;
            card.skipSpawnWhenDevotionArtifactEnabled = true;
            card.maxSpawnsPerStage = -1;
            card.prismaticTrialSpawnChance = 1f;

            return card;
        }
        // end TODO

        public GameObject CreateDoubleShotChargeEffect()
        {
            var chargeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ChargeGolem.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderCharge", false);

            var particleDuration = chargeEffect.GetComponent<ScaleParticleSystemDuration>();
            particleDuration.initialDuration = 3f; // same as charge state duration
            particleDuration._newDuration = EnemiesReturns.Configuration.MechanicalSpider.DoubleShotChargeDuration.Value; // same as charge state duration

            var light = chargeEffect.transform.Find("Point light").gameObject.GetComponent<Light>();
            light.color = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            var glow = chargeEffect.transform.Find("Particles/Glow").gameObject.GetComponent<ParticleSystem>();

            var glowMain = glow.main;
            glowMain.simulationSpeed = 2f;

            var colorOverLifetime = glow.colorOverLifetime;

            var gradient = new Gradient();
            gradient.mode = GradientMode.Blend;
            gradient.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 0f, time = 0f },
                new GradientAlphaKey{alpha = 181f, time = 0.962f},
                new GradientAlphaKey{alpha = 255f, time = 1f}
            };
            gradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(0.8490566f, 0.6350543f, 0.1321645f), time = 0f},
                new GradientColorKey{color = new Color(0.5849056f, 0.4381365f, 0.09656459f), time = 0.942f},
                new GradientColorKey{color = new Color(1f, 1f, 0f), time = 1f},
            };

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            chargeEffect.transform.Find("Particles/Glow").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderChargeGlow", CreateChargeGlowMaterial);

            var sparks = chargeEffect.transform.Find("Particles/Sparks").gameObject.GetComponent<ParticleSystem>();
            var colorOverLifetime2 = sparks.colorOverLifetime;

            var gradient2 = new Gradient();
            gradient2.mode = GradientMode.Blend;
            gradient2.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 255f, time = 0f },
                new GradientAlphaKey{alpha = 0f, time = 1f}
            };
            gradient2.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(0.8923398f, 1f, 0.1372549f), time = 0f},
                new GradientColorKey{color = new Color(1f, 0.6179392f, 0.03137255f), time = 0.05f},
                new GradientColorKey{color = new Color(1f, 0.6179392f, 0.03137255f), time = 1f},
            };

            colorOverLifetime2.color = new ParticleSystem.MinMaxGradient(gradient2);

            particleDuration.particleSystems = new ParticleSystem[] { glow };

            // adding stuff so it becomes an effect
            var vfxAttributes = chargeEffect.AddComponent<VFXAttributes>();
            vfxAttributes.DoNotPool = false;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
            vfxAttributes.optionalLights = new Light[] { light };

            //var effectComponent = chargeEffect.AddComponent<EffectComponent>();
            //effectComponent.positionAtReferencedTransform = true;
            //effectComponent.parentToReferencedTransform = true;

            return chargeEffect;
        }

        public Material CreateChargeGlowMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matArcaneCircleProvi.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderChargeGlow";
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/UI/texCrosshairCircle.png").WaitForCompletion());

            return material;
        }

        public GameObject CreateDoubleShotProjectilePrefab(GameObject impactEffect)
        {
            var projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotProjectile", true);

            var projectileController = projectilePrefab.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = CreateDoubleShotGhostPrefab();
            projectileController.flightSoundLoop = CreateProjectileLoopSoundDef();

            var projectileSingleTargetImpact = projectilePrefab.GetComponent<ProjectileSingleTargetImpact>();
            projectileSingleTargetImpact.impactEffect = impactEffect;

            return projectilePrefab;
        }

        private LoopSoundDef CreateProjectileLoopSoundDef()
        {
            var projectileFlightSoundLoop = ScriptableObject.CreateInstance<LoopSoundDef>();
            (projectileFlightSoundLoop as ScriptableObject).name = "lsdMechanicalSpiderProjectileFlight";
            projectileFlightSoundLoop.startSoundName = "ER_Spider_Projectile_Loop_Play";
            projectileFlightSoundLoop.startSoundName = "ER_Spider_Projectile_Loop_Stop";

            ProjectileFlightSoundLoop = projectileFlightSoundLoop;

            return projectileFlightSoundLoop;
        }

        public GameObject CreateDoubleShotImpactEffect()
        {
            var projectileEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitImpactEffect.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotImpactEffect", false);

            var effectComponent = projectileEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "ER_Spider_Projectile_Hit_Play";

            UnityEngine.GameObject.DestroyImmediate(projectileEffect.transform.Find("Goo").gameObject);

            var flashParticleSystem = projectileEffect.transform.Find("Flash").gameObject.GetComponent<ParticleSystem>();
            var main = flashParticleSystem.main;
            main.startColor = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            projectileEffect.transform.Find("Point Light").gameObject.GetComponent<Light>().color = new Color(0.8490566f, 0.6350543f, 0.1321645f);
            projectileEffect.transform.Find("Ring, Mesh").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectile", CreateDoubleShotProjectileImpactMaterial);

            return projectileEffect;
        }

        public GameObject CreateDoubleShotGhostPrefab()
        {
            var projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRampingGhost.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotProjectileGhost", false);

            var flames = projectileGhost.transform.Find("Flames").gameObject;
            flames.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectileFlames", CreateDoubleShotProjectileFlamesMaterial);
            var flamesCoL = flames.GetComponent<ParticleSystem>().colorOverLifetime;
            var flameGradient = new Gradient();
            flameGradient.mode = GradientMode.Blend;
            flameGradient.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 0f, time = 0f },
                new GradientAlphaKey{alpha = 255f, time = 0.138f },
                new GradientAlphaKey{alpha = 0f, time = 1f}
            };
            flameGradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(1f, 0.969696f, 0.6196f), time = 0f},
                new GradientColorKey{color = new Color(0.9137f, 0.5220f, 0.0039f), time = 0.418f},
            };
            flamesCoL.color = new ParticleSystem.MinMaxGradient(flameGradient);

            projectileGhost.transform.Find("BurstVFX").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectileBurst", CreateDoubleShotProjectileBurstMaterial);

            var trail = projectileGhost.transform.Find("Trail").gameObject;
            var trailTrail = trail.GetComponent<TrailRenderer>();
            var trailGradient = new Gradient();
            trailGradient.mode = GradientMode.Blend;
            trailGradient.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 255f, time = 0.421f },
                new GradientAlphaKey{alpha = 4f, time = 1f}
            };
            trailGradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(1f, 1f, 1f), time = 0f},
                new GradientColorKey{color = new Color(0.4705f, 0.8840f, 09f), time = 0.038f},
                new GradientColorKey{color = new Color(0.4235f, 0.6274f, 0.9058f), time = 0.156f},
                new GradientColorKey{color = new Color(0.2666f, 0.2196f, 0.0941f), time = 1f},
            };
            trailTrail.colorGradient = trailGradient;
            trailTrail.material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectileTrail", CreateDoubleShotProjectileTrailMaterial);

            var core = projectileGhost.transform.Find("Core").gameObject;
            var coreCoL = core.GetComponent<ParticleSystem>().colorOverLifetime;
            coreCoL.color = new ParticleSystem.MinMaxGradient(flameGradient);

            projectileGhost.transform.Find("Point Light").gameObject.GetComponent<Light>().color = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            return projectileGhost;
        }

        public GameObject CreateDoubleShotGhostPrefabOld()
        {
            var projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitGhost.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotProjectileGhostOld", false);
            projectileGhost.transform.Find("Goo, WS").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectile", CreateDoubleShotProjectileImpactMaterial);
            projectileGhost.transform.Find("Goo, Directional").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectile", CreateDoubleShotProjectileImpactMaterial);
            projectileGhost.transform.Find("Trail").gameObject.GetComponent<TrailRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotTrail", CreateDoubleShotTrailOldMaterial);

            projectileGhost.transform.Find("Point light").gameObject.GetComponent<Light>().color = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            UnityEngine.GameObject.DestroyImmediate(projectileGhost.GetComponent<DetachParticleOnDestroyAndEndEmission>()); // fixes log spam on projectile destroy

            return projectileGhost;
        }

        public Material CreateDoubleShotProjectileTrailMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoShotgunTracerCore.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotProjectileTrail";
            material.SetColor("_TintColor", new Color(1f, 0.6540f, 0f, 1f));

            return material;
        }

        public Material CreateDoubleShotProjectileFlamesMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoFmjSweetSpotGlow.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotProjectileFlames";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            return material;
        }

        public Material CreateDoubleShotProjectileBurstMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoFmjSweetSpotBurst.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotProjectileBurst";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            return material;
        }

        public Material CreateDoubleShotProjectileImpactMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/FlyingVermin/matVerminGooSmall.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotProjectile";
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Junk/Mage/texMageLaserMask.png").WaitForCompletion());
            material.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudPixel2.png").WaitForCompletion());
            material.SetTextureScale("_Cloud1Tex", new Vector2(0.5f, 0.5f));
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());
            material.SetFloat("_InvFade", 7f);
            material.SetFloat("_AlphaBoost", 7.32f);
            material.SetFloat("_Cutoff", 0.542f);
            material.SetFloat("_SpecularStrength", 0.701f);
            material.SetFloat("_SpecularExponent", 2.86f);
            material.SetInt("_RampInfo", 0);
            return material;
        }

        public Material CreateDoubleShotTrailOldMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/FlyingVermin/matVerminGooTrail.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotTrail";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            return material;
        }

    }
}
