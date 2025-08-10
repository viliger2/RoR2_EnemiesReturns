﻿using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Colossus
{
    public class ColossusBody : BodyBase
    {
        #region InternalConfigs

        public const float MAX_BARRAGE_EMISSION = 7f;

        public const float MAX_EYE_LIGHT_RANGE = 15f;

        public const float MAX_SPOT_LIGHT_RANGE = 70f;

        public const float NORMAL_EYE_LIGHT_RANGE = 6f;

        #endregion

        public struct Skills
        {
            public static SkillDef Stomp;
            public static SkillDef StoneClap;
            public static SkillDef LaserBarrage;
            public static SkillDef HeadLaser;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
            public static SkinDef Snowy;
            public static SkinDef Sandy;
            public static SkinDef Grassy;
            public static SkinDef SkyMeadow;
            public static SkinDef Castle;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
            public static SkillFamily Secondary;
            public static SkillFamily Utility;
            public static SkillFamily Special;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscColossusDefault;
            public static CharacterSpawnCard cscColossusSandy;
            public static CharacterSpawnCard cscColossusSnowy;
            public static CharacterSpawnCard cscColossusGrassy;
            public static CharacterSpawnCard cscColossusSkyMeadow;
            public static CharacterSpawnCard cscColossusCastle;
        }

        public static GameObject BodyPrefab;

        protected override bool AddHitBoxes => true;
        protected override bool AddSetStateOnHurt => false;
        protected override bool AddCrouchMecanim => true;

        public SkillDef CreateStoneClapSkill()
        {
            var commandoBarrage = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyBarrage.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("ColossusBodyStoneClap", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Colossus.RockClap.RockClapStart)))
            {
                nameToken = "ENEMIES_RETURNS_COLOSSUS_STONE_CLAP_NAME",
                descriptionToken = "ENEMIES_RETURNS_COLOSSUS_STONE_CLAP_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.Colossus.RockClapCooldown.Value,
                icon = commandoBarrage.icon
            });
        }

        public SkillDef CreateStompSkill()
        {
            var loaderGroundSlam = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/GroundSlam.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("ColossusBodyStomp", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Colossus.Stomp.StompEnter)))
            {
                nameToken = "ENEMIES_RETURNS_COLOSSUS_STOMP_NAME",
                descriptionToken = "ENEMIES_RETURNS_COLOSSUS_STOMP_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.Colossus.StompCooldown.Value,
                icon = loaderGroundSlam.icon
            });
        }

        public SkillDef CreateLaserBarrageSkill()
        {
            var captainShotgun = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CaptainShotgun.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("ColossusBodyLaserBarrage", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Colossus.HeadLaserBarrage.HeadLaserBarrageStart)))
            {
                nameToken = "ENEMIES_RETURNS_COLOSSUS_LASER_BARRAGE_NAME",
                descriptionToken = "ENEMIES_RETURNS_COLOSSUS_LASER_BARRAGE_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.Colossus.LaserBarrageCooldown.Value,
                icon = captainShotgun.icon
            });
        }

        public SkillDef CreateHeadLaserSkill()
        {
            var voidFiend = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/FireCorruptBeam.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("ColossusBodyHeadLaser", new EntityStates.SerializableEntityStateType(typeof(Junk.ModdedEntityStates.Colossus.HeadLaser.HeadLaserStart)))
            {
                nameToken = "ENEMIES_RETURNS_COLOSSUS_HEAD_LASER_NAME",
                descriptionToken = "ENEMIES_RETURNS_COLOSSUS_HEAD_LASER_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 45f,
                icon = voidFiend.icon
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.Colossus.DirectorCost.Value)
            {
                hullSize = HullClassification.BeetleQueen,
                occupyPosition = true,
                skinDef = skin,
                bodyPrefab = bodyPrefab,
            });
        }

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, ExplicitPickupDropTable droptable)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, log, droptable);

            body.AddComponent<ColossusAwooga>();

            #region SettingParticleMaterials
            var trails = body.transform.Find("ModelBase/mdlColossus/Armature/root/root_pelvis_control/spine/spine.001/head/LaserChargeParticles/Trails");
            trails.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matColossusLaserBarrageChargeTrails", CreateLaserBarrageChargeTrailsMaterial);

            var distortion = body.transform.Find("ModelBase/mdlColossus/Armature/root/root_pelvis_control/spine/spine.001/head/LaserChargeParticles/Distortion");
            distortion.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matInverseDistortion.mat").WaitForCompletion();
            #endregion

            var rocksInitialTransform = body.transform.Find("ModelBase/mdlColossus/Points/RocksSpawnPoint");

            var mdlColossus = body.transform.Find("ModelBase/" + ModelName()).gameObject;
            mdlColossus.AddComponent<FloatingRocksController>().initialPosition = rocksInitialTransform;

            //var helper = body.AddComponent<WalkSpeedDebugHelper>();
            //helper.animator = body.transform.Find("ModelBase/mdlColossus").GetComponent<Animator>();
            //helper.animationParameters = new string[] { "walkSpeedDebug" };

            return body;
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Colossus.Death.InitialDeathState)));
        }

        protected override IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return new IHitboxes.HitBoxesParams[]
            {
                new IHitboxes.HitBoxesParams
                {
                    groupName = "LeftStomp",
                    pathsToTransforms = new string[] { "Armature/foot.l/LeftStompHitbox" }
                },
                new IHitboxes.HitBoxesParams
                {
                    groupName = "RightStomp",
                    pathsToTransforms = new string[] { "Armature/foot.r/RightStompHitbox" }
                }
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdGolem.asset").WaitForCompletion();

        protected override string ModelName() => "mdlColossus";

        protected override float CharacterDirectionTurnSpeed => 90f;

        protected override float MaxInteractionDistance => 8f;

        protected override CharacterCameraParams CharacterCameraParams()
        {
            return Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardRaidboss.asset").WaitForCompletion();
        }

        protected override IModelLocator.ModelLocatorParams ModelLocatorParams()
        {
            return new IModelLocator.ModelLocatorParams()
            {
                dontReleaseModelOnDeath = EnemiesReturns.Configuration.Colossus.DestroyModelOnDeath.Value
            };
        }

        protected override IAimAnimator.AimAnimatorParams AimAnimatorParams()
        {
            return new IAimAnimator.AimAnimatorParams()
            {
                pitchRangeMin = -70f,
                pitchRangeMax = 70f,

                yawRangeMin = -135f,
                yawRangeMax = 135f,

                pitchGiveUpRange = 50f,
                yawGiveUpRange = 50f,

                giveUpDuration = 5f,

                raisedApproachSpeed = 180f,
                loweredApproachSpeed = 180f,
                smoothTime = 0.3f,
            };
        }

        protected override IKinematicCharacterMotor.KinemacitCharacterMotorParams KinemacitCharacterMotorParams()
        {
            return new IKinematicCharacterMotor.KinemacitCharacterMotorParams
            {
                StableGroundLayers = LayerIndex.world.mask,
                MaxStepHeight = 1f
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_COLOSSUS_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                subtitleNameToken = "ENEMIES_RETURNS_COLOSSUS_BODY_SUBTITLE",
                bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage | CharacterBody.BodyFlags.IgnoreKnockback,
                mainRootSpeed = 7.5f,

                baseMaxHealth = EnemiesReturns.Configuration.Colossus.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.Colossus.BaseMoveSpeed.Value,
                baseAcceleration = 20f,
                baseJumpPower = EnemiesReturns.Configuration.Colossus.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.Colossus.BaseDamage.Value,
                baseArmor = EnemiesReturns.Configuration.Colossus.BaseArmor.Value,

                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.Colossus.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.Colossus.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.Colossus.LevelArmor.Value,

                hullClassification = HullClassification.BeetleQueen,
                bodyColor = new Color(0.36f, 0.36f, 0.44f),
                isChampion = true
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                landingSound = "ER_Colossus_Landing_Play",
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Colossus").gameObject.GetComponent<SkinnedMeshRenderer>();
            var headRenderer = modelPrefab.transform.Find("Colossus Head").gameObject.GetComponent<SkinnedMeshRenderer>();
            var eyeRenderer = modelPrefab.transform.Find("Colossus Eye").gameObject.GetComponent<SkinnedMeshRenderer>();
            var particleRenderer = modelPrefab.transform.Find("Armature/root/root_pelvis_control/spine/spine.001/ParticleSystem").gameObject.GetComponent<ParticleSystemRenderer>();
            var eyeLight = modelPrefab.transform.Find("Armature/root/root_pelvis_control/spine/spine.001/head/Light").gameObject.GetComponent<Light>();

            var renderInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = headRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = eyeRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = false,
                    hideOnDeath = true // only for items, what a waste of time
                }
            };
            var lightInfos = new CharacterModel.LightInfo[]
            {
                new CharacterModel.LightInfo
                {
                    light = eyeLight,
                    defaultColor = eyeLight.color
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = false,
                lightInfos = lightInfos,
                renderInfos = renderInfos,
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Colossus").gameObject.GetComponent<SkinnedMeshRenderer>();
            var headRenderer = modelPrefab.transform.Find("Colossus Head").gameObject.GetComponent<SkinnedMeshRenderer>();
            var eyeRenderer = modelPrefab.transform.Find("Colossus Eye").gameObject.GetComponent<SkinnedMeshRenderer>();
            var particleRenderer = modelPrefab.transform.Find("Armature/root/root_pelvis_control/spine/spine.001/ParticleSystem").gameObject.GetComponent<ParticleSystemRenderer>();
            var flagObject = modelPrefab.transform.Find("Armature/root/root_pelvis_control/spine/spine.001/head/mdlFlag").gameObject;

            var renderInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = headRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = eyeRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = false,
                    hideOnDeath = true // only for items, what a waste of time
                }
            };
            SkinDefs.Default = Utils.CreateSkinDef("skinColossusDefault", modelPrefab, renderInfos);

            CharacterModel.RendererInfo[] snowyRenderer = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSnowy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSnowy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusEye"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };
            SkinDefs.Snowy = Utils.CreateSkinDef("skinColossusSnowy", modelPrefab, snowyRenderer, SkinDefs.Default);

            CharacterModel.RendererInfo[] sandyRenderer = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSandy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSandy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusEye"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };
            SkinDefs.Sandy = Utils.CreateSkinDef("skinColossusSandy", modelPrefab, sandyRenderer, SkinDefs.Default);

            CharacterModel.RendererInfo[] grassyRenderer = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusGrassy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusGrassy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusEye"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };
            SkinDefs.Grassy = Utils.CreateSkinDef("skinColossusGrassy", modelPrefab, grassyRenderer, SkinDefs.Default);

            CharacterModel.RendererInfo[] skyMeadowRenderer = new CharacterModel.RendererInfo[]
{
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSkyMeadow"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSkyMeadow"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusEye"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };
            SkinDefs.SkyMeadow = Utils.CreateSkinDef("skinColossusSkyMeadow", modelPrefab, skyMeadowRenderer, SkinDefs.Default);

            CharacterModel.RendererInfo[] castleRenderer = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSMBBody"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = headRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusSMBHead"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = eyeRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matColossusEye"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = particleRenderer,
                    defaultMaterial = particleRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };
            SkinDefs.Castle = Utils.CreateSkinDef("skinColossusCastle", modelPrefab, castleRenderer, SkinDefs.Default, new GameObject[] { flagObject });

            return new SkinDef[] { SkinDefs.Default, SkinDefs.Castle, SkinDefs.Snowy, SkinDefs.Sandy, SkinDefs.Grassy, SkinDefs.SkyMeadow };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Colossus.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Colossus.ColossusMain))
                },
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Weapon",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle))
                }
            };
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            return new IFootStepHandler.FootstepHandlerParams
            {
                enableFootstepDust = true,
                baseFootstepString = "ER_Colossus_Step_Play",
                footstepDustPrefab = CreateColossusStepEffect()
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "Stomp", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "StoneClap", SkillSlot.Secondary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Utility, "LaserBarrage", SkillSlot.Utility),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Special, "HeadLaser", SkillSlot.Special),
            };
        }

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 15f,
                maxDistance = 50f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        private GameObject CreateColossusStepEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion().InstantiateClone("ColossusFootstepDust", false);

            var components = clonedEffect.GetComponentsInChildren<ParticleSystem>();
            foreach (var component in components)
            {
                var main = component.main;
                main.startSize = new ParticleSystem.MinMaxCurve(15f, 18f);
            }

            return clonedEffect;
        }

        public Material CreateLaserBarrageChargeTrailsMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion());
            material.name = "matColossusLaserBarrageChargeTrails";
            material.SetColor("_TintColor", new Color(1f, 0f, 0f));
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texRampImp2.png").WaitForCompletion());

            return material;
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 8f,
                pathToPoint0 = "ModelBase/mdlColossus/Armature/root/root_pelvis_control/spine/spine.001/head",
                pathToPoint1 = "ModelBase/mdlColossus/Armature/root"
            };
        }

        protected override ICrouchMecanim.CrouchMecanimParams CrouchMecanimParams()
        {
            return new ICrouchMecanim.CrouchMecanimParams
            {
                duckHeight = 25f,
                smoothdamp = 0.3f,
                initialverticalOffset = 0f
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsColossus";
            #region FireElite
            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(0.00133F, 0.11172F, 0.00157F),
                localAngles = new Vector3(20.60016F, 340F, 359.7185F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(0.00095F, 0.07965F, 0.00112F),
                localAngles = new Vector3(24.60459F, 24.28895F, 2.60649F),
                localScale = new Vector3(-0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteFire.EliteFireEquipment),
                displayRuleGroup = displayRuleGroupFire,
            });
            #endregion

            #region HauntedElite
            var displayRuleGroupHaunted = new DisplayRuleGroup();
            displayRuleGroupHaunted.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteHaunted.DisplayEliteStealthCrown),
                childName = "Chest",
                localPos = new Vector3(-0.01152F, -0.32538F, -0.02099F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.16025F, 0.11155F, 0.21315F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupHaunted,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteHaunted.EliteHauntedEquipment),
            });
            #endregion

            #region IceElite
            var displayRuleGroupIce = new DisplayRuleGroup();
            displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteIce.DisplayEliteIceCrown),
                childName = "Head",
                localPos = new Vector3(-0.02513F, 0.56141F, -0.269F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupIce,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteIce.EliteIceEquipment)
            });
            #endregion

            #region LightningElite
            var displayRuleGroupLightning = new DisplayRuleGroup();
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "Head",
                localPos = new Vector3(-0.02181F, 0.46209F, 0.10716F),
                localAngles = new Vector3(292.6887F, 3.59969F, 181.911F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "Head",
                localPos = new Vector3(-0.12685F, 0.47042F, -0.11722F),
                localAngles = new Vector3(288.6145F, 259.8474F, 143.9413F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "Head",
                localPos = new Vector3(0.10414F, 0.45989F, -0.09844F),
                localAngles = new Vector3(287.5787F, 127.0641F, 171.7998F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLightning,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteLightning.EliteLightningEquipment)
            });
            #endregion

            #region LunarElite
            var displayRuleGroupLunar = new DisplayRuleGroup();
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarEye),
                childName = "Head",
                localPos = new Vector3(0.00503F, 0.42051F, -0.06089F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.47317F, 0.47317F, 0.47317F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "Chest",
                localPos = new Vector3(0F, 0.00002F, -0.37656F),
                localAngles = new Vector3(-0.00001F, 180F, 180F),
                localScale = new Vector3(0.22275F, 0.22275F, 0.22275F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLunar,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteLunar.EliteLunarEquipment)
            });
            #endregion

            #region PoisonElite
            var displayRuleGroupPoison = new DisplayRuleGroup();
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "ShoulderR",
                localPos = new Vector3(0.05337F, 0.29063F, -0.02488F),
                localAngles = new Vector3(318.2777F, 269.6648F, 89.31491F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "ShoulderL",
                localPos = new Vector3(-0.0039F, 0.27819F, 0.00551F),
                localAngles = new Vector3(309.1658F, 90.22986F, 270.5801F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "ThighR",
                localPos = new Vector3(0.00182F, 0.49292F, 0.15797F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.05F, 0.05F, 0.05F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "ThighL",
                localPos = new Vector3(-0.00176F, 0.47607F, 0.14286F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.05F, 0.05F, 0.05F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupPoison,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.ElitePoison.ElitePoisonEquipment)
            });
            #endregion

            #region EliteEarth
            var displayRuleGroupEarth = new DisplayRuleGroup();
            displayRuleGroupEarth.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteEarth.DisplayEliteMendingAntlers),
                childName = "Head",
                localPos = new Vector3(-0.00503F, 0.35341F, -0.06072F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1.1449F, 1.1449F, 1.1449F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupEarth,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteEarth.EliteEarthEquipment)
            });
            #endregion

            #region VoidElite
            var displayRuleGroupVoid = new DisplayRuleGroup();
            displayRuleGroupVoid.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteVoid.DisplayAffixVoid),
                childName = "Chest",
                localPos = new Vector3(0F, -0.10164F, 0.24164F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.35F, 0.35F, 0.35F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupVoid,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteVoid.EliteVoidEquipment)
            });
            #endregion

            #region BeadElite
            var displayRuleGroupBead = new DisplayRuleGroup();
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "Head",
                localPos = new Vector3(-0.03787F, 0.48882F, -0.059F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.03455F, 0.025F, 0.03455F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "ShoulderR",
                localPos = new Vector3(0.0042F, 0.32214F, -0.06562F),
                localAngles = new Vector3(0.55693F, 359.2249F, 48.48366F),
                localScale = new Vector3(0.04197F, 0.03F, 0.04197F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "ShoulderL",
                localPos = new Vector3(-0.00393F, 0.28007F, 0.00555F),
                localAngles = new Vector3(0.24913F, 0.51682F, 331.4208F),
                localScale = new Vector3(0.041F, 0.03F, 0.041F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupBead,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(RoR2BepInExPack.GameAssetPaths.RoR2_DLC2_Elites_EliteBead.EliteBeadEquipment_asset)
            });
            #endregion

            #region GoldElite
            var displayRuleGroupGold = new DisplayRuleGroup();
            displayRuleGroupGold.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteGold.DisplayEliteAurelioniteEquipment),
                childName = "Chest",
                localPos = new Vector3(0F, -0.66501F, 0.23605F),
                localAngles = new Vector3(12.27343F, 0F, -0.00001F),
                localScale = new Vector3(0.99081F, 1.07413F, 1.17694F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupGold,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteGold.EliteAurelioniteEquipment)
            });
            #endregion

            #region AeonianElite
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                var displayRuleGroupAeonian = new DisplayRuleGroup();
                displayRuleGroupAeonian.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.02513F, 0.56141F, -0.269F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.05664F, 0.07367F, 0.07367F),
                    limbMask = LimbFlags.None
                });

                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupAeonian,
                    keyAsset = Content.Equipment.EliteAeonian
                });
            }
            #endregion

            #region PartyHat
            if (Items.PartyHat.PartyHatFactory.ShouldThrowParty())
            {
                var displayRuleGroupPartyHat = new DisplayRuleGroup();
                displayRuleGroupPartyHat.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Items.PartyHat.PartyHatFactory.PartyHatDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(-0.01372F, 0.46715F, -0.04329F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.23833F, 0.23833F, 0.23833F),
                    limbMask = LimbFlags.None
                });
                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupPartyHat,
                    keyAsset = Content.Items.PartyHat
                });
            }
            #endregion

            return idrs;
        }

    }
}
