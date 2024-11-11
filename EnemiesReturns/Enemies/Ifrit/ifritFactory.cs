using EnemiesReturns.EditorHelpers;
using EnemiesReturns.PrefabAPICompat;
using EnemiesReturns.Projectiles;
using HG;
using KinematicCharacterController;
using RoR2;
using RoR2.Audio;
using RoR2.CharacterAI;
using RoR2.Mecanim;
using RoR2.Networking;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using ThreeEyedGames;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using static EnemiesReturns.Utils;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritFactory
    {
        public struct Skills
        {
            public static SkillDef SummonPylon;

            public static SkillDef Hellzone;

            public static SkillDef FlameCharge;

            public static SkillDef Smash;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;

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

        public GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, ExplicitPickupDropTable droptable)
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
            characterDirection.turnSpeed = EnemiesReturns.Configuration.Ifrit.TurnSpeed.Value;
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

            characterBody.baseMaxHealth = EnemiesReturns.Configuration.Ifrit.BaseMaxHealth.Value;
            characterBody.baseRegen = 0f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = EnemiesReturns.Configuration.Ifrit.BaseMoveSpeed.Value;
            characterBody.baseAcceleration = 60f;
            characterBody.baseJumpPower = EnemiesReturns.Configuration.Ifrit.BaseJumpPower.Value;
            characterBody.baseDamage = EnemiesReturns.Configuration.Ifrit.BaseDamage.Value;
            characterBody.baseAttackSpeed = 1f;
            characterBody.baseCrit = 0f;
            characterBody.baseArmor = EnemiesReturns.Configuration.Ifrit.BaseArmor.Value;
            characterBody.baseVisionDistance = float.PositiveInfinity;
            characterBody.baseJumpCount = 1;
            characterBody.sprintingSpeedMultiplier = 1.45f;

            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturns.Configuration.Ifrit.LevelMaxHealth.Value;
            characterBody.levelDamage = EnemiesReturns.Configuration.Ifrit.LevelDamage.Value;
            characterBody.levelArmor = EnemiesReturns.Configuration.Ifrit.LevelArmor.Value;

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

            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.5f;
            modelLocator.normalMaxAngleDelta = 35f;
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
            var gsPrimary = bodyPrefab.AddComponent<GenericSkill>();
            gsPrimary._skillFamily = SkillFamilies.Primary;
            gsPrimary.skillName = "Smash";
            gsPrimary.hideInCharacterSelect = false;
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
            skillLocator.primary = gsPrimary;
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
            sfxLocator.deathSound = "ER_Ifrit_Death_Play";
            sfxLocator.barkSound = "";
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
            if (capsuleCollider.center != Vector3.zero)
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

            var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Spine/Spine.001/MainHurtbox");
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;

            var flameHitBox = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Spine/Spine.001/Neck/Head/Jaw/FlameChargeHitbox").gameObject.AddComponent<HitBox>();
            var chargeHitbox = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/ChargeHitbox").gameObject.AddComponent<HitBox>();
            var smashHitbox = bodyPrefab.transform.Find("ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/SmashHitbox").gameObject.AddComponent<HitBox>();
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

            #region FixFireShader
            var particles = mdlIfrit.gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
            var material = ContentProvider.GetOrCreateMaterial("matIfritManeFire", CreateManeFiresMaterial);
            foreach (var particleComponent in particles)
            {
                particleComponent.material = material;
            }
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
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false
                }
            };
            foreach (var particleComponent in particles)
            {
                ArrayUtils.ArrayAppend(ref characterModel.baseRendererInfos, new CharacterModel.RendererInfo
                {
                    renderer = particleComponent,
                    defaultMaterial = particleComponent.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false,
                });
            }
            #endregion

            #region HitBoxGroupSmash
            var hbgBite = mdlIfrit.AddComponent<HitBoxGroup>();
            hbgBite.groupName = "Smash";
            hbgBite.hitBoxes = new HitBox[] { smashHitbox };
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
            modelPanelParameters.minDistance = 4f;
            modelPanelParameters.maxDistance = 24f;
            #endregion

            #region SkinDefs
            SkinDefs.Default = CreateSkinDef("skinIfritDefault", mdlIfrit, characterModel.baseRendererInfos);

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
            asdHellzone.maxDistance = 50f;
            asdHellzone.selectionRequiresTargetLoS = false;
            asdHellzone.selectionRequiresOnGround = false;
            asdHellzone.selectionRequiresAimTarget = false;
            asdHellzone.maxTimesSelected = -1;

            asdHellzone.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdHellzone.activationRequiresTargetLoS = false;
            asdHellzone.activationRequiresAimTargetLoS = false;
            asdHellzone.activationRequiresAimConfirmation = false;
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

        public Material CreateManeFiresMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/GreaterWisp/matGreaterWispFire.mat").WaitForCompletion());
            material.name = "matIfritManeFire";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            return material;
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

        #endregion

        #region SkillDefs

        internal SkillDef CreateSummonPylonSkill()
        {
            var skillDef = ScriptableObject.CreateInstance<SkillDef>();

            (skillDef as ScriptableObject).name = "IfritBodySummonPylon";
            skillDef.skillName = "SummonPylon";

            skillDef.skillNameToken = "ENEMIES_RETURNS_IFRIT_SUMMON_PYLON_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_IFRIT_SUMMON_PYLON_DESCRIPTION";
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyChargeSnipeSuper.asset").WaitForCompletion();
            if (iconSource)
            {
                skillDef.icon = iconSource.icon;
            }

            skillDef.activationStateMachineName = "Body";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.SummonPylon));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = EnemiesReturns.Configuration.Ifrit.PillarCooldown.Value;
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
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallAirstrikeAlt.asset").WaitForCompletion();
            if (iconSource)
            {
                skillDef.icon = iconSource.icon;
            }

            skillDef.activationStateMachineName = "Body";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneStart));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = EnemiesReturns.Configuration.Ifrit.HellzoneCooldown.Value;
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
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/Chef/ChefSear.asset").WaitForCompletion();
            if (iconSource)
            {
                skillDef.icon = iconSource.icon;
            }

            skillDef.activationStateMachineName = "Body";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeBegin));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = EnemiesReturns.Configuration.Ifrit.FlameChargeCooldown.Value;
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

        internal SkillDef CreateSmashSkill()
        {
            var skillDef = ScriptableObject.CreateInstance<SkillDef>();

            (skillDef as ScriptableObject).name = "IfritWeaponSmash";
            skillDef.skillName = "Smash";

            skillDef.skillNameToken = "ENEMIES_RETURNS_IFRIT_SMASH_NAME";
            skillDef.skillDescriptionToken = "ENEMIES_RETURNS_IFRIT_SMASH_DESCRIPTION";
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/FalseSon/FalseSonClubSlam.asset").WaitForCompletion();
            if (iconSource)
            {
                skillDef.icon = iconSource.icon;
            }

            skillDef.activationStateMachineName = "Weapon";
            skillDef.activationState = new EntityStates.SerializableEntityStateType(typeof(Junk.ModdedEntityStates.Ifrit.Smash));
            skillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            skillDef.baseRechargeInterval = 0f;
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
            card.directorCreditCost = EnemiesReturns.Configuration.Ifrit.DirectorCost.Value;
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
                childName = "ShoulderR",
                localPos = new Vector3(-0.07618F, -0.22209F, -0.07389F),
                localAngles = new Vector3(356.5784F, 60.60427F, 303.4548F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "ShoulderL",
                localPos = new Vector3(0.02979F, -0.23431F, -0.0541F),
                localAngles = new Vector3(333.7023F, 319.7617F, 43.31199F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                childName = "Head",
                localPos = new Vector3(0.04621F, -0.3427F, -0.48813F),
                localAngles = new Vector3(329.0771F, 358.3423F, 183.6484F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                localPos = new Vector3(0.06479F, -0.38449F, -0.49458F),
                localAngles = new Vector3(42.55269F, 173.9934F, 178.0236F),
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
                childName = "Chest",
                localPos = new Vector3(0.1547F, -0.21215F, -0.27268F),
                localAngles = new Vector3(330.5753F, 141.3175F, 118.0065F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.12394F, 0.58573F, -0.3623F),
                localAngles = new Vector3(338.1842F, 225.1289F, 269.9162F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Pelvis",
                localPos = new Vector3(-0.11418F, 0.03336F, -0.08187F),
                localAngles = new Vector3(341.638F, 224.6317F, 258.2183F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.12395F, -0.24669F, -0.21695F),
                localAngles = new Vector3(338.1842F, 225.1289F, 269.9162F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(0.31959F, 0.56037F, -0.22525F),
                localAngles = new Vector3(336.4365F, 137.4821F, 109.3917F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Pelvis",
                localPos = new Vector3(0.21452F, 0.02704F, -0.06636F),
                localAngles = new Vector3(357.0562F, 147.3994F, 121.6319F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
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
                localPos = new Vector3(0F, 0.11885F, -0.18199F),
                localAngles = new Vector3(7.73144F, 180.0001F, 180F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.00001F, 0.75732F, -0.35454F),
                localAngles = new Vector3(7.73144F, 180.0001F, 180F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.00002F, -0.34168F, -0.561F),
                localAngles = new Vector3(7.73144F, 180.0001F, 180F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
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
                childName = "Chest",
                localPos = new Vector3(0.02025F, 1.06304F, -0.35235F),
                localAngles = new Vector3(321.0952F, 175.1595F, 1.17263F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(0.00727F, 0.3707F, -0.44392F),
                localAngles = new Vector3(3.87917F, 179.9956F, 182.1576F),
                localScale = new Vector3(0.15F, 0.15F, 0.15F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "Pelvis",
                localPos = new Vector3(-0.22009F, 0.02119F, -0.10358F),
                localAngles = new Vector3(358.2295F, 178.8255F, 0.63453F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                localPos = new Vector3(-0.00464F, 0.09453F, 0.00552F),
                localAngles = new Vector3(296.9135F, 187.694F, 169.9966F),
                localScale = new Vector3(3F, 3F, 3F),
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
                childName = "Head",
                localPos = new Vector3(-0.01302F, 0.66335F, 0.04278F),
                localAngles = new Vector3(278.5223F, 352.4039F, 6.56596F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupVoid,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC1/EliteVoid/EliteVoidEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region BeadElite
            var displayRuleGroupBead = new DisplayRuleGroup();
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Elites/EliteBead/DisplayEliteBeadSpike.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.03164F, 0.04501F, -0.42897F),
                localAngles = new Vector3(285.3486F, 351.2853F, 11.47881F),
                localScale = new Vector3(0.14951F, 0.14951F, 0.14951F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupBead,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC2/Elites/EliteBead/EliteBeadEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region GoldElite
            var displayRuleGroupGold = new DisplayRuleGroup();
            displayRuleGroupGold.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Elites/EliteAurelionite/DisplayEliteAurelioniteEquipment.prefab").WaitForCompletion(),
                childName = "Head",
                localPos = new Vector3(-0.01787F, 0.91068F, -0.17726F),
                localAngles = new Vector3(343.8055F, 180.3407F, 177.7121F),
                localScale = new Vector3(1.17694F, 1.17694F, 1.17694F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupGold,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC2/Elites/EliteAurelionite/EliteAurelioniteEquipment.asset").WaitForCompletion()
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
            return EnemiesReturns.Configuration.Ifrit.PillarMaxInstances.Value;
        }

        private void Renamer(GameObject object1)
        {

        }
    }
}

