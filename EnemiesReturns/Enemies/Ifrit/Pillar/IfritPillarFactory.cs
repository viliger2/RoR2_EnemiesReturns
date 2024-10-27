using EnemiesReturns.Configuration;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Helpers;
using EnemiesReturns.PrefabAPICompat;
using HG;
using RoR2;
using RoR2.CharacterAI;
using RoR2.EntityLogic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.Ifrit.Pillar
{
    public class IfritPillarFactory
    {
        public struct Enemy
        {
            public static SpawnCard scIfritPillar;

            public static GameObject IfritPillarBody;

            public static GameObject IfritPillarMaster;
        }

        public struct Player
        {
            public static SpawnCard scIfritPillar;

            public static GameObject IfritPillarBody;

            public static GameObject IfritPillarMaster;
        }

        public class BodyInformation
        {
            public GameObject bodyPrefab;
            public Sprite sprite;
            public float baseDamage;
            public float levelDamage;
            public EntityStates.SerializableEntityStateType mainState;
            public EntityStates.SerializableEntityStateType deathState;
            public bool enableLineRenderer;
            public float explosionRadius;
        }

        public GameObject CreateBody(BodyInformation bodyInformation, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var bodyPrefab = bodyInformation.bodyPrefab;
            Transform modelBase = bodyPrefab.transform.Find("ModelBase");
            Transform modelTransform = bodyPrefab.transform.Find("ModelBase/IfritPillar");
            Transform hurtboxTransform = bodyPrefab.transform.Find("ModelBase/IfritPillar/IfritPillarArmture/Hurtbox");
            Transform fireball = bodyPrefab.transform.Find("ModelBase/IfritPillar/Fireball");

            var focusPoint = bodyPrefab.transform.Find("ModelBase/IfritPillar/LogBookTarget");
            var cameraPosition = bodyPrefab.transform.Find("ModelBase/IfritPillar/LogBookTarget/LogBookCamera");


            #region PillarBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region InputBankTest
            var inputBank = bodyPrefab.AddComponent<InputBankTest>();
            #endregion

            #region SkillLocator
            bodyPrefab.AddComponent<SkillLocator>();
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!bodyPrefab.TryGetComponent(out teamComponent))
            {
                teamComponent = bodyPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.None;
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!bodyPrefab.TryGetComponent(out characterBody))
            {
                characterBody = bodyPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ENEMIES_RETURNS_IFRIT_PYLON_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.HasBackstabImmunity;
            characterBody.baseMaxHealth = EnemiesReturns.Configuration.Ifrit.PillarBodyBaseMaxHealth.Value;
            characterBody.baseDamage = bodyInformation.baseDamage;
            characterBody.baseArmor = 0f;
            characterBody.levelArmor = 0f;
            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturns.Configuration.Ifrit.PillarBodyLevelMaxHealth.Value;
            characterBody.levelDamage = bodyInformation.levelDamage;
            characterBody.baseMoveSpeed = 0f;
            characterBody.levelMoveSpeed = 0f;
            characterBody.hullClassification = HullClassification.Golem;
            if (bodyInformation.sprite != null)
            {
                characterBody.portraitIcon = bodyInformation.sprite.texture;
            }
            #endregion

            #region HealthComponent
            var healthComponent = bodyPrefab.AddComponent<HealthComponent>();
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            healthComponent.dontShowHealthbar = false;
            #endregion

            #region ModelLocator
            var modelLocator = bodyPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBase;
            modelLocator.dontDetatchFromParent = true;
            modelLocator.noCorpse = true;
            modelLocator.normalizeToFloor = false;
            #endregion

            #region EntityStateMachine_Body
            var esmBody = bodyPrefab.AddComponent<EntityStateMachine>();
            esmBody.customName = "Body";
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pillar.SpawnState));
            esmBody.mainStateType = bodyInformation.mainState;
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = bodyPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { esmBody };
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = bodyInformation.deathState;
            #endregion

            #region Deployable
            bodyPrefab.AddComponent<Deployable>();
            #endregion

            #endregion

            #region Hurtbox
            var hurtbox = hurtboxTransform.gameObject.AddComponent<HurtBox>();
            hurtbox.healthComponent = healthComponent;
            hurtbox.isBullseye = true;
            #endregion

            #region Model
            var mdlPillar = modelTransform.gameObject;

            #region HurtBoxGroup
            var hurtboxGroup = mdlPillar.AddComponent<HurtBoxGroup>();
            hurtboxGroup.mainHurtBox = hurtbox;
            hurtboxGroup.hurtBoxes = new HurtBox[] { hurtbox };
            #endregion

            #region ChildLocator
            var childLocator = mdlPillar.AddComponent<ChildLocator>();
            var ourChildLocator = mdlPillar.GetComponent<OurChildLocator>();
            childLocator.transformPairs = Array.ConvertAll(ourChildLocator.transformPairs, item =>
            {
                return new ChildLocator.NameTransformPair
                {
                    name = item.name,
                    transform = item.transform,
                };
            });
            UnityEngine.Object.Destroy(ourChildLocator);
            #endregion

            #region CharacterModel
            var characterModel = mdlPillar.AddComponent<CharacterModel>();
            characterModel.body = characterBody;
            characterModel.autoPopulateLightInfos = true;
            var skinnedRenderers = bodyPrefab.GetComponentsInChildren<SkinnedMeshRenderer>();
            characterModel.baseRendererInfos = Array.ConvertAll(skinnedRenderers, (item) =>
            {
                return new CharacterModel.RendererInfo
                {
                    renderer = item,
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                };
            });
            var renderers = bodyPrefab.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = Array.ConvertAll(renderers, (item) =>
            {
                return new CharacterModel.RendererInfo
                {
                    renderer = item,
                    defaultMaterial = item.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                };
            });
            characterModel.baseRendererInfos = ArrayUtils.Join(characterModel.baseRendererInfos, rendererInfos);
            #endregion

            #region DestroyOnUnseen
            mdlPillar.AddComponent<DestroyOnUnseen>().cull = false;
            #endregion

            if (bodyInformation.enableLineRenderer)
            {
                #region LineRenderer
                var linerenderer = mdlPillar.GetComponentInChildren<LineRenderer>();
                linerenderer.material = ContentProvider.GetOrCreateMaterial("matIfritPylonLine", CreateLineRendererMaterial);
                #endregion

                #region LineRendererHelper
                mdlPillar.AddComponent<DeployableLineRendererToOwner>().childOriginName = "LineOriginPoint";
                #endregion
            }
            else
            {
                var linerenderer = mdlPillar.GetComponentInChildren<LineRenderer>();
                UnityEngine.Object.Destroy(linerenderer);
            }

            #region TeamIndicator

            var scaledTransform = bodyPrefab.transform.Find("ModelBase/IfritPillar/ScaledOnInit");
            var osc = scaledTransform.gameObject.AddComponent<ObjectScaleCurve>();
            osc.useOverallCurveOnly = false;
            osc.resetOnAwake = true;
            osc.useUnscaledTime = false;
            osc.timeMax = 3f;
            osc.curveX = acdLookup["acdLinearCurve"].curve;
            osc.curveY = acdLookup["acdLinearCurve"].curve;
            osc.curveZ = acdLookup["acdLinearCurve"].curve;
            osc.overallCurve = acdLookup["acdTeamIndicatorOverallCurve"].curve;

            var indicatorObject = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, FullSphere.prefab").WaitForCompletion());
            indicatorObject.GetComponent<TeamAreaIndicator>().teamComponent = teamComponent;
            indicatorObject.transform.parent = scaledTransform;
            var teamIndicatorScale = 18.75f * (bodyInformation.explosionRadius / 30f);
            indicatorObject.transform.localScale = new Vector3(teamIndicatorScale, teamIndicatorScale, teamIndicatorScale); // 18.75 is equal to 30 explosion radius after rescaling the model
            indicatorObject.transform.localPosition = Vector3.zero;
            indicatorObject.transform.localRotation = Quaternion.identity;

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "TeamAreaIndicator", transform = indicatorObject.transform });
            #endregion

            #region LanternFire
            var lanternFire = bodyPrefab.transform.Find("ModelBase/IfritPillar/IfritPillarArmture/MainPillar/Chain1.1/Lantern/Fire");
            lanternFire.gameObject.GetComponent<Renderer>().material = ContentProvider.GetOrCreateMaterial("matIfritLanternFire", CreateLanternFireMaterial); ;
            #endregion

            #region ModelPanelParameters
            var modelPanelParameters = mdlPillar.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focusPoint;
            modelPanelParameters.cameraPositionTransform = cameraPosition;
            modelPanelParameters.modelRotation = new Quaternion(0, 0, 0, 1);
            modelPanelParameters.minDistance = 15f;
            modelPanelParameters.maxDistance = 50f;
            #endregion

            #endregion

            #region Fireball
            var fire = fireball.Find("Fire");
            var fireParticleSystem = fire.gameObject.GetComponent<ParticleSystem>();
            var fireRenderer = fireParticleSystem.GetComponent<Renderer>();
            fireRenderer.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniExplosion1.mat").WaitForCompletion();

            var fireLightTransform = fireball.Find("Light");
            var fireFlickerLight = fireLightTransform.gameObject.AddComponent<FlickerLight>();
            fireFlickerLight.light = fireLightTransform.gameObject.GetComponent<Light>();
            fireFlickerLight.sinWaves = new Wave[]
            {
                new Wave
                {
                    amplitude = 0.1f,
                    frequency = 4,
                    cycleOffset = 1.23f
                },
                new Wave
                {
                    amplitude = 0.2f,
                    frequency = 3,
                    cycleOffset = 1.34f
                },
                new Wave
                {
                    amplitude = 0.2f,
                    frequency = 5,
                    cycleOffset = 0f
                },
            };

            var fireLightIntencityCurve = fireLightTransform.gameObject.AddComponent<LightIntensityCurve>();
            fireLightIntencityCurve.curve = acdLookup["adcIfritPylonLightIntencityCurve"].curve;
            fireLightIntencityCurve.timeMax = EnemiesReturns.Configuration.Ifrit.PillarExplosionRadius.Value;
            #endregion

            #region AimAssist
            var aimAssistTarget = bodyPrefab.transform.Find("ModelBase/IfritPillar/AimAssist").gameObject.AddComponent<AimAssistTarget>();
            aimAssistTarget.point0 = bodyPrefab.transform.Find("ModelBase/IfritPillar/IfritPillarArmture/TopTransform");
            aimAssistTarget.point1 = bodyPrefab.transform.Find("ModelBase/IfritPillar/IfritPillarArmture/BaseTransform");
            aimAssistTarget.assistScale = 1f;
            aimAssistTarget.healthComponent = healthComponent;
            aimAssistTarget.teamComponent = teamComponent;
            #endregion

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public Material CreateLanternFireMaterial()
        {
            Material material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC2/helminthroost/Assets/matHRFireStaticRedLArge.mat").WaitForCompletion());
            material.name = "matIfritLanternFire";
            material.SetFloat("_DepthOffset", -10f);

            return material;
        }

        public Material CreateLineRendererMaterial()
        {
            var lineMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Captain/matCaptainAirstrikeAltLaser.mat").WaitForCompletion());
            lineMaterial.name = "matIfritPylonLine";
            lineMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampCaptainAirstrike.png").WaitForCompletion());
            lineMaterial.SetColor("_TintColor", new Color(255f / 255f, 53f / 255f, 0f));
            lineMaterial.SetFloat("_Boost", 7.315614f);
            lineMaterial.SetFloat("_AlphaBoost", 5.603551f);
            lineMaterial.SetFloat("_AlphaBias", 0f);
            lineMaterial.SetFloat("_DistortionStrength", 1f);
            lineMaterial.SetVector("_CutoffScroll", new Vector4(5f, 0f, 0f, 0f));

            return lineMaterial;
        }

        public GameObject CreateMaster(GameObject masterPrefab, GameObject bodyPrefab)
        {
            #region NetworkIdentity
            masterPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterMaster
            var characterMaster = masterPrefab.AddComponent<CharacterMaster>();
            characterMaster.bodyPrefab = bodyPrefab;
            characterMaster.spawnOnStart = false;
            characterMaster.teamIndex = TeamIndex.Monster;
            characterMaster.destroyOnBodyDeath = true;
            characterMaster.isBoss = false;
            characterMaster.preventGameOver = true;
            #endregion

            #region Inventory
            masterPrefab.AddComponent<Inventory>();
            #endregion

            #region MinionOwnership
            if (!masterPrefab.TryGetComponent<MinionOwnership>(out _))
            {
                masterPrefab.AddComponent<MinionOwnership>();
            }
            #endregion

            #region EntityStateMachineAI
            var esmAI = masterPrefab.AddComponent<EntityStateMachine>();
            esmAI.customName = "AI";
            esmAI.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Guard));
            esmAI.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Guard));
            #endregion

            #region BaseAI
            var baseAI = masterPrefab.AddComponent<BaseAI>();
            baseAI.fullVision = false;
            baseAI.neverRetaliateFriendlies = true;
            baseAI.enemyAttentionDuration = 5f;
            baseAI.desiredSpawnNodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            baseAI.stateMachine = esmAI;
            baseAI.scanState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Guard));
            baseAI.isHealer = false;
            baseAI.enemyAttention = 0f;
            baseAI.aimVectorDampTime = 0.05f;
            baseAI.aimVectorMaxSpeed = 180f;
            #endregion

            #region AIOwnership
            masterPrefab.AddComponent<AIOwnership>();
            #endregion

            masterPrefab.RegisterNetworkPrefab();

            return masterPrefab;
        }

        public GameObject CreateExplosionEffect()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBoss/OmniExplosionVFXRoboBallBossDeath.prefab").WaitForCompletion().InstantiateClone("IfritPylonExplosionEffect", false);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<OmniEffect>());

            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
            }

            return gameObject;
        }

        public GameObject CreateExlosionEffectAlt()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClayBoss/ClayBossDeath.prefab").WaitForCompletion().InstantiateClone("IfritPylonExplosionEffectAlt", false);
            gameObject.GetComponent<EffectComponent>().applyScale = true;

            var scale = gameObject.transform.localScale.x * (EnemiesReturns.Configuration.Ifrit.PillarExplosionRadius.Value / 30f);
            gameObject.transform.localScale = new Vector3(scale, scale, scale);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<AwakeEvent>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<DelayedEvent>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Corpse>());

            UnityEngine.Object.DestroyImmediate(gameObject.transform.Find("mdlClayBossShattered").gameObject);

            UnityEngine.Object.DestroyImmediate(gameObject.transform.Find("Particles/Goo").gameObject);

            return gameObject;
        }

        public GameObject CreateSpawnEffect()
        {
            var cloneEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanSpawnEffect.prefab").WaitForCompletion().InstantiateClone("IfritPillarSpawnEffect", false);

            var shakeEmitter = cloneEffect.GetComponent<ShakeEmitter>();
            shakeEmitter.duration = 3f;
            shakeEmitter.radius = 10f;

            var components = cloneEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            var light = cloneEffect.GetComponentInChildren<Light>();
            if (light)
            {
                light.range = 5f;
            }

            cloneEffect.transform.localScale = new Vector3(1f, 1f, 1f);

            return cloneEffect;
        }

        public GameObject CreateDeathFallEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("IfritPillarFallEffect", false);

            UnityEngine.Object.DestroyImmediate(clonedEffect.GetComponent<ShakeEmitter>());

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            }

            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Large").gameObject);
            UnityEngine.Object.DestroyImmediate(clonedEffect.transform.Find("ParticleInitial/Spikes, Small").gameObject);

            clonedEffect.transform.localScale = new Vector3(1f, 1f, 1f);

            return clonedEffect;
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            var card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            (card as ScriptableObject).name = name;
            card.prefab = master;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Golem;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.directorCreditCost = 0;
            card.occupyPosition = true;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.noElites = true;
            card.forbiddenAsBoss = true;
            if (skin && bodyGameObject && bodyGameObject.TryGetComponent<CharacterBody>(out var body))
            {
                card.loadout = new SerializableLoadout
                {
                    bodyLoadouts = new SerializableLoadout.BodyLoadout[]
                    {
                        new SerializableLoadout.BodyLoadout()
                        {
                            body = body,
                            skinChoice = skin,
                            skillChoices = Array.Empty<SerializableLoadout.BodyLoadout.SkillChoice>() // yes, we need it
						}
                    }
                };
            };

            return card;
        }
    }
}
