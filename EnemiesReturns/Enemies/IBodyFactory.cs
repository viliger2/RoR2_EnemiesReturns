using EnemiesReturns.EditorHelpers;
using KinematicCharacterController;
using RoR2;
using RoR2.Networking;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies
{
    public interface IBodyFactory
    {
        internal class CharacterMotorParams
        {
            public CharacterMotorParams(CharacterDirection direction)
            {
                this.direction = direction;
            }

            public CharacterDirection direction;
            public bool muteWalkMotion = false;
            public float mass = 100f;
            public float airControl = 0.25f;
            public bool disableAirControl = false;
            public bool generateParametersOnAwake = true;
        }

        internal class CharacterBodyParams
        {
            public CharacterBodyParams(string nameToken, GameObject crosshair, Transform aimOrigin, Texture icon, EntityStates.SerializableEntityStateType initialState)
            {
                this.nameToken = nameToken;
                this.defaultCrosshairPrefab = crosshair;
                this.aimOrigin = aimOrigin;
                this.portraitIcon = icon;
                this.preferredInitialStateType = initialState;
            }

            public string nameToken;
            public CharacterBody.BodyFlags flags = CharacterBody.BodyFlags.None;
            public bool rootMotionInMainState = false;
            public float mainRootSpeed = 33f;

            public float baseMaxHealth = 300f;
            public float baseRegen = 0f;
            public float baseMaxShield = 0f;
            public float baseMoveSpeed = 7f;
            public float baseAcceleration = 40f;
            public float baseJumpPower = 20f;
            public float baseDamage = 20f;
            public float baseAttackSpeed = 1f;
            public float baseCrit = 0f;
            public float baseArmor = 0f;
            public float baseVisionDistance = float.PositiveInfinity;
            public int baseJumpCount = 1;
            public float sprintingMultiplier = 1.45f;

            public float levelMaxHealth = 90f;
            public float levelRegen = 0f;
            public float levelMaxShield = 0f;
            public float levelMoveSpeed = 0f;
            public float levelJumpPower = 0f;
            public float levelDamage = 4f;
            public float levelAttackSpeed = 0f;
            public float levelCrit = 0f;
            public float levelArmor = 0f;

            public float spreadBloomDecayTime = 0.45f;
            public GameObject defaultCrosshairPrefab;
            public Transform aimOrigin;
            public HullClassification hullClassification = HullClassification.Human;
            public Texture portraitIcon;
            public Color bodyColor = Color.red;
            public bool isChampion = false;
            public EntityStates.SerializableEntityStateType preferredInitialStateType;

            public bool autoCalculateStats
            {
                get => _autoCalculateStats;
                set
                {
                    if (value)
                    {
                        PerformAutoCalculateLevelStats();
                        _autoCalculateStats = value;
                    }
                }
            }

            private bool _autoCalculateStats = false;

            public void PerformAutoCalculateLevelStats()
            {
                levelMaxHealth = Mathf.Round(baseMaxHealth * 0.3f);
                levelMaxShield = Mathf.Round(baseMaxShield * 0.3f);
                levelRegen = baseRegen * 0.2f;
                levelMoveSpeed = 0f;
                levelJumpPower = 0f;
                levelDamage = baseDamage * 0.2f;
                levelAttackSpeed = 0f;
                levelCrit = 0f;
                levelArmor = 0f;
            }
        }

        internal class ModelLocatorParams
        {
            public ModelLocatorParams(Transform modelTransform, Transform modelBaseTransform)
            {
                this.modelTransform = modelTransform;
                this.modelBaseTransform = modelBaseTransform;
            }


            public Transform modelTransform;
            public Transform modelBaseTransform;

            public bool autoUpdateModelTransform = true;
            public bool dontDetachFromParent = false;

            public bool noCorpse = false;
            public bool dontReleaseModelOnDeath = false;
            public bool preserveModel = false;
            public bool forceCulled = false;

            public bool normalizeToFloor = false;
            public float normalSmoothdampTime = 0.1f;
            public float normalMaxAngleDelta = 90f;
        }

        internal class SfxLocatorParams
        {
            public string deathSound = "";
            public string barkSound = "";
            public string openSound = "";
            public string landingSound = "";
            public string fallDamageSound = "";
            public string aliveLoopStart = "";
            public string aliveLoopStop = "";
            public string sprintLoopStart = "";
            public string sprintLoopStop = "";
        }

        internal class SetStateOnHurtParams
        {
            public SetStateOnHurtParams(EntityStateMachine target, EntityStates.SerializableEntityStateType hurtState, params EntityStateMachine[] idle)
            {
                this.targetStateMachine = target;
                this.hurtState = hurtState;
                this.idleStateMachines = idle;
            }

            public EntityStateMachine targetStateMachine;
            public EntityStateMachine[] idleStateMachines;
            public EntityStates.SerializableEntityStateType hurtState;
            public float hitThreshold = 0.3f;
            public bool canBeHitStunned = true;
            public bool canBeStunned = true;
            public bool canBeFrozen = true;
        }

        internal class KinemacitCharacterMotorParams
        {
            public KinemacitCharacterMotorParams(CapsuleCollider capsule, Rigidbody rigidBody, ICharacterController characterController)
            {
                this.AttachedRigidBody = rigidBody;
                this.characterController = characterController;
                if (capsule.radius < 0.5f)
                {
                    Log.Warning($"CapsuleCollider {capsule} has radius less than a beetle (0.5f), this WILL result in pathfinding issues for AIs.");
                };
                if (capsule.height < 1.82f)
                {
                    Log.Warning($"CapsuleCollider {capsule} has heigh less than a beetle (1.82f), this WILL result in pathfinding issues for AIs.");
                };
                if (capsule.center != Vector3.zero)
                {
                    Log.Warning($"CapsuleCollider {capsule} has non-zero center, this WILL result in pathfinding issues for AIs.");
                };
                this.Capsule = capsule;
            }

            public ICharacterController characterController;
            public Rigidbody AttachedRigidBody;
            public CapsuleCollider Capsule;

            public float GroundDetectionExtraDistance = 0f;
            public float MaxStableSlopeAngle = 55f;
            public LayerMask StableGroundLayers = Physics.AllLayers;
            public bool DiscreteCollisionEvents = false;

            public StepHandlingMethod StepHandling = StepHandlingMethod.Standard;
            public float MaxStepHeight = 0.2f;
            public bool AllowSteppingWithoutStableGrounding = false;
            public float MinRequiredStepDepth = 0.1f;

            public bool LedgeAndDenivelationHandling = true;
            public float MaxStableDistanceFromLedge = 0.5f;
            public float MaxVelocityForLedgeSnap = 0f;
            public float MaxStableDenivelationAngle = 55f;

            public bool InteractiveRigidbodyHandling = true;
            public RigidbodyInteractionType RigidbodyInteractionType = RigidbodyInteractionType.None;
            public float SimulatedCharacterMass = 1f;
            public bool PreserveAttachedRigidbodyMomentum = true;

            public bool HasPlanarConstraint = false;
            public Vector3 PlanarConstraintAxis = Vector3.forward;

            public bool CheckMovementInitialOverlaps = true;
            public bool KillVelocityWhenExceedMaxMovementIterations = true;
            public bool KillRemainingMovementWhenExceedMaxMovementIterations = true;
            public float timeUntilUpdate = 0f;
            public bool playerCharacter = false;

        }

        // if you are having issues with AimAnimator,
        // * just add Additive Reference Pose for your pitch and yaw animations in the middle of the animation
        // * make both animations loop
        // * set them both to zero speed in your animation controller
        // * I haven't found how to add poses to "separate" animation files, so those have to be in fbx
        internal class AimAnimatorParams
        {
            public AimAnimatorParams(InputBankTest inputBank, CharacterDirection direction)
            {
                this.inputBank = inputBank;
                this.directionComponent = direction;
            }

            public InputBankTest inputBank;
            public CharacterDirection directionComponent;

            public float pitchRangeMin = -25f; // its looking up, not down, for fuck sake
            public float pitchRangeMax = 25f;

            public float yawRangeMin = -70f;
            public float yawRangeMax = 70f;

            public float pitchGiveUpRange = 20f;
            public float yawGiveUpRange = 20f;

            public float giveUpDuration = 3f;

            public float raisedApproachSpeed = 720f;
            public float loweredApproachSpeed = 360f;
            public float smoothTime = 0.1f;

            public bool fullYaw = false;
            public AimAnimator.AimType aimType = AimAnimator.AimType.Direct;

            public bool enableAimWeight = false;
            public bool UseTransformedAimVector = false;
        }

        internal class FootstepHandlerParams
        {
            public string baseFootstepString = "";
            public string baseFootliftString = "";
            public string sprintFootstepOverrideString = "";
            public string sprintFootliftOverrideString = "";
            public bool enableFootstepDust = false;
            public GameObject footstepDustPrefab;
        }

        #region Model

        internal AimAnimator AddAimAnimator(GameObject model, AimAnimatorParams aimParams)
        {
            AimAnimator aimAnimator = model.GetOrAddComponent<AimAnimator>();

            aimAnimator.inputBank = aimParams.inputBank;
            aimAnimator.directionComponent = aimParams.directionComponent;
            aimAnimator.pitchRangeMin = aimParams.pitchRangeMin;
            aimAnimator.pitchRangeMax = aimParams.pitchRangeMax;
            aimAnimator.yawRangeMin = aimParams.yawRangeMin;
            aimAnimator.yawRangeMax = aimParams.yawRangeMax;
            aimAnimator.pitchGiveupRange = aimParams.pitchGiveUpRange;
            aimAnimator.yawGiveupRange = aimParams.yawGiveUpRange;
            aimAnimator.giveupDuration = aimParams.giveUpDuration;
            aimAnimator.raisedApproachSpeed = aimParams.raisedApproachSpeed;
            aimAnimator.loweredApproachSpeed = aimParams.loweredApproachSpeed;
            aimAnimator.smoothTime = aimParams.smoothTime;
            aimAnimator.fullYaw = aimParams.fullYaw;
            aimAnimator.aimType = aimParams.aimType;
            aimAnimator.enableAimWeight = aimParams.enableAimWeight;
            aimAnimator.UseTransformedAimVector = aimParams.UseTransformedAimVector;

            return aimAnimator;
        }

        internal ChildLocator AddChildLocator(GameObject model)
        {
            ChildLocator childLocator = model.GetOrAddComponent<ChildLocator>(); ;

            var ourChildLocator = model.GetComponent<OurChildLocator>();
            if (!ourChildLocator)
            {
                Log.Warning($"Model {model} is missing OurChildLocator component, ChildLocator will be empty!");
                return childLocator;
            }
            childLocator.transformPairs = Array.ConvertAll(ourChildLocator.transformPairs, item =>
            {
                return new ChildLocator.NameTransformPair
                {
                    name = item.name,
                    transform = item.transform,
                };
            });
            UnityEngine.Object.Destroy(ourChildLocator);

            return childLocator;
        }

        internal HurtBoxGroup AddHurtBoxGroup(GameObject model, HurtBox[] hurtboxes)
        {
            HurtBoxGroup hurtBoxGroup = model.GetOrAddComponent<HurtBoxGroup>();

            hurtBoxGroup.hurtBoxes = hurtboxes.ToArray();
            for (short i = 0; i < hurtBoxGroup.hurtBoxes.Length; i++)
            {
                hurtBoxGroup.hurtBoxes[i].hurtBoxGroup = hurtBoxGroup;
                hurtBoxGroup.hurtBoxes[i].indexInGroup = i;
                if (hurtBoxGroup.hurtBoxes[i].isBullseye)
                {
                    hurtBoxGroup.bullseyeCount++;
                }
                if (hurtBoxGroup.hurtBoxes[i].transform.name == "MainHurtBox")
                {
                    hurtBoxGroup.mainHurtBox = hurtBoxGroup.hurtBoxes[i];
                }
            }

            return hurtBoxGroup;
        }

        internal AnimationEvents AddAnimationEvents(GameObject model)
        {
            return model.GetOrAddComponent<AnimationEvents>();
        }

        internal DestroyOnUnseen AddDestroyOnUnseen(GameObject model)
        {
            return model.GetOrAddComponent<DestroyOnUnseen>();
        }

        internal CharacterModel AddCharacterModel(GameObject model, CharacterBody body, ItemDisplayRuleSet idrs, CharacterModel.RendererInfo[] renderInfos, bool autoPopulateLightInfos = true)
        {
            CharacterModel characterModel = model.GetOrAddComponent<CharacterModel>();

            characterModel.body = body;
            characterModel.itemDisplayRuleSet = idrs;
            characterModel.autoPopulateLightInfos = autoPopulateLightInfos;
            characterModel.baseRendererInfos = renderInfos;

            return characterModel;
        }

        internal HitBoxGroup AddHitBoxGroup(GameObject model, string name, params HitBox[] hitboxes)
        {
            var hitBoxGroup = model.AddComponent<HitBoxGroup>();
            hitBoxGroup.groupName = name;
            hitBoxGroup.hitBoxes = hitboxes;

            return hitBoxGroup;
        }

        internal FootstepHandler AddFootstepHandler(GameObject model, FootstepHandlerParams footstepHandlerParams)
        {
            FootstepHandler footstepHandler = model.GetOrAddComponent<FootstepHandler>();

            footstepHandler.baseFootstepString = footstepHandlerParams.baseFootstepString;
            footstepHandler.baseFootliftString = footstepHandlerParams.baseFootliftString;
            footstepHandler.sprintFootstepOverrideString = footstepHandlerParams.sprintFootstepOverrideString;
            footstepHandler.sprintFootliftOverrideString = footstepHandlerParams.sprintFootliftOverrideString;
            footstepHandler.enableFootstepDust = footstepHandlerParams.enableFootstepDust;
            footstepHandler.footstepDustPrefab = footstepHandlerParams.footstepDustPrefab;

            return footstepHandler;
        }

        internal ModelPanelParameters AddModelPanelParameters(GameObject model, Transform focusPoint, Transform cameraPoint, Quaternion modelRotation, float minDistance = 1.5f, float maxDistance = 6f)
        {
            ModelPanelParameters modelPanel = model.GetOrAddComponent<ModelPanelParameters>();

            modelPanel.focusPointTransform = focusPoint;
            modelPanel.cameraPositionTransform = cameraPoint;
            modelPanel.modelRotation = modelRotation;
            modelPanel.minDistance = minDistance;
            modelPanel.maxDistance = maxDistance;

            return modelPanel;
        }

        internal ModelSkinController AddModelSkinController(GameObject model, params SkinDef[] skinDefs)
        {
            ModelSkinController skinController = model.GetOrAddComponent<ModelSkinController>();
            skinController.skins = skinDefs;

            return skinController;
        }

        #endregion

        #region Body

        internal NetworkIdentity AddNetworkIdentity(GameObject bodyPrefab, bool serverOnly = false, bool localAuthority = true)
        {
            var networkIdentity = bodyPrefab.GetOrAddComponent<NetworkIdentity>();
            networkIdentity.serverOnly = serverOnly;
            networkIdentity.localPlayerAuthority = localAuthority;

            return networkIdentity;
        }

        internal CharacterDirection AddCharacterDirection(GameObject bodyPrefab, Transform modelBase, Animator modelAnimator, float turnSpeed = 720f)
        {
            CharacterDirection direction;
            if (!bodyPrefab.TryGetComponent<CharacterDirection>(out direction))
            {
                direction = bodyPrefab.AddComponent<CharacterDirection>();
            }
            direction.targetTransform = modelBase;
            direction.turnSpeed = turnSpeed;
            direction.modelAnimator = modelAnimator;

            return direction;
        }

        internal CharacterMotor AddCharacterMotor(GameObject bodyPrefab, CharacterMotorParams parameters)
        {
            CharacterMotor motor = bodyPrefab.GetOrAddComponent<CharacterMotor>();
            motor.characterDirection = parameters.direction;
            motor.muteWalkMotion = parameters.muteWalkMotion;
            motor.mass = parameters.mass;
            motor.airControl = parameters.airControl;
            motor.disableAirControlUntilCollision = parameters.disableAirControl;
            motor.generateParametersOnAwake = parameters.generateParametersOnAwake;

            return motor;
        }

        internal InputBankTest AddInputBankTest(GameObject bodyPrefab)
        {
            return bodyPrefab.GetOrAddComponent<InputBankTest>();
        }

        internal CharacterBody AddCharacterBody(GameObject bodyPrefab, CharacterBodyParams bodyParams)
        {
            CharacterBody body = bodyPrefab.GetOrAddComponent<CharacterBody>();

            body.baseNameToken = bodyParams.nameToken;
            body.bodyFlags = bodyParams.flags;
            body.rootMotionInMainState = bodyParams.rootMotionInMainState;
            body.mainRootSpeed = bodyParams.mainRootSpeed;

            body.baseMaxHealth = bodyParams.baseMaxHealth;
            body.baseRegen = bodyParams.baseRegen;
            body.baseMaxShield = bodyParams.baseMaxShield;
            body.baseMoveSpeed = bodyParams.baseMoveSpeed;
            body.baseAcceleration = bodyParams.baseAcceleration;
            body.baseJumpPower = bodyParams.baseJumpPower;
            body.baseDamage = bodyParams.baseDamage;
            body.baseAttackSpeed = bodyParams.baseAttackSpeed;
            body.baseCrit = bodyParams.baseCrit;
            body.baseArmor = bodyParams.baseArmor;
            body.baseVisionDistance = bodyParams.baseVisionDistance;
            body.baseJumpCount = bodyParams.baseJumpCount;
            body.sprintingSpeedMultiplier = bodyParams.sprintingMultiplier;

            body.levelMaxHealth = bodyParams.levelMaxHealth;
            body.levelRegen = bodyParams.levelRegen;
            body.levelMaxShield = bodyParams.levelMaxShield;
            body.levelMoveSpeed = bodyParams.levelMoveSpeed;
            body.levelJumpPower = bodyParams.levelJumpPower;
            body.levelDamage = bodyParams.levelDamage;
            body.levelAttackSpeed = bodyParams.levelAttackSpeed;
            body.levelCrit = bodyParams.levelCrit;
            body.levelArmor = bodyParams.levelArmor;

            body.spreadBloomDecayTime = bodyParams.spreadBloomDecayTime;
            body._defaultCrosshairPrefab = bodyParams.defaultCrosshairPrefab;
            body.aimOriginTransform = bodyParams.aimOrigin;
            body.hullClassification = bodyParams.hullClassification;
            body.portraitIcon = bodyParams.portraitIcon;
            body.bodyColor = bodyParams.bodyColor;
            body.isChampion = bodyParams.isChampion;
            body.preferredInitialStateType = bodyParams.preferredInitialStateType;

            return body;
        }

        internal CameraTargetParams AddCameraTargetParams(GameObject bodyPrefab, CharacterCameraParams cameraParams)
        {
            CameraTargetParams cameraTargetParams = bodyPrefab.GetOrAddComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = cameraParams;

            return cameraTargetParams;
        }

        internal ModelLocator AddModelLocator(GameObject bodyPrefab, ModelLocatorParams modelLocatorParams)
        {
            ModelLocator modelLocator = bodyPrefab.GetOrAddComponent<ModelLocator>();

            modelLocator.modelTransform = modelLocatorParams.modelTransform;
            modelLocator.modelBaseTransform = modelLocatorParams.modelBaseTransform;

            modelLocator.autoUpdateModelTransform = modelLocatorParams.autoUpdateModelTransform;
            modelLocator.dontDetatchFromParent = modelLocatorParams.dontDetachFromParent;

            modelLocator.noCorpse = modelLocatorParams.noCorpse;
            modelLocator.dontReleaseModelOnDeath = modelLocatorParams.dontReleaseModelOnDeath;
            modelLocator.preserveModel = modelLocatorParams.preserveModel;
            modelLocator.forceCulled = modelLocatorParams.forceCulled;

            modelLocator.normalizeToFloor = modelLocatorParams.normalizeToFloor;
            modelLocator.normalSmoothdampTime = modelLocatorParams.normalSmoothdampTime;
            modelLocator.normalMaxAngleDelta = modelLocatorParams.normalMaxAngleDelta;

            return modelLocator;
        }

        internal EntityStateMachine AddEntityStateMachine(GameObject bodyPrefab, string name, EntityStates.SerializableEntityStateType initialState, EntityStates.SerializableEntityStateType mainState)
        {
            var esm = bodyPrefab.AddComponent<EntityStateMachine>();
            esm.customName = name;
            esm.initialStateType = initialState;
            esm.mainStateType = mainState;

            return esm;
        }

        internal GenericSkill AddGenericSkill(GameObject bodyPrefab, SkillFamily family, string skillName, bool hideInCharacterSelect)
        {
            var skill = bodyPrefab.AddComponent<GenericSkill>();
            skill._skillFamily = family;
            skill.skillName = skillName;
            skill.hideInCharacterSelect = hideInCharacterSelect;

            return skill;
        }

        internal SkillLocator AddSkillLocator(GameObject bodyPrefab, GenericSkill primary, GenericSkill secondary, GenericSkill utility, GenericSkill special)
        {
            SkillLocator skillLocator = bodyPrefab.GetOrAddComponent<SkillLocator>();
            skillLocator.primary = primary;
            skillLocator.secondary = secondary;
            skillLocator.special = special;
            skillLocator.utility = utility;

            return skillLocator;
        }

        internal TeamComponent AddTeamComponent(GameObject bodyPrefab, TeamIndex teamIndex = TeamIndex.None)
        {
            TeamComponent teamComponent = bodyPrefab.GetOrAddComponent<TeamComponent>();
            teamComponent.teamIndex = teamIndex;

            return teamComponent;
        }

        internal HealthComponent AddHealthComponent(GameObject bodyPrefab, bool dontShotHealthbar = false, float globalDeathEventChance = 1f)
        {
            HealthComponent healthComponent = bodyPrefab.GetOrAddComponent<HealthComponent>();

            healthComponent.dontShowHealthbar = dontShotHealthbar;
            healthComponent.globalDeathEventChanceCoefficient = globalDeathEventChance;

            return healthComponent;
        }

        internal Interactor AddInteractor(GameObject bodyPrefab, float interactionDistance)
        {
            Interactor interactor = bodyPrefab.GetOrAddComponent<Interactor>();
            interactor.maxInteractionDistance = interactionDistance;

            return interactor;
        }

        internal InteractionDriver AddInteractionDriver(GameObject bodyPrefab)
        {
            return bodyPrefab.GetOrAddComponent<InteractionDriver>();
        }

        internal CharacterDeathBehavior AddCharacterDeathBehavior(GameObject bodyPrefab, EntityStateMachine deathStateMachine, EntityStates.SerializableEntityStateType deathState, params EntityStateMachine[] idleStateMachines)
        {
            CharacterDeathBehavior characterDeathBehavior = bodyPrefab.GetOrAddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathState = deathState;
            characterDeathBehavior.deathStateMachine = deathStateMachine;
            characterDeathBehavior.idleStateMachine = idleStateMachines;

            return characterDeathBehavior;
        }

        internal CharacterNetworkTransform AddCharacterNetworkTransform(GameObject bodyPrefab, float positionTransmitInterval = 0.1f, float interpolationFactor = 2f)
        {
            CharacterNetworkTransform characterNetworkTransform = bodyPrefab.GetOrAddComponent<CharacterNetworkTransform>();
            characterNetworkTransform.positionTransmitInterval = positionTransmitInterval;
            characterNetworkTransform.interpolationFactor = interpolationFactor;

            return characterNetworkTransform;
        }

        internal NetworkStateMachine AddNetworkStateMachine(GameObject bodyPrefab, params EntityStateMachine[] esms)
        {
            NetworkStateMachine networkStateMachine = bodyPrefab.GetOrAddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = esms;

            return networkStateMachine;
        }

        internal DeathRewards AddDeathRewards(GameObject bodyPrefab, UnlockableDef log, ExplicitPickupDropTable droptable = null)
        {
            DeathRewards deathRewards = bodyPrefab.GetOrAddComponent<DeathRewards>();
            deathRewards.logUnlockableDef = log;
            deathRewards.bossDropTable = droptable;

            return deathRewards;
        }

        internal EquipmentSlot AddEquipmentSlot(GameObject bodyPrefab)
        {
            return bodyPrefab.GetOrAddComponent<EquipmentSlot>();
        }

        internal SfxLocator AddSfxLocator(GameObject bodyPrefab, SfxLocatorParams sfxParams)
        {
            SfxLocator sfxLocator = bodyPrefab.GetOrAddComponent<SfxLocator>();
            sfxLocator.deathSound = sfxParams.deathSound;
            sfxLocator.barkSound = sfxParams.barkSound;
            sfxLocator.openSound = sfxParams.openSound;
            sfxLocator.landingSound = sfxParams.landingSound;
            sfxLocator.fallDamageSound = sfxParams.fallDamageSound;
            sfxLocator.aliveLoopStart = sfxParams.aliveLoopStart;
            sfxLocator.aliveLoopStop = sfxParams.aliveLoopStop;
            sfxLocator.sprintLoopStart = sfxParams.sprintLoopStart;
            sfxLocator.sprintLoopStop = sfxParams.sprintLoopStop;

            return sfxLocator;
        }

        internal SetStateOnHurt AddSetStateOnHurt(GameObject bodyPrefab, SetStateOnHurtParams hurtParams)
        {
            SetStateOnHurt state = bodyPrefab.GetOrAddComponent<SetStateOnHurt>();
            state.targetStateMachine = hurtParams.targetStateMachine;
            state.idleStateMachine = hurtParams.idleStateMachines;
            state.hurtState = hurtParams.hurtState;
            state.hitThreshold = hurtParams.hitThreshold;
            state.canBeFrozen = hurtParams.canBeFrozen;
            state.canBeHitStunned = hurtParams.canBeHitStunned;
            state.canBeStunned = hurtParams.canBeStunned;

            return state;
        }

        internal KinematicCharacterMotor AddKinematicCharacterMotor(GameObject bodyPrefab, KinemacitCharacterMotorParams kcmParams)
        {
            KinematicCharacterMotor kinematicCharacterMotor = bodyPrefab.GetOrAddComponent<KinematicCharacterMotor>();

            kinematicCharacterMotor.CharacterController = kcmParams.characterController;
            kinematicCharacterMotor.Capsule = kcmParams.Capsule;
            kinematicCharacterMotor._attachedRigidbody = kcmParams.AttachedRigidBody;
            kinematicCharacterMotor.CapsuleRadius = kcmParams.Capsule.radius;
            kinematicCharacterMotor.CapsuleHeight = kcmParams.Capsule.height;
            kinematicCharacterMotor.CapsuleYOffset = 0f;

            kinematicCharacterMotor.GroundDetectionExtraDistance = kcmParams.GroundDetectionExtraDistance;
            kinematicCharacterMotor.MaxStableSlopeAngle = kcmParams.MaxStableSlopeAngle;
            kinematicCharacterMotor.StableGroundLayers = kcmParams.StableGroundLayers;
            kinematicCharacterMotor.DiscreteCollisionEvents = kcmParams.DiscreteCollisionEvents;

            kinematicCharacterMotor.StepHandling = kcmParams.StepHandling;
            kinematicCharacterMotor.MaxStepHeight = kcmParams.MaxStepHeight;
            kinematicCharacterMotor.AllowSteppingWithoutStableGrounding = kcmParams.AllowSteppingWithoutStableGrounding;
            kinematicCharacterMotor.MinRequiredStepDepth = kcmParams.MinRequiredStepDepth;

            kinematicCharacterMotor.LedgeAndDenivelationHandling = kcmParams.LedgeAndDenivelationHandling;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = kcmParams.MaxStableDistanceFromLedge;
            kinematicCharacterMotor.MaxVelocityForLedgeSnap = kcmParams.MaxVelocityForLedgeSnap;
            kinematicCharacterMotor.MaxStableDenivelationAngle = kcmParams.MaxStableDenivelationAngle;

            kinematicCharacterMotor.InteractiveRigidbodyHandling = kcmParams.InteractiveRigidbodyHandling;
            kinematicCharacterMotor.RigidbodyInteractionType = kcmParams.RigidbodyInteractionType;
            kinematicCharacterMotor.SimulatedCharacterMass = kcmParams.SimulatedCharacterMass;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = kcmParams.PreserveAttachedRigidbodyMomentum;

            kinematicCharacterMotor.HasPlanarConstraint = kcmParams.HasPlanarConstraint;
            kinematicCharacterMotor.PlanarConstraintAxis = kcmParams.PlanarConstraintAxis;

            kinematicCharacterMotor.CheckMovementInitialOverlaps = kcmParams.CheckMovementInitialOverlaps;
            kinematicCharacterMotor.KillVelocityWhenExceedMaxMovementIterations = kcmParams.KillVelocityWhenExceedMaxMovementIterations;
            kinematicCharacterMotor.KillRemainingMovementWhenExceedMaxMovementIterations = kcmParams.KillRemainingMovementWhenExceedMaxMovementIterations;
            kinematicCharacterMotor.timeUntilUpdate = kcmParams.timeUntilUpdate;
            kinematicCharacterMotor.playerCharacter = kcmParams.playerCharacter;

            return kinematicCharacterMotor;
        }

        #endregion

        internal HurtBox[] SetupHurtboxes(GameObject bodyPrefab, SurfaceDef surfaceDef, HealthComponent healthComponent)
        {
            List<HurtBox> hurtBoxes = new List<HurtBox>();

            var hurtBoxesTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "HurtBox").ToArray();
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

            var mainHurtboxTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "MainHurtBox").First();
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;

            return hurtBoxes.ToArray();

        }

        internal AimAssistTarget AddAimAssist(GameObject prefab, Transform head, Transform root, HealthComponent healthComponent, TeamComponent teamComponent)
        {
            AimAssistTarget aimAssist = prefab.GetOrAddComponent<AimAssistTarget>();
            aimAssist.point0 = head;
            aimAssist.point1 = root;
            aimAssist.assistScale = 1f;
            aimAssist.healthComponent = healthComponent;
            aimAssist.teamComponent = teamComponent;

            return aimAssist;
        }

        public GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log);
    }
}
