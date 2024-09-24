using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Helpers;
using EnemiesReturns.PrefabAPICompat;
using HG;
using RoR2;
using RoR2.EntityLogic;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritPylonFactory
    {
        public static SpawnCard scIfritPylon;

        public static GameObject IfritPylonBody;

        public static GameObject IfritPylonMaster;

        public GameObject CreateBody(GameObject bodyPrefab, AnimationCurveDef acdLight)
        {
            Transform modelBase = bodyPrefab.transform.Find("ModelBase");
            Transform modelTransform = bodyPrefab.transform.Find("ModelBase/IfritPillar");
            Transform hurtboxTransform = bodyPrefab.transform.Find("ModelBase/Hurtbox");
            Transform fireball = bodyPrefab.transform.Find("ModelBase/IfritPillar/Fireball");

            #region PylonBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region SkillLocator
            bodyPrefab.AddComponent<SkillLocator>();
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!bodyPrefab.TryGetComponent<TeamComponent>(out teamComponent))
            {
                teamComponent = bodyPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.Monster;
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!bodyPrefab.TryGetComponent<CharacterBody>(out characterBody))
            {
                characterBody = bodyPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ENEMIES_RETURNS_IFRIT_PYLON_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.HasBackstabImmunity;
            characterBody.baseMaxHealth = EnemiesReturnsConfiguration.Ifrit.PillarBodyBaseMaxHealth.Value;
            characterBody.baseDamage = EnemiesReturnsConfiguration.Ifrit.BaseDamage.Value;
            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturnsConfiguration.Ifrit.PillarBodyLevelMaxHealth.Value;
            characterBody.levelDamage = EnemiesReturnsConfiguration.Ifrit.LevelDamage.Value;
            characterBody.hullClassification = HullClassification.Golem;
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
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pylon.ChargingExplosion));
            esmBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Pylon.ChargingExplosion));
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = bodyPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { esmBody };
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath));
            #endregion

            #region Deployable
            bodyPrefab.AddComponent<Deployable>();
            #endregion

            #endregion

            #region Hurtbox
            var hurtbox = hurtboxTransform.gameObject.AddComponent<HurtBox>();
            hurtbox.healthComponent = healthComponent;
            #endregion

            #region Model
            var mdlPylonGameObject = modelTransform.gameObject;

            var lavaFlow = bodyPrefab.transform.Find("ModelBase/IfritPillar/MeshLavaFlow");
            var lavaMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC2/helminthroost/Assets/matHRLava.mat").WaitForCompletion());
            lavaMaterial.name = "matIfritPylonLava";
            lavaMaterial.SetTexture("_EmTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC2/Scorchling/texLavaCrack.png").WaitForCompletion());
            lavaMaterial.SetFloat("_EmPower", 10f);
            ContentProvider.MaterialCache.Add(lavaMaterial);
            lavaFlow.gameObject.GetComponent<MeshRenderer>().material = lavaMaterial;

            #region HurtBoxGroup
            var hurtboxGroup = mdlPylonGameObject.AddComponent<HurtBoxGroup>();
            hurtboxGroup.mainHurtBox = hurtbox;
            hurtboxGroup.hurtBoxes = new HurtBox[] { hurtbox };
            #endregion

            #region ChildLocator
            var childLocator = mdlPylonGameObject.AddComponent<ChildLocator>();
            var ourChildLocator = mdlPylonGameObject.GetComponent<OurChildLocator>();
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
            //modelRenderer.material = skinsLookup["matIfrit"];
            //headRenderer.material = skinsLookup["matColossus"];

            var characterModel = mdlPylonGameObject.AddComponent<CharacterModel>();
            characterModel.body = characterBody;
            characterModel.autoPopulateLightInfos = true;
            // TODO: FIX
            var renderers = bodyPrefab.GetComponentsInChildren<MeshRenderer>();
            characterModel.baseRendererInfos = Array.ConvertAll(renderers, (item) =>
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
            //characterModel.baseLightInfos = new CharacterModel.LightInfo[] // TODO
            //{
            //    new CharacterModel.LightInfo
            //    {
            //        light = eyeLight,
            //        defaultColor = eyeLight.color
            //    }
            //};
            #endregion

            #region LineRenderer
            var linerenderer = mdlPylonGameObject.GetComponent<LineRenderer>();

            var lineMaterial = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Captain/matCaptainAirstrikeAltLaser.mat").WaitForCompletion());
            lineMaterial.name = "matIfritPylonLine";
            lineMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampCaptainAirstrike.png").WaitForCompletion());
            lineMaterial.SetColor("_TintColor", new Color(255f / 255f, 53f / 255f, 0f));
            lineMaterial.SetFloat("_Boost", 7.315614f);
            lineMaterial.SetFloat("_AlphaBoost", 5.603551f);
            lineMaterial.SetFloat("_AlphaBias", 0f);
            lineMaterial.SetFloat("_DistortionStrength", 1f);
            lineMaterial.SetVector("_CutoffScroll", new Vector4(5f, 0f, 0f, 0f));
            ContentProvider.MaterialCache.Add(lineMaterial);
            linerenderer.material = lineMaterial;
            #endregion

            #region LineRendererHelper
            mdlPylonGameObject.AddComponent<DeployableLineRendererToOwner>().childOriginName = "LineOriginPoint";
            #endregion

            #region TeamIndicator
            var indicatorObject = UnityEngine.GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, FullSphere.prefab").WaitForCompletion());
            indicatorObject.GetComponent<TeamAreaIndicator>().teamComponent = teamComponent;
            indicatorObject.transform.parent = modelTransform;
            var teamIndicatorScale = 22f * (EnemiesReturnsConfiguration.Ifrit.PillarExplosionRadius.Value / 30f);
            indicatorObject.transform.localScale = new Vector3(teamIndicatorScale, teamIndicatorScale, teamIndicatorScale); // 22 is equal to 30 explosion radius
            indicatorObject.transform.localPosition = Vector3.zero;
            indicatorObject.transform.localRotation = Quaternion.identity;

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "TeamAreaIndicator", transform = indicatorObject.transform });
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
            fireLightIntencityCurve.curve = acdLight.curve;
            fireLightIntencityCurve.timeMax = EnemiesReturnsConfiguration.Ifrit.PillarExplosionRadius.Value;
            #endregion

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public GameObject CreateExplosionEffect()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBoss/OmniExplosionVFXRoboBallBossDeath.prefab").WaitForCompletion().InstantiateClone("IfritPylonExplosionEffect", false);

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<OmniEffect>());

            foreach(Transform child in gameObject.transform)
            {
                child.gameObject.SetActive(true);
            }

            return gameObject;
        }

        public GameObject CreateExlosionEffectAlt()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClayBoss/ClayBossDeath.prefab").WaitForCompletion().InstantiateClone("IfritPylonExplosionEffectAlt", false);
            gameObject.GetComponent<EffectComponent>().applyScale = true;

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<AwakeEvent>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<DelayedEvent>());
            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<Corpse>());

            UnityEngine.GameObject.DestroyImmediate(gameObject.transform.Find("mdlClayBossShattered").gameObject);

            UnityEngine.GameObject.DestroyImmediate(gameObject.transform.Find("Particles/Goo").gameObject);

            return gameObject;
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

            masterPrefab.RegisterNetworkPrefab();

            return masterPrefab;
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
