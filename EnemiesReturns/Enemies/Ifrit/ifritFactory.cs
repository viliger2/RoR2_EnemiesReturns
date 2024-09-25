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
using RoR2.Audio;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritFactory
    {
        public struct Skills
        {
            public static SkillDef SummonPylon;

            public static SkillDef Hellzone;

            public static SkillDef FlameCharge;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Special;

            public static SkillFamily Secondary;

            public static SkillFamily Utility;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;

        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscIfritDefault;
        }

        public static GameObject IfritBody;

        public static GameObject IfritMaster;

        public static DeployableSlot PylonDeployable;

        public GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, Dictionary<string, Material> materialLookup, ExplicitPickupDropTable droptable)
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
            var tailTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Tail/Tail.001");

            var animator = modelTransform.gameObject.GetComponent<Animator>();

            #region IfritBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterDirection
            var characterDirection = bodyPrefab.AddComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBase;
            characterDirection.turnSpeed = EnemiesReturnsConfiguration.Ifrit.TurnSpeed.Value;
            characterDirection.modelAnimator = animator;
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
            characterBody.mainRootSpeed = 33f;

            characterBody.baseMaxHealth = EnemiesReturnsConfiguration.Ifrit.BaseMaxHealth.Value;
            characterBody.baseRegen = 0f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = EnemiesReturnsConfiguration.Ifrit.BaseMoveSpeed.Value;
            characterBody.baseAcceleration = 60f;
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
            characterBody.bodyColor = new Color(1f, 0.6082f, 0f);
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

            modelLocator.noCorpse = false;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.preserveModel = false;

            modelLocator.normalizeToFloor = true; // TODO: i dont fucking know anymore
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
            var gsUtility = bodyPrefab.AddComponent<GenericSkill>();
            gsUtility._skillFamily = SkillFamilies.Utility;
            gsUtility.skillName = "FlameCharge";
            gsUtility.hideInCharacterSelect = false;
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
            skillLocator.utility = gsUtility;
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
            bodyPrefab.AddComponent<Interactor>().maxInteractionDistance = 4f;
            #endregion

            #region InteractionDriver
            bodyPrefab.AddComponent<InteractionDriver>();
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.DeathState));
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
            sfxLocator.deathSound = ""; // TODO
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

            var hurtBoxesTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "Hurtbox").ToArray();
            List<HurtBox> hurtBoxes = new List<HurtBox>();
            foreach (Transform t in hurtBoxesTransform)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            var sniperHurtBoxes = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "SniperHurtbox").ToArray();
            foreach (Transform t in sniperHurtBoxes)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBox.isSniperTarget = true;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Spine/Spine.001/MainHurtbox"); // TODO
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;

            var flameHitBox = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Spine/Spine.001/Neck/Head/Jaw/FlameChargeHitbox").gameObject.AddComponent<HitBox>();
            var chargeHitbox = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/ChargeHitbox").gameObject.AddComponent<HitBox>();
            #endregion

            #region mdlIfrit
            var mdlIfrit = modelTransform.gameObject;

            #region AimAnimator
            // if you are having issues with AimAnimator,
            // * just add Additive Reference Pose for your pitch and yaw animations in the middle of the animation
            // * make both animations loop
            // * set them both to zero speed in your animation controller
            // * I haven't found how to add poses to "separate" animation files, so those have to be in fbx
            var aimAnimator = mdlIfrit.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBank;
            aimAnimator.directionComponent = characterDirection;
            // TODO: maybe change ranges?
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
            #endregion

            #region HitBoxFlameBreath
            var hbgFlame = mdlIfrit.AddComponent<HitBoxGroup>();
            hbgFlame.groupName = "FlameCharge";
            hbgFlame.hitBoxes = new HitBox[] { flameHitBox };
            #endregion

            #region HitBoxBodyCharge
            var hbgBody = mdlIfrit.AddComponent<HitBoxGroup>();
            hbgBody.groupName = "BodyCharge";
            hbgBody.hitBoxes = new HitBox[] { chargeHitbox };
            #endregion

            #region FootstepHandler
            FootstepHandler footstepHandler = null;
            if (!mdlIfrit.TryGetComponent(out footstepHandler))
            {
                footstepHandler = mdlIfrit.AddComponent<FootstepHandler>();
            }
            footstepHandler.enableFootstepDust = true;
            footstepHandler.baseFootstepString = "Play_beetle_queen_step";
            footstepHandler.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion();
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
            RenderInfo[] defaultRender = Array.ConvertAll(characterModel.baseRendererInfos, item => new RenderInfo
            {
                renderer = (SkinnedMeshRenderer)item.renderer,
                material = item.defaultMaterial,
                ignoreOverlays = item.ignoreOverlays

            });

            SkinDefs.Default = CreateSkinDef("skinIfritDefault", mdlIfrit, defaultRender);

            var modelSkinController = mdlIfrit.AddComponent<ModelSkinController>();
            modelSkinController.skins = new SkinDef[]
            {
                SkinDefs.Default
            };
            #endregion

            #region RandomBlinkController
            var rbc = mdlIfrit.AddComponent<RandomBlinkController>();
            rbc.animator = animator;
            rbc.blinkTriggers = new string[] { "BlinkEye" };
            rbc.blinkChancePerUpdate = 10f;
            #endregion

            #region FixFireShader
            var particles = mdlIfrit.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach(var particleComponent in particles)
            {
                particleComponent.GetComponent<Renderer>().material = materialLookup["matIfritManeFire"];
            }
            #endregion

            #region BisonSprintEffect
            var bisonBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bison/BisonBody.prefab").WaitForCompletion();
            var sprintEffectTransform = bisonBody.transform.Find("ModelBase/mdlBison/BisonArmature/ROOT/SprintEffect");
            var sprintEffectCopy = UnityEngine.GameObject.Instantiate(sprintEffectTransform.gameObject);
            sprintEffectCopy.transform.parent = modelTransform.Find("Armature");
            sprintEffectCopy.transform.localPosition = new Vector3(-0.7647f, 3.0132f, 0.0441f);
            sprintEffectCopy.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
            sprintEffectCopy.transform.localScale = new Vector3(2f, 2f, 2f);
            sprintEffectCopy.SetActive(false);

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "SprintEffect", transform = sprintEffectCopy.transform });
            #endregion

            //mdlIfrit.AddComponent<DunnoRaycasterOrSomething>();

            //var helper = mdlIfrit.AddComponent<AnimationParameterHelper>();
            //helper.animator = animator;
            //helper.animationParameters = new string[] { "walkSpeedDebug" };
            #endregion

            #region AimAssist
            var aimAssistTarget = bodyPrefab.transform.Find("ModelBase/mdlIfrit/AimAssist").gameObject.AddComponent<AimAssistTarget>();
            aimAssistTarget.point0 = headTransform;
            aimAssistTarget.point1 = tailTransform;
            aimAssistTarget.assistScale = 2f;
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
            var asdPylon = masterPrefab.AddComponent<AISkillDriver>();
            asdPylon.customName = "SummonPylon";
            asdPylon.skillSlot = SkillSlot.Special;

            asdPylon.requiredSkill = null;
            asdPylon.requireSkillReady = true;
            asdPylon.requireEquipmentReady = false;
            asdPylon.minUserHealthFraction = float.NegativeInfinity;
            asdPylon.maxUserHealthFraction = 0.6f;
            asdPylon.minTargetHealthFraction = float.NegativeInfinity;
            asdPylon.maxTargetHealthFraction = float.PositiveInfinity;
            asdPylon.minDistance = 0f;
            asdPylon.maxDistance = float.PositiveInfinity;
            asdPylon.selectionRequiresTargetLoS = false;
            asdPylon.selectionRequiresOnGround = false;
            asdPylon.selectionRequiresAimTarget = false;
            asdPylon.maxTimesSelected = -1;

            asdPylon.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdPylon.activationRequiresTargetLoS = true;
            asdPylon.activationRequiresAimTargetLoS = false;
            asdPylon.activationRequiresAimConfirmation = false;
            asdPylon.movementType = AISkillDriver.MovementType.Stop;
            asdPylon.moveInputScale = 1;
            asdPylon.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdPylon.ignoreNodeGraph = false;
            asdPylon.shouldSprint = false;
            asdPylon.shouldFireEquipment = false;
            asdPylon.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdPylon.driverUpdateTimerOverride = -1f;
            asdPylon.resetCurrentEnemyOnNextDriverSelection = false;
            asdPylon.noRepeat = true;
            asdPylon.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_Hellzone
            var asdHellzone = masterPrefab.AddComponent<AISkillDriver>();
            asdHellzone.customName = "Hellzone";
            asdHellzone.skillSlot = SkillSlot.Secondary;

            asdHellzone.requiredSkill = null;
            asdHellzone.requireSkillReady = true;
            asdHellzone.requireEquipmentReady = false;
            asdHellzone.minUserHealthFraction = float.NegativeInfinity;
            asdHellzone.maxUserHealthFraction = float.PositiveInfinity;
            asdHellzone.minTargetHealthFraction = float.NegativeInfinity;
            asdHellzone.maxTargetHealthFraction = float.PositiveInfinity;
            asdHellzone.minDistance = 0f;
            asdHellzone.maxDistance = 15f;
            asdHellzone.selectionRequiresTargetLoS = true;
            asdHellzone.selectionRequiresOnGround = false;
            asdHellzone.selectionRequiresAimTarget = true;
            asdHellzone.maxTimesSelected = -1;

            asdHellzone.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdHellzone.activationRequiresTargetLoS = true;
            asdHellzone.activationRequiresAimTargetLoS = false;
            asdHellzone.activationRequiresAimConfirmation = true;
            asdHellzone.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdHellzone.moveInputScale = 0;
            asdHellzone.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdHellzone.ignoreNodeGraph = false;
            asdHellzone.shouldSprint = false;
            asdHellzone.shouldFireEquipment = false;
            asdHellzone.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdHellzone.driverUpdateTimerOverride = -1f;
            asdHellzone.resetCurrentEnemyOnNextDriverSelection = false;
            asdHellzone.noRepeat = true;
            asdHellzone.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_FlameCharge
            var asdFlameCharge = masterPrefab.AddComponent<AISkillDriver>();
            asdFlameCharge.customName = "FlameCharge";
            asdFlameCharge.skillSlot = SkillSlot.Utility;

            asdFlameCharge.requiredSkill = null;
            asdFlameCharge.requireSkillReady = true;
            asdFlameCharge.requireEquipmentReady = false;
            asdFlameCharge.minUserHealthFraction = float.NegativeInfinity;
            asdFlameCharge.maxUserHealthFraction = float.PositiveInfinity;
            asdFlameCharge.minTargetHealthFraction = float.NegativeInfinity;
            asdFlameCharge.maxTargetHealthFraction = float.PositiveInfinity;
            asdFlameCharge.minDistance = 7f;
            asdFlameCharge.maxDistance = 60f;
            asdFlameCharge.selectionRequiresTargetLoS = true;
            asdFlameCharge.selectionRequiresOnGround = false;
            asdFlameCharge.selectionRequiresAimTarget = false;
            asdFlameCharge.maxTimesSelected = -1;

            asdFlameCharge.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdFlameCharge.activationRequiresTargetLoS = true;
            asdFlameCharge.activationRequiresAimTargetLoS = false;
            asdFlameCharge.activationRequiresAimConfirmation = true;
            asdFlameCharge.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdFlameCharge.moveInputScale = 1;
            asdFlameCharge.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdFlameCharge.ignoreNodeGraph = true;
            asdFlameCharge.shouldSprint = false;
            asdFlameCharge.shouldFireEquipment = false;
            asdFlameCharge.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdFlameCharge.driverUpdateTimerOverride = -1f;
            asdFlameCharge.resetCurrentEnemyOnNextDriverSelection = false;
            asdFlameCharge.noRepeat = true;
            asdFlameCharge.nextHighPriorityOverride = null;
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
        
        public GameObject CreateHellzoneProjectile(GameObject dotzonePrefab)
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
                component.childrenProjectilePrefab = dotzonePrefab;
                component.blastRadius = EnemiesReturnsConfiguration.Ifrit.HellzoneRadius.Value; 
                component.blastDamageCoefficient = 1f; // leave it at 1 so projectile itself deals full damage
                component.childrenDamageCoefficient = EnemiesReturnsConfiguration.Ifrit.HellzoneDoTZoneDamage.Value;
            }

            return gameObject;
        }

        public GameObject CreateHellfireDotZoneProjectile(GameObject pillarPrefab, Texture2D texLavaCrack)
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenAcid.prefab").WaitForCompletion().InstantiateClone("IfritHellzoneDoTZoneProjectile", true);

            gameObject.GetComponent<ProjectileDotZone>().lifetime = EnemiesReturnsConfiguration.Ifrit.HellzoneDoTZoneLifetime.Value
                + EnemiesReturnsConfiguration.Ifrit.HellzonePillarCount.Value * EnemiesReturnsConfiguration.Ifrit.HellzonePillarDelay.Value; 

            gameObject.GetComponent<ProjectileController>().ghostPrefab = null;

            gameObject.GetComponent<ProjectileDamage>().damageType.damageType = DamageType.IgniteOnHit;

            var fxTransform = gameObject.transform.Find("FX");
            var fxScale = EnemiesReturnsConfiguration.Ifrit.HellzoneRadius.Value;
            fxTransform.localScale = new Vector3(fxScale, fxScale, fxScale); 

            var decal = gameObject.transform.Find("FX/Decal");
            decal.gameObject.SetActive(true);
            UnityEngine.GameObject.DestroyImmediate(gameObject.transform.Find("FX/Spittle").gameObject);
            UnityEngine.GameObject.DestroyImmediate(gameObject.transform.Find("FX/Gas").gameObject);

            var light = gameObject.transform.Find("FX/Point Light");
            light.gameObject.SetActive(true);
            light.localPosition = new Vector3(0f, 0.1f, 0f);

            var lightComponent = light.GetComponent<Light>();
            lightComponent.range = EnemiesReturnsConfiguration.Ifrit.HellzoneRadius.Value; 
            lightComponent.color = new Color(1f, 0.54f, 0.172f);

            gameObject.transform.Find("FX/Hitbox").transform.localScale = new Vector3(1.5f, 0.33f, 1.5f);

            var decalGameObject = decal.gameObject;
            decal.localScale = new Vector3(1.8f, 0.5f, 1.8f);
            var decalComponent = decalGameObject.GetComponent<Decal>();
            var newDecalMaterial = UnityEngine.Object.Instantiate(decalComponent.Material);
            newDecalMaterial.name = "matIfritHellzoneDecalLavaCrack";
            newDecalMaterial.SetTexture("_MaskTex", texLavaCrack);
            newDecalMaterial.SetColor("_Color", new Color(255f / 255f, 103f / 255f, 127f / 255f));
            newDecalMaterial.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/ColorRamps/texBehemothRamp.png").WaitForCompletion());
            newDecalMaterial.SetFloat("_AlphaBoost", 1f);
            decalComponent.Material = newDecalMaterial;

            var teamIndicator = UnityEngine.GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/TeamAreaIndicator, GroundOnly.prefab").WaitForCompletion());
            teamIndicator.transform.parent = fxTransform;
            teamIndicator.transform.localPosition = Vector3.zero;
            teamIndicator.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            teamIndicator.transform.localScale = Vector3.one;
            teamIndicator.GetComponent<TeamAreaIndicator>().teamFilter = gameObject.GetComponent<TeamFilter>();

            var scorchlingBody = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Scorchling/ScorchlingBody.prefab").WaitForCompletion();
            var scorchlingPile = UnityEngine.GameObject.Instantiate(scorchlingBody.transform.Find("ModelBase/mdlScorchling/mdlScorchlingBreachPile").gameObject);
            scorchlingPile.transform.parent = fxTransform;
            scorchlingPile.transform.localPosition = Vector3.zero;
            scorchlingPile.transform.localRotation = Quaternion.identity;
            float scale = 0.5f * (EnemiesReturnsConfiguration.Ifrit.HellzoneRadius.Value / 9f); // 0.5 fits 9 so scale of it
            scorchlingPile.transform.localScale = new Vector3(scale, scale, scale);

            var spawnChildrenComponent = gameObject.AddComponent<ProjectileSpawnChildrenInRowsWithDelay>();
            spawnChildrenComponent.radius = EnemiesReturnsConfiguration.Ifrit.HellzoneRadius.Value;
            spawnChildrenComponent.numberOfRows = EnemiesReturnsConfiguration.Ifrit.HellzonePillarCount.Value;
            spawnChildrenComponent.childrenDamageCoefficient = EnemiesReturnsConfiguration.Ifrit.HellzonePillarDamage.Value;
            spawnChildrenComponent.delayEachRow = EnemiesReturnsConfiguration.Ifrit.HellzonePillarDelay.Value;
            spawnChildrenComponent.childPrefab = pillarPrefab;

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
            projectileOverlapAttack.damageCoefficient = 1f;
            projectileOverlapAttack.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileExplosionVFX.prefab").WaitForCompletion();
            projectileOverlapAttack.forceVector = new Vector3(0f, EnemiesReturnsConfiguration.Ifrit.HellzonePillarForce.Value, 0f);
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

        public Material CreateManeMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/GreaterWisp/matGreaterWispFire.mat").WaitForCompletion());
            material.name = "matIfritManeFire";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());
            ContentProvider.MaterialCache.Add(material);

            return material;
        }

        public GameObject CreateFlameBreath()
        {
            var gameObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FlamebreathEffect.prefab").WaitForCompletion();

            UnityEngine.Object.DestroyImmediate(gameObject.GetComponent<ScaleParticleSystemDuration>());

            var components = gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach(var component in components)
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
            foreach(var component in components)
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

            skillDef.baseRechargeInterval = EnemiesReturnsConfiguration.Ifrit.PillarCooldown.Value;
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

            skillDef.baseRechargeInterval = EnemiesReturnsConfiguration.Ifrit.HellzoneCooldown.Value;
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

        internal SkillDef CreateFlameChargeSkill()
        {
            var skillDef = ScriptableObject.CreateInstance<SkillDef>();

            (skillDef as ScriptableObject).name = "IfritBodyFlameCharge";
            skillDef.skillName = "FlameCharge";

            skillDef.skillNameToken = "ENEMIES_RETURNS_IFRIT_FLAME_CHARGE_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_IFRIT_FLAME_CHARGE_DESCRIPTION";
            //var loaderGroundSlam = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/GroundSlam.asset").WaitForCompletion();
            //if (loaderGroundSlam)
            //{
            //    skillDef.icon = loaderGroundSlam.icon;
            //}

            skillDef.activationStateMachineName = "Body";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeBegin));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = EnemiesReturnsConfiguration.Ifrit.FlameChargeCooldown.Value;
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
            (idrs as ScriptableObject).name = "idrsIfrit";
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
            return EnemiesReturnsConfiguration.Ifrit.PillarMaxInstances.Value;
        }

        private void Renamer(GameObject object1)
        {

        }
    }
}

