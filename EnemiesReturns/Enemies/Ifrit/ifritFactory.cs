using RoR2.Networking;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using KinematicCharacterController;
using System.Linq;
using RoR2.CharacterAI;
using EnemiesReturns.EditorHelpers;
using RoR2.Skills;
using RoR2.Projectile;
using HG;
using EntityStates;
using static RoR2.ItemDisplayRuleSet;
using EnemiesReturns.Projectiles;
using ThreeEyedGames;
using static EnemiesReturns.Utils;
using RoR2.Mecanim;
using EnemiesReturns.PrefabAPICompat;
using EnemiesReturns.Helpers;
using RoR2.EntityLogic;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritFactory
    {
        public struct Skills
        {
            public static SkillDef SummonPylon;

            public static SkillDef Hellzone;
            //public static SkillDef Stomp;

            //public static SkillDef StoneClap;

            //public static SkillDef LaserBarrage;

            //public static SkillDef HeadLaser;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Special;

            public static SkillFamily Secondary;

            //public static SkillFamily Primary;

            //public static SkillFamily Utility;

            //public static SkillFamily Special;
        }

        public struct SkinDefs
        {
            //public static SkinDef Default;
            //public static SkinDef Snowy;
            //public static SkinDef Sandy;
            //public static SkinDef Grassy;
            //public static SkinDef SkyMeadow;
            //public static SkinDef Castle;
        }

        public struct SpawnCards
        {
            //public static CharacterSpawnCard cscColossusDefault;

            //public static CharacterSpawnCard cscColossusSandy;

            //public static CharacterSpawnCard cscColossusSnowy;

            //public static CharacterSpawnCard cscColossusGrassy;

            //public static CharacterSpawnCard cscColossusSkyMeadow;

            //public static CharacterSpawnCard cscColossusCastle;
        }

        public static GameObject IfritBody;

        public static GameObject IfritMaster;

        public static DeployableSlot PylonDeployable;

        public GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, Dictionary<string, Material> skinsLookup, ExplicitPickupDropTable droptable)
        {
            var aimOrigin = bodyPrefab.transform.Find("AimOrigin");
            var cameraPivot = bodyPrefab.transform.Find("CameraPivot");
            var modelTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit");
            var modelBase = bodyPrefab.transform.Find("ModelBase");

            var focusPoint = bodyPrefab.transform.Find("ModelBase/mdlIfrit/LogBookTarget");
            var cameraPosition = bodyPrefab.transform.Find("ModelBase/mdlIfrit/LogBookTarget/LogBookCamera");

            var modelRenderer = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Ifrit").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maneRenderer = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Flame mane").gameObject.GetComponent<SkinnedMeshRenderer>();
            var beardRenderer = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Flame beard").gameObject.GetComponent<SkinnedMeshRenderer>();

            var headTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Spine/Spine.001/Neck/Head");
            var rootTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root");

            #region IfritBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterDirection
            var characterDirection = bodyPrefab.AddComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBase;
            characterDirection.turnSpeed = 100f;
            #endregion

            #region CharacterMotor
            var characterMotor = bodyPrefab.AddComponent<CharacterMotor>();
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 500f;
            characterMotor.airControl = 0.25f;
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.generateParametersOnAwake = true;
            #endregion

            #region InputBankTest
            var inputBank = bodyPrefab.AddComponent<InputBankTest>();
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!bodyPrefab.TryGetComponent(out characterBody))
            {
                characterBody = bodyPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ENEMIES_RETURNS_IFRIT_BODY_NAME";
            characterBody.subtitleNameToken = "ENEMIES_RETURNS_IFRIT_BODY_SUBTITLE";
            characterBody.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
            characterBody.rootMotionInMainState = false;
            characterBody.mainRootSpeed = 7.5f;

            characterBody.baseMaxHealth = EnemiesReturnsConfiguration.Ifrit.BaseMaxHealth.Value;
            characterBody.baseRegen = 0f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = EnemiesReturnsConfiguration.Ifrit.BaseMoveSpeed.Value;
            characterBody.baseAcceleration = 20f;
            characterBody.baseJumpPower = EnemiesReturnsConfiguration.Ifrit.BaseJumpPower.Value;
            characterBody.baseDamage = EnemiesReturnsConfiguration.Ifrit.BaseDamage.Value;
            characterBody.baseAttackSpeed = 1f;
            characterBody.baseCrit = 0f;
            characterBody.baseArmor = EnemiesReturnsConfiguration.Ifrit.BaseArmor.Value;
            characterBody.baseVisionDistance = float.PositiveInfinity;
            characterBody.baseJumpCount = 1;
            characterBody.sprintingSpeedMultiplier = 1.45f;

            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturnsConfiguration.Ifrit.LevelMaxHealth.Value;
            characterBody.levelDamage = EnemiesReturnsConfiguration.Ifrit.LevelDamage.Value;
            characterBody.levelArmor = EnemiesReturnsConfiguration.Ifrit.LevelArmor.Value;

            characterBody.wasLucky = false;
            characterBody.spreadBloomDecayTime = 0.45f;
            characterBody._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            characterBody.aimOriginTransform = aimOrigin;
            characterBody.hullClassification = HullClassification.Golem;
            if (sprite != null)
            {
                characterBody.portraitIcon = sprite.texture;
            }
            characterBody.bodyColor = new Color(0.36f, 0.36f, 0.44f);
            characterBody.isChampion = true;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized));
            #endregion

            #region CameraTargetParams
            var cameraTargetParams = bodyPrefab.AddComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardHuge.asset").WaitForCompletion();
            cameraTargetParams.cameraPivotTransform = cameraPivot;
            #endregion

            #region ModelLocator
            var modelLocator = bodyPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBase;

            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = false;

            modelLocator.noCorpse = false;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.preserveModel = false;

            modelLocator.normalizeToFloor = false;
            modelLocator.normalSmoothdampTime = 0.1f;
            modelLocator.normalMaxAngleDelta = 90f;
            #endregion

            #region EntityStateMachineBody
            var esmBody = bodyPrefab.AddComponent<EntityStateMachine>();
            esmBody.customName = "Body";
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.SpawnState));
            esmBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain));
            #endregion

            #region EntityStateMachineWeapon
            var esmWeapon = bodyPrefab.AddComponent<EntityStateMachine>();
            esmWeapon.customName = "Weapon";
            esmWeapon.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            esmWeapon.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            #endregion

            #region GenericSkills

            #region Primary
            //var gsPrimary = bodyPrefab.AddComponent<GenericSkill>();
            //gsPrimary._skillFamily = SkillFamilies.Primary;
            //gsPrimary.skillName = "Stomp";
            //gsPrimary.hideInCharacterSelect = false;
            #endregion

            #region Secondary
            var gsSecondary = bodyPrefab.AddComponent<GenericSkill>();
            gsSecondary._skillFamily = SkillFamilies.Secondary;
            gsSecondary.skillName = "Hellzone";
            gsSecondary.hideInCharacterSelect = false;
            #endregion

            #region Utility
            //var gsUtility = bodyPrefab.AddComponent<GenericSkill>();
            //gsUtility._skillFamily = SkillFamilies.Utility;
            //gsUtility.skillName = "LaserBarrage";
            //gsUtility.hideInCharacterSelect = false;
            #endregion

            #region Special
            var gsSpecial = bodyPrefab.AddComponent<GenericSkill>();
            gsSpecial._skillFamily = SkillFamilies.Special;
            gsSpecial.skillName = "SummonPylon";
            gsSpecial.hideInCharacterSelect = false;
            #endregion

            #endregion

            #region SkillLocator
            SkillLocator skillLocator = null;
            if (!bodyPrefab.TryGetComponent(out skillLocator))
            {
                skillLocator = bodyPrefab.AddComponent<SkillLocator>();
            }
            //skillLocator.primary = gsPrimary;
            skillLocator.secondary = gsSecondary;
            //skillLocator.utility = gsUtility;
            skillLocator.special = gsSpecial;
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!bodyPrefab.TryGetComponent(out teamComponent))
            {
                teamComponent = bodyPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.None;
            #endregion

            #region HealthComponent
            var healthComponent = bodyPrefab.AddComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = false;
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            #endregion

            #region Interactor
            bodyPrefab.AddComponent<Interactor>().maxInteractionDistance = 8f;
            #endregion

            #region InteractionDriver
            bodyPrefab.AddComponent<InteractionDriver>();
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath));
            characterDeathBehavior.idleStateMachine = new EntityStateMachine[] { esmWeapon };
            #endregion

            #region CharacterNetworkTransform
            var characterNetworkTransform = bodyPrefab.AddComponent<CharacterNetworkTransform>();
            characterNetworkTransform.positionTransmitInterval = 0.1f;
            characterNetworkTransform.interpolationFactor = 2f;
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = bodyPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { esmBody, esmWeapon };
            #endregion

            #region DeathRewards
            var deathRewards = bodyPrefab.AddComponent<DeathRewards>();
            deathRewards.logUnlockableDef = log;
            if (droptable) 
            {
                deathRewards.bossDropTable = droptable; 
            }
            #endregion

            #region EquipmentSlot
            bodyPrefab.AddComponent<EquipmentSlot>();
            #endregion

            #region SfxLocator
            var sfxLocator = bodyPrefab.AddComponent<SfxLocator>();
            sfxLocator.deathSound = ""; // each death will have its own sound in animator
            sfxLocator.barkSound = ""; // TODO
            #endregion

            #region KinematicCharacterMotor
            var capsuleCollider = bodyPrefab.GetComponent<CapsuleCollider>();

            var kinematicCharacterMotor = bodyPrefab.AddComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor._attachedRigidbody = bodyPrefab.GetComponent<Rigidbody>();

            // new shit
            kinematicCharacterMotor.StableGroundLayers = LayerIndex.world.mask;
            kinematicCharacterMotor.AllowSteppingWithoutStableGrounding = false;
            kinematicCharacterMotor.LedgeAndDenivelationHandling = true;
            kinematicCharacterMotor.SimulatedCharacterMass = 1f;
            kinematicCharacterMotor.CheckMovementInitialOverlaps = true;
            kinematicCharacterMotor.KillVelocityWhenExceedMaxMovementIterations = true;
            kinematicCharacterMotor.KillRemainingMovementWhenExceedMaxMovementIterations = true;
            kinematicCharacterMotor.DiscreteCollisionEvents = false;
            // end new shit

            kinematicCharacterMotor.CapsuleRadius = capsuleCollider.radius;
            kinematicCharacterMotor.CapsuleHeight = capsuleCollider.height;
            if(capsuleCollider.center != Vector3.zero)
            {
                Log.Error("CapsuleCollider for " + bodyPrefab + " has non-zero center. This WILL result in pathing issues for AI.");
            }
            kinematicCharacterMotor.CapsuleYOffset = 0f;

            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 1f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;

            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;

            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = new Vector3(0f, 0f, 1f);

            kinematicCharacterMotor.StepHandling = StepHandlingMethod.Standard;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            #endregion

            #endregion

            #region SetupBoxes

            var surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdLemurian.asset").WaitForCompletion();

            var hurtBoxesTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "HurtBox").ToArray();
            List<HurtBox> hurtBoxes = new List<HurtBox>();
            foreach (Transform t in hurtBoxesTransform)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            var sniperHurtBoxes = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "SniperHurtBox").ToArray();
            foreach (Transform t in sniperHurtBoxes)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBox.isSniperTarget = true;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            //var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/root/root_pelvis_control/spine/MainHurtBox"); // TODO
            var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Hurtbox"); // TODO
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            #endregion

            #region mdlIfrit
            var mdlIfrit = modelTransform.gameObject;
            var animator = modelTransform.gameObject.GetComponent<Animator>();

            #region AimAnimator
            // if you are having issues with AimAnimator,
            // * just add Additive Reference Pose for your pitch and yaw animations in the middle of the animation
            // * make both animations loop
            // * set them both to zero speed in your animation controller
            // * I haven't found how to add poses to "separate" animation files, so those have to be in fbx
            var aimAnimator = mdlIfrit.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBank;
            aimAnimator.directionComponent = characterDirection;

            aimAnimator.pitchRangeMin = -70f; // its looking up, not down, for fuck sake
            aimAnimator.pitchRangeMax = 70f;

            aimAnimator.yawRangeMin = -135f;
            aimAnimator.yawRangeMax = 135f;

            aimAnimator.pitchGiveupRange = 50f;
            aimAnimator.yawGiveupRange = 50f;

            aimAnimator.giveupDuration = 5f;

            aimAnimator.raisedApproachSpeed = 180f;
            aimAnimator.loweredApproachSpeed = 180f;
            aimAnimator.smoothTime = 0.3f;

            aimAnimator.fullYaw = false;
            aimAnimator.aimType = AimAnimator.AimType.Direct;

            aimAnimator.enableAimWeight = false;
            aimAnimator.UseTransformedAimVector = false;
            #endregion

            #region ChildLocator
            var childLocator = mdlIfrit.AddComponent<ChildLocator>();
            var ourChildLocator = mdlIfrit.GetComponent<OurChildLocator>();
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

            #region HurtBoxGroup
            var hurtboxGroup = mdlIfrit.AddComponent<HurtBoxGroup>();
            hurtboxGroup.hurtBoxes = hurtBoxes.ToArray();
            for (short i = 0; i < hurtboxGroup.hurtBoxes.Length; i++)
            {
                hurtboxGroup.hurtBoxes[i].hurtBoxGroup = hurtboxGroup;
                hurtboxGroup.hurtBoxes[i].indexInGroup = i;
                if (hurtboxGroup.hurtBoxes[i].isBullseye)
                {
                    hurtboxGroup.bullseyeCount++;
                }
            }
            hurtboxGroup.mainHurtBox = mainHurtBox;
            #endregion

            #region AnimationEvents
            if (!mdlIfrit.TryGetComponent<AnimationEvents>(out _))
            {
                mdlIfrit.AddComponent<AnimationEvents>();
            }
            #endregion

            #region DestroyOnUnseen
            mdlIfrit.AddComponent<DestroyOnUnseen>().cull = false;
            #endregion

            #region CharacterModel
            //modelRenderer.material = skinsLookup["matIfrit"];
            //headRenderer.material = skinsLookup["matColossus"];

            var characterModel = mdlIfrit.AddComponent<CharacterModel>();
            characterModel.body = characterBody;
            characterModel.itemDisplayRuleSet = CreateItemDisplayRuleSet();
            characterModel.autoPopulateLightInfos = true;
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
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
                    renderer = maneRenderer,
                    defaultMaterial = maneRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = beardRenderer,
                    defaultMaterial = beardRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                }
            };
            //characterModel.baseLightInfos = new CharacterModel.LightInfo[] // TODO
            //{
            //    new CharacterModel.LightInfo
            //    {
            //        light = eyeLight,
            //        defaultColor = eyeLight.color
            //    }
            //};
            #endregion

            #region HitBoxGroupLeftStomp
            //var leftstompHitbox = mdlIfrit.transform.Find("Armature/foot.l/LeftStompHitbox").gameObject.AddComponent<HitBox>();

            //var hbgLeftStomp = mdlIfrit.AddComponent<HitBoxGroup>();
            //hbgLeftStomp.groupName = "LeftStomp";
            //hbgLeftStomp.hitBoxes = new HitBox[] { leftstompHitbox };
            #endregion

            #region HitBoxGroupLeftStomp
            //var rightStompHitbox = mdlIfrit.transform.Find("Armature/foot.r/RightStompHitbox").gameObject.AddComponent<HitBox>();

            //var hbgRightStomp = mdlIfrit.AddComponent<HitBoxGroup>();
            //hbgRightStomp.groupName = "RightStomp";
            //hbgRightStomp.hitBoxes = new HitBox[] { rightStompHitbox };
            #endregion

            #region FootstepHandler
            FootstepHandler footstepHandler = null;
            if (!mdlIfrit.TryGetComponent(out footstepHandler))
            {
                footstepHandler = mdlIfrit.AddComponent<FootstepHandler>(); // TODO
            }
            footstepHandler.enableFootstepDust = true;
            footstepHandler.baseFootstepString = "Play_lemurian_step";
            footstepHandler.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion();
            #endregion

            #region ModelPanelParameters
            var modelPanelParameters = mdlIfrit.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focusPoint;
            modelPanelParameters.cameraPositionTransform = cameraPosition;
            modelPanelParameters.modelRotation = new Quaternion(0, 0, 0, 1);
            modelPanelParameters.minDistance = 15f;
            modelPanelParameters.maxDistance = 50f;
            #endregion

            #region SkinDefs
//            RenderInfo[] defaultRender = Array.ConvertAll(characterModel.baseRendererInfos, item => new RenderInfo
//            {
//                renderer = (SkinnedMeshRenderer)item.renderer,
//                material = item.defaultMaterial,
//                ignoreOverlays = item.ignoreOverlays

//            });
//            SkinDefs.Default = CreateSkinDef("skinColossusDefault", mdlIfrit, defaultRender);

//            RenderInfo[] snowyRenderer = new RenderInfo[]
//            {
//                new RenderInfo
//                {
//                    renderer = modelRenderer,
//                    material = skinsLookup["matColossusSnowy"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = headRenderer,
//                    material = skinsLookup["matColossusSnowy"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = eyeRenderer,
//                    material = skinsLookup["matColossusEye"],
//                    ignoreOverlays = true
//                }
//            };
//            SkinDefs.Snowy = CreateSkinDef("skinColossusSnowy", mdlIfrit, snowyRenderer, SkinDefs.Default);

//            RenderInfo[] sandyRenderer = new RenderInfo[]
//            {
//                new RenderInfo
//                {
//                    renderer = modelRenderer,
//                    material = skinsLookup["matColossusSandy"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = headRenderer,
//                    material = skinsLookup["matColossusSandy"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = eyeRenderer,
//                    material = skinsLookup["matColossusEye"],
//                    ignoreOverlays = true
//                }
//            };
//            SkinDefs.Sandy = CreateSkinDef("skinColossusSandy", mdlIfrit, sandyRenderer, SkinDefs.Default);

//            RenderInfo[] grassyRenderer = new RenderInfo[]
//{
//                new RenderInfo
//                {
//                    renderer = modelRenderer,
//                    material = skinsLookup["matColossusGrassy"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = headRenderer,
//                    material = skinsLookup["matColossusGrassy"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = eyeRenderer,
//                    material = skinsLookup["matColossusEye"],
//                    ignoreOverlays = true
//                }
//};
//            SkinDefs.Grassy = CreateSkinDef("skinColossusGrassy", mdlIfrit, grassyRenderer, SkinDefs.Default);

//            RenderInfo[] skyMeadowRenderer = new RenderInfo[]
//{
//                new RenderInfo
//                {
//                    renderer = modelRenderer,
//                    material = skinsLookup["matColossusSkyMeadow"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = headRenderer,
//                    material = skinsLookup["matColossusSkyMeadow"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = eyeRenderer,
//                    material = skinsLookup["matColossusEye"],
//                    ignoreOverlays = true
//                }
//};
//            SkinDefs.SkyMeadow = CreateSkinDef("skinColossusSkyMeadow", mdlIfrit, skyMeadowRenderer, SkinDefs.Default);

//            RenderInfo[] castleRenderer = new RenderInfo[]
//{
//                new RenderInfo
//                {
//                    renderer = modelRenderer,
//                    material = skinsLookup["matColossusSMBBody"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = headRenderer,
//                    material = skinsLookup["matColossusSMBHead"],
//                    ignoreOverlays = false
//                },
//                new RenderInfo
//                {
//                    renderer = eyeRenderer,
//                    material = skinsLookup["matColossusEye"],
//                    ignoreOverlays = true
//                }
//};
//            SkinDefs.Castle = CreateSkinDef("skinColossusCastle", mdlIfrit, castleRenderer, SkinDefs.Default, new GameObject[] { flagObject });

            //var modelSkinController = mdlIfrit.AddComponent<ModelSkinController>();
            //modelSkinController.skins = new SkinDef[]
            //{
            //    SkinDefs.Default,
            //    SkinDefs.Snowy,
            //    SkinDefs.Grassy,
            //    SkinDefs.Sandy,
            //    SkinDefs.SkyMeadow,
            //    SkinDefs.Castle
            //};
            #endregion

            //var helper = mdlColossus.AddComponent<AnimationParameterHelper>();
            //helper.animator = animator;
            //helper.animationParameters = new string[] { "walkSpeedDebug" };
            #endregion

            #region AimAssist
            var aimAssistTarget = bodyPrefab.transform.Find("ModelBase/mdlIfrit/AimAssist").gameObject.AddComponent<AimAssistTarget>();
            aimAssistTarget.point0 = headTransform;
            aimAssistTarget.point1 = rootTransform;
            aimAssistTarget.assistScale = 4f;
            aimAssistTarget.healthComponent = healthComponent;
            aimAssistTarget.teamComponent = teamComponent;
            #endregion

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
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

            #region EntityStateMachineAI
            var esmAI = masterPrefab.AddComponent<EntityStateMachine>();
            esmAI.customName = "AI";
            esmAI.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            esmAI.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            #endregion

            #region BaseAI
            var baseAI = masterPrefab.AddComponent<BaseAI>();
            baseAI.fullVision = false;
            baseAI.neverRetaliateFriendlies = true;
            baseAI.enemyAttentionDuration = 5f;
            baseAI.desiredSpawnNodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            baseAI.stateMachine = esmAI;
            baseAI.scanState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            baseAI.isHealer = false;
            baseAI.enemyAttention = 0f;
            baseAI.aimVectorDampTime = 0.05f;
            baseAI.aimVectorMaxSpeed = 180f;
            #endregion

            #region MinionOwnership
            if (!masterPrefab.TryGetComponent<MinionOwnership>(out _))
            {
                masterPrefab.AddComponent<MinionOwnership>();
            }
            #endregion

            #region AISkillDriver_SummonPylon
            var asdClap = masterPrefab.AddComponent<AISkillDriver>();
            asdClap.customName = "SummonPylon";
            asdClap.skillSlot = SkillSlot.Special;

            asdClap.requiredSkill = null;
            asdClap.requireSkillReady = true;
            asdClap.requireEquipmentReady = false;
            asdClap.minUserHealthFraction = float.NegativeInfinity;
            asdClap.maxUserHealthFraction = 0.6f;
            asdClap.minTargetHealthFraction = float.NegativeInfinity;
            asdClap.maxTargetHealthFraction = float.PositiveInfinity;
            asdClap.minDistance = 0f;
            asdClap.maxDistance = float.PositiveInfinity;
            asdClap.selectionRequiresTargetLoS = false;
            asdClap.selectionRequiresOnGround = false;
            asdClap.selectionRequiresAimTarget = false;
            asdClap.maxTimesSelected = -1;

            asdClap.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdClap.activationRequiresTargetLoS = true;
            asdClap.activationRequiresAimTargetLoS = false;
            asdClap.activationRequiresAimConfirmation = false;
            asdClap.movementType = AISkillDriver.MovementType.Stop;
            asdClap.moveInputScale = 1;
            asdClap.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdClap.ignoreNodeGraph = false;
            asdClap.shouldSprint = false;
            asdClap.shouldFireEquipment = false;
            asdClap.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdClap.driverUpdateTimerOverride = -1f;
            asdClap.resetCurrentEnemyOnNextDriverSelection = false;
            asdClap.noRepeat = true;
            asdClap.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_ChaseOffNodeGraph
            var asdChaseOffNodeGraph = masterPrefab.AddComponent<AISkillDriver>();
            asdChaseOffNodeGraph.customName = "ChaseOffNodegraph";
            asdChaseOffNodeGraph.skillSlot = SkillSlot.None;

            asdChaseOffNodeGraph.requiredSkill = null;
            asdChaseOffNodeGraph.requireSkillReady = false;
            asdChaseOffNodeGraph.requireEquipmentReady = false;
            asdChaseOffNodeGraph.minUserHealthFraction = float.NegativeInfinity;
            asdChaseOffNodeGraph.maxUserHealthFraction = float.PositiveInfinity;
            asdChaseOffNodeGraph.minTargetHealthFraction = float.NegativeInfinity;
            asdChaseOffNodeGraph.maxTargetHealthFraction = float.PositiveInfinity;
            asdChaseOffNodeGraph.minDistance = 0f;
            asdChaseOffNodeGraph.maxDistance = 7f;
            asdChaseOffNodeGraph.selectionRequiresTargetLoS = true;
            asdChaseOffNodeGraph.selectionRequiresOnGround = false;
            asdChaseOffNodeGraph.selectionRequiresAimTarget = false;
            asdChaseOffNodeGraph.maxTimesSelected = -1;

            asdChaseOffNodeGraph.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdChaseOffNodeGraph.activationRequiresTargetLoS = false;
            asdChaseOffNodeGraph.activationRequiresAimTargetLoS = false;
            asdChaseOffNodeGraph.activationRequiresAimConfirmation = false;
            asdChaseOffNodeGraph.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdChaseOffNodeGraph.moveInputScale = 1;
            asdChaseOffNodeGraph.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdChaseOffNodeGraph.ignoreNodeGraph = true;
            asdChaseOffNodeGraph.shouldSprint = false;
            asdChaseOffNodeGraph.shouldFireEquipment = false;
            asdChaseOffNodeGraph.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdChaseOffNodeGraph.driverUpdateTimerOverride = -1;
            asdChaseOffNodeGraph.resetCurrentEnemyOnNextDriverSelection = false;
            asdChaseOffNodeGraph.noRepeat = false;
            asdChaseOffNodeGraph.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_PathFromAfar
            var asdPathFromAfar = masterPrefab.AddComponent<AISkillDriver>();
            asdPathFromAfar.customName = "PathFromAfar";
            asdPathFromAfar.skillSlot = SkillSlot.None;

            asdPathFromAfar.requiredSkill = null;
            asdPathFromAfar.requireSkillReady = false;
            asdPathFromAfar.requireEquipmentReady = false;
            asdPathFromAfar.minUserHealthFraction = float.NegativeInfinity;
            asdPathFromAfar.maxUserHealthFraction = float.PositiveInfinity;
            asdPathFromAfar.minTargetHealthFraction = float.NegativeInfinity;
            asdPathFromAfar.maxTargetHealthFraction = float.PositiveInfinity;
            asdPathFromAfar.minDistance = 0f;
            asdPathFromAfar.maxDistance = float.PositiveInfinity;
            asdPathFromAfar.selectionRequiresTargetLoS = false;
            asdPathFromAfar.selectionRequiresOnGround = false;
            asdPathFromAfar.selectionRequiresAimTarget = false;
            asdPathFromAfar.maxTimesSelected = -1;

            asdPathFromAfar.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdPathFromAfar.activationRequiresTargetLoS = false;
            asdPathFromAfar.activationRequiresAimTargetLoS = false;
            asdPathFromAfar.activationRequiresAimConfirmation = false;
            asdPathFromAfar.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdPathFromAfar.moveInputScale = 1;
            asdPathFromAfar.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdPathFromAfar.ignoreNodeGraph = false;
            asdPathFromAfar.shouldSprint = false;
            asdPathFromAfar.shouldFireEquipment = false;
            asdPathFromAfar.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdPathFromAfar.driverUpdateTimerOverride = -1;
            asdPathFromAfar.resetCurrentEnemyOnNextDriverSelection = false;
            asdPathFromAfar.noRepeat = false;
            asdPathFromAfar.nextHighPriorityOverride = null;
            #endregion

            masterPrefab.RegisterNetworkPrefab();

            return masterPrefab;
        }

        #region GameObjects
        
        public GameObject CreateHellzoneProjectile(GameObject pillarPrefab, GameObject pillarGhostPrefab)
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("IfritHellzoneProjectile", true);

            var spawnChildrenComponent = gameObject.AddComponent<ProjectileSpawnChildrenInRows>();
            spawnChildrenComponent.radius = 9f; // TODO
            spawnChildrenComponent.numberOfRows = 3; // TODO
            spawnChildrenComponent.childrenDamageCoefficient = 1f; // TODO
            spawnChildrenComponent.delayEachRow = 0.5f; // TODO
            spawnChildrenComponent.childPrefab = CreateHellzonePillarProjectile(pillarPrefab, pillarGhostPrefab);

            return gameObject;
        }

        public GameObject CreateHellzonePillarProjectile(GameObject gameObject, GameObject ghostPrefab)
        {
            var hitboxTransform = gameObject.transform.Find("Hitbox");
            if(!hitboxTransform)
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


            var networkTransform = gameObject.AddComponent<ProjectileNetworkTransform>();
            networkTransform.positionTransmitInterval = 0.03f;
            networkTransform.interpolationFactor = 1f;
            networkTransform.allowClientsideCollision = false;

            var projectileDamage = gameObject.AddComponent<ProjectileDamage>();
            projectileDamage.damageType = (DamageTypeCombo)DamageType.IgniteOnHit;
            projectileDamage.useDotMaxStacksFromAttacker = false;

            gameObject.AddComponent<TeamFilter>();

            var hitboxGroup = gameObject.AddComponent<HitBoxGroup>();
            hitboxGroup.name = "Hitbox";
            hitboxGroup.hitBoxes = new HitBox[] { hitbox };

            var projectileOverlapAttack = gameObject.AddComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.enabled = false;
            projectileOverlapAttack.damageCoefficient = 1f;
            //projectileOverlapAttack.impactEffect = ; // TODO
            projectileOverlapAttack.forceVector = new Vector3(0f, 2400f, 0f);
            projectileOverlapAttack.overlapProcCoefficient = 1f;
            projectileOverlapAttack.maximumOverlapTargets = 100;
            projectileOverlapAttack.fireFrequency = 0.001f;
            projectileOverlapAttack.resetInterval = -1f;

            var projectileSimple = gameObject.AddComponent<ProjectileSimple>();
            projectileSimple.lifetime = 3f; // TODO
            projectileSimple.lifetimeExpiredEffect = null;
            projectileSimple.desiredForwardSpeed = 0f;
            projectileSimple.updateAfterFiring = false;
            projectileSimple.enableVelocityOverLifetime = false;
            projectileSimple.oscillate = false;

            var enabler = gameObject.AddComponent<ComponentStateSwitcher>();
            enabler.enabled = false;
            enabler.delay = 0.5f; // TODO
            enabler.state = true;
            enabler.component = projectileOverlapAttack;

            gameObject.RegisterNetworkPrefab();
            return gameObject;
        }

        public GameObject CreateHellzonePillarProjectileGhost(GameObject gameObject)
        {
            gameObject.AddComponent<ProjectileGhostController>().inheritScaleFromProjectile = true;

            var vfxAttributes = gameObject.AddComponent<VFXAttributes>();
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.Medium;
            vfxAttributes.DoNotPool = true;

            //gameObject.AddComponent<EffectManagerHelper>();

            return gameObject;
        }

        #endregion

        #region SkillDefs

        internal SkillDef CreateSummonPylonSkill()
        {
            var skillDef = ScriptableObject.CreateInstance<SkillDef>();

            (skillDef as ScriptableObject).name = "IfritBodySummonPylon";
            skillDef.skillName = "SummonPylon";

            skillDef.skillNameToken = "ENEMIES_RETURNS_IFRIT_SUMMON_PYLON_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_IFRIT_SUMMON_PYLON_DESCRIPTION";
            //var loaderGroundSlam = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/GroundSlam.asset").WaitForCompletion();
            //if (loaderGroundSlam)
            //{
            //    skillDef.icon = loaderGroundSlam.icon;
            //}

            skillDef.activationStateMachineName = "Body";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.SummonPylon));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = 45f; // TODO
            skillDef.baseMaxStock = 1;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 1;

            skillDef.resetCooldownTimerOnUse = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.dontAllowPastMaxStocks = false;
            skillDef.beginSkillCooldownOnSkillEnd = false;

            skillDef.cancelSprintingOnActivation = true;
            skillDef.forceSprintDuringState = false;
            skillDef.canceledFromSprinting = false;

            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;

            return skillDef;
        }

        internal SkillDef CreateHellzoneSkill()
        {
            var skillDef = ScriptableObject.CreateInstance<SkillDef>();

            (skillDef as ScriptableObject).name = "IfritBodyHellzone";
            skillDef.skillName = "Hellzone";

            skillDef.skillNameToken = "ENEMIES_RETURNS_IFRIT_HELLZONE_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_IFRIT_HELLZONE_DESCRIPTION";
            //var loaderGroundSlam = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/GroundSlam.asset").WaitForCompletion();
            //if (loaderGroundSlam)
            //{
            //    skillDef.icon = loaderGroundSlam.icon;
            //}

            skillDef.activationStateMachineName = "Body";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneStart));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = 10f; // TODO
            skillDef.baseMaxStock = 1;
            skillDef.rechargeStock = 1;
            skillDef.requiredStock = 1;
            skillDef.stockToConsume = 1;

            skillDef.resetCooldownTimerOnUse = false;
            skillDef.fullRestockOnAssign = true;
            skillDef.dontAllowPastMaxStocks = false;
            skillDef.beginSkillCooldownOnSkillEnd = false;

            skillDef.cancelSprintingOnActivation = true;
            skillDef.forceSprintDuringState = false;
            skillDef.canceledFromSprinting = false;

            skillDef.isCombatSkill = true;
            skillDef.mustKeyPress = false;

            return skillDef;
        }

        #endregion

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            var card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            (card as ScriptableObject).name = name;
            card.prefab = master;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.BeetleQueen;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.directorCreditCost = EnemiesReturnsConfiguration.Ifrit.DirectorCost.Value;
            card.occupyPosition = true;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.noElites = false;
            card.forbiddenAsBoss = false;
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

        private ItemDisplayRuleSet CreateItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();

            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Head",
                localPos = new Vector3(0.00133F, 0.11172F, 0.00157F),
                localAngles = new Vector3(20.60016F, 340F, 359.7185F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Head",
                localPos = new Vector3(0.00095F, 0.07965F, 0.00112F),
                localAngles = new Vector3(24.60459F, 24.28895F, 2.60649F),
                localScale = new Vector3(-0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteFire/EliteFireEquipment.asset").WaitForCompletion(),
                displayRuleGroup = displayRuleGroupFire,
            });
            #endregion

            #region HauntedElite
            var displayRuleGroupHaunted = new DisplayRuleGroup();
            displayRuleGroupHaunted.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteHaunted/DisplayEliteStealthCrown.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.01152F, -0.32538F, -0.02099F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.16025F, 0.11155F, 0.21315F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupHaunted,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteHaunted/EliteHauntedEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region IceElite
            var displayRuleGroupIce = new DisplayRuleGroup();
            displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteIce/DisplayEliteIceCrown.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.02513F, 0.56141F, -0.269F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupIce,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteIce/EliteIceEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region LightningElite
            var displayRuleGroupLightning = new DisplayRuleGroup();
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.02181F, 0.46209F, 0.10716F),
                localAngles = new Vector3(292.6887F, 3.59969F, 181.911F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.12685F, 0.47042F, -0.11722F),
                localAngles = new Vector3(288.6145F, 259.8474F, 143.9413F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(0.10414F, 0.45989F, -0.09844F),
                localAngles = new Vector3(287.5787F, 127.0641F, 171.7998F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });


            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLightning,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteLightning/EliteLightningEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region LunarElite
            var displayRuleGroupLunar = new DisplayRuleGroup();
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(0F, 0.00002F, -0.37656F),
                localAngles = new Vector3(-0.00001F, 180F, 180F),
                localScale = new Vector3(0.22275F, 0.22275F, 0.22275F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLunar,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/EliteLunar/EliteLunarEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region PoisonElite
            var displayRuleGroupPoison = new DisplayRuleGroup();
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "ShoulderR",
                localPos = new Vector3(0.05337F, 0.29063F, -0.02488F),
                localAngles = new Vector3(318.2777F, 269.6648F, 89.31491F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "ShoulderL",
                localPos = new Vector3(-0.0039F, 0.27819F, 0.00551F),
                localAngles = new Vector3(309.1658F, 90.22986F, 270.5801F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "ThighR",
                localPos = new Vector3(0.00182F, 0.49292F, 0.15797F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.05F, 0.05F, 0.05F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "ThighL",
                localPos = new Vector3(-0.00176F, 0.47607F, 0.14286F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.05F, 0.05F, 0.05F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupPoison,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/ElitePoison/ElitePoisonEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region EliteEarth
            var displayRuleGroupEarth = new DisplayRuleGroup();
            displayRuleGroupEarth.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/DisplayEliteMendingAntlers.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.00503F, 0.35341F, -0.06072F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1.1449F, 1.1449F, 1.1449F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupEarth,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteEarth/EliteEarthEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region VoidElite
            var displayRuleGroupVoid = new DisplayRuleGroup();
            displayRuleGroupVoid.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteVoid/DisplayAffixVoid.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(0F, -0.10164F, 0.24164F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.35F, 0.35F, 0.35F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupVoid,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteVoid/EliteVoidEquipment.asset").WaitForCompletion()
            });
            #endregion

            return idrs;
        }

        public static void Hooks()
        {
            PylonDeployable = R2API.DeployableAPI.RegisterDeployableSlot(GetPylonCount);
        }

        private static int GetPylonCount(CharacterMaster master, int countMultiplier)
        {
            return 1; // TODO: config
        }

        private void Renamer(GameObject object1)
        {

        }
    }
}

