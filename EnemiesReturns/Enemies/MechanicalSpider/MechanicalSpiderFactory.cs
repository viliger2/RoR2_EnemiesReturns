using EnemiesReturns.PrefabAPICompat;
using HG;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.ItemDisplayRuleSet;
using static EnemiesReturns.Utils;
using RoR2.Networking;
using KinematicCharacterController;
using System.Linq;
using EnemiesReturns.EditorHelpers;
using ThreeEyedGames;
using EnemiesReturns.Configuration;
using EnemiesReturns.Helpers;

namespace EnemiesReturns.Enemies.MechanicalSpider
{
    public class MechanicalSpiderFactory
    {
        public struct Skills
        {
            public static SkillDef DoubleShot;

            public static SkillDef Dash;

            //public static SkillDef ChargedSpit;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;

            public static SkillFamily Utility;

            //public static SkillFamily Secondary;

            //public static SkillFamily Special;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
            //public static SkinDef Lakes;
            //public static SkinDef Sulfur;
            //public static SkinDef Depths;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscMechanicalSpiderDefault;
            //public static CharacterSpawnCard cscSpitterLakes;
            //public static CharacterSpawnCard cscSpitterSulfur;
            //public static CharacterSpawnCard cscSpitterDepths;
        }

        public static GameObject MechanicalSpiderBody;

        public static GameObject MechanicalSpiderMaster;

        public GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var aimOrigin = bodyPrefab.transform.Find("AimOrigin");
            var cameraPivot = bodyPrefab.transform.Find("CameraPivot");
            var modelTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider");
            var modelBase = bodyPrefab.transform.Find("ModelBase");

            var focusPoint = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/LogBookTarget");
            var cameraPosition = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/LogBookTarget/LogBookCamera");

            var modelRenderer = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/MechanicalSpider").gameObject.GetComponent<SkinnedMeshRenderer>();

            var headTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Gun");
            var rootTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root");
            if (!headTransform)
            {
                Log.Warning("headTransform is null! This WILL result in stuck camera on " + nameof(MechanicalSpider) + " spawn.");
            }
            if (!rootTransform)
            {
                Log.Warning("rootTransform is null! This WILL result in stuck camera on " + nameof(MechanicalSpider) + " spawn.");
            }

            var animator = modelTransform.gameObject.GetComponent<Animator>();

            #region MechanicalSpiderBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterDirection
            var characterDirection = bodyPrefab.AddComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBase;
            characterDirection.turnSpeed = 720f;
            characterDirection.modelAnimator = animator;
            #endregion

            #region CharacterMotor
            var characterMotor = bodyPrefab.AddComponent<CharacterMotor>();
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 100f;
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
            characterBody.baseNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.None | CharacterBody.BodyFlags.Mechanical;
            characterBody.rootMotionInMainState = false;
            characterBody.mainRootSpeed = 33f;

            characterBody.baseMaxHealth = EnemiesReturns.Configuration.MechanicalSpider.BaseMaxHealth.Value;
            characterBody.baseRegen = 0f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = EnemiesReturns.Configuration.MechanicalSpider.BaseMoveSpeed.Value;
            characterBody.baseAcceleration = 40f;
            characterBody.baseJumpPower = EnemiesReturns.Configuration.MechanicalSpider.BaseJumpPower.Value;
            characterBody.baseDamage = EnemiesReturns.Configuration.MechanicalSpider.BaseDamage.Value;
            characterBody.baseAttackSpeed = 1f;
            characterBody.baseCrit = 0f;
            characterBody.baseArmor = EnemiesReturns.Configuration.MechanicalSpider.BaseArmor.Value;
            characterBody.baseVisionDistance = float.PositiveInfinity;
            characterBody.baseJumpCount = 1;
            characterBody.sprintingSpeedMultiplier = 1.45f;

            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturns.Configuration.MechanicalSpider.LevelMaxHealth.Value;
            characterBody.levelDamage = EnemiesReturns.Configuration.MechanicalSpider.LevelDamage.Value;
            characterBody.levelArmor = EnemiesReturns.Configuration.MechanicalSpider.LevelArmor.Value;

            characterBody.wasLucky = false;
            characterBody.spreadBloomDecayTime = 0.45f;
            characterBody._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            characterBody.aimOriginTransform = aimOrigin;
            characterBody.hullClassification = HullClassification.Human;
            if (sprite)
            {
                characterBody.portraitIcon = sprite.texture;
            }
            characterBody.bodyColor = new Color(0.5568628f, 0.627451f, 0.6745098f);
            characterBody.isChampion = false;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized));
            #endregion

            #region CameraTargetParams
            var cameraTargetParams = bodyPrefab.AddComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardTall.asset").WaitForCompletion();
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

            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.3f;
            modelLocator.normalMaxAngleDelta = 55f;
            #endregion

            #region EntityStateMachineBody
            var esmBody = bodyPrefab.AddComponent<EntityStateMachine>();
            esmBody.customName = "Body";
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.SpawnState));
            esmBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.MainState));
            #endregion

            #region EntityStateMachineWeapon
            var esmWeapon = bodyPrefab.AddComponent<EntityStateMachine>();
            esmWeapon.customName = "Weapon";
            esmWeapon.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            esmWeapon.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            #endregion

            #region EntityStateSlide
            var esmSlide = bodyPrefab.AddComponent<EntityStateMachine>();
            esmSlide.customName = "Slide";
            esmSlide.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            esmSlide.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            #endregion

            #region GenericSkills

            #region Primary
            var gsPrimary = bodyPrefab.AddComponent<GenericSkill>();
            gsPrimary._skillFamily = SkillFamilies.Primary;
            gsPrimary.skillName = "DoubleShot";
            gsPrimary.hideInCharacterSelect = false;
            #endregion

            #region Utility
            var gsUtility = bodyPrefab.AddComponent<GenericSkill>();
            gsUtility._skillFamily = SkillFamilies.Utility;
            gsUtility.skillName = "Dash";
            gsUtility.hideInCharacterSelect = false;
            #endregion

            #region Special
            //var gsSpecial = bodyPrefab.AddComponent<GenericSkill>();
            //gsSpecial._skillFamily = SkillFamilies.Special;
            //gsSpecial.skillName = "ChargedSpit";
            //gsSpecial.hideInCharacterSelect = false;
            #endregion

            #endregion

            #region SkillLocator
            SkillLocator skillLocator = null;
            if (!bodyPrefab.TryGetComponent(out skillLocator))
            {
                skillLocator = bodyPrefab.AddComponent<SkillLocator>();
            }
            skillLocator.primary = gsPrimary;
            skillLocator.utility = gsUtility;
            //skillLocator.secondary = gsSecondary;
            //skillLocator.special = gsSpecial;
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
            bodyPrefab.AddComponent<Interactor>().maxInteractionDistance = 3f;
            #endregion

            #region InteractionDriver
            bodyPrefab.AddComponent<InteractionDriver>();
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath)); // TODO: spawn interactable on death that you can use
            characterDeathBehavior.idleStateMachine = new EntityStateMachine[] { esmWeapon, esmSlide };
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
            bodyPrefab.AddComponent<DeathRewards>().logUnlockableDef = log;
            #endregion

            #region EquipmentSlot
            bodyPrefab.AddComponent<EquipmentSlot>();
            #endregion

            #region SfxLocator
            var sfxLocator = bodyPrefab.AddComponent<SfxLocator>();
            sfxLocator.deathSound = ""; // TODO
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
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
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
            kinematicCharacterMotor.playerCharacter = true; // thanks Randy
            #endregion

            #region SetStateOnHurt
            var setStateOnHurt = bodyPrefab.AddComponent<SetStateOnHurt>();
            setStateOnHurt.targetStateMachine = esmBody;
            setStateOnHurt.idleStateMachine = new EntityStateMachine[] { esmWeapon };
            setStateOnHurt.hurtState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtState));
            setStateOnHurt.hitThreshold = 0.3f;
            setStateOnHurt.canBeHitStunned = true;
            setStateOnHurt.canBeStunned = true;
            setStateOnHurt.canBeFrozen = true;
            #endregion

            #region ExecuteSkillOnDamage
            var skillOnDamage = bodyPrefab.AddComponent<ExecuteSkillOnDamage>();
            skillOnDamage.characterBody = characterBody;
            skillOnDamage.skillToExecute = gsUtility;
            skillOnDamage.mainStateMachine = esmBody;
            #endregion

            #endregion

            #region SetupBoxes

            var surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/RoboBallBoss/sdRoboBall.asset").WaitForCompletion(); // TODO: maybe make my own

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

            var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/MainHurtbox");
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;

            //var hitBox = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head/Hitbox").gameObject.AddComponent<HitBox>();
            #endregion

            #region mdlMechanicalSpider
            var mdlMechanicalSpider = modelTransform.gameObject;

            // FIXING IMPACT\LIGHTIMPACT\LANDING ISSUES:
            // first of all, animation should not have "landing" by itself in it, only the impact of landing
            // second, add a reference pose to animation at the time of where animation reaches its resting position
            // third, if your animation doesn't do the first rule then just clip it so it has minimal body movement
            // and only has the impact of landing 

            #region AimAnimator
            // if you are having issues with AimAnimator,
            // * just add Additive Reference Pose for your pitch and yaw animations in the middle of the animation
            // * make both animations loop
            // * set them both to zero speed in your animation controller
            // * I haven't found how to add poses to "separate" animation files, so those have to be in fbx
            var aimAnimator = mdlMechanicalSpider.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBank;
            aimAnimator.directionComponent = characterDirection;

            aimAnimator.pitchRangeMin = -65f; // its looking up, not down, for fuck sake
            aimAnimator.pitchRangeMax = 65f;

            aimAnimator.yawRangeMin = -180f;
            aimAnimator.yawRangeMax = 180f;
            aimAnimator.fullYaw = true;

            aimAnimator.pitchGiveupRange = 40f;
            aimAnimator.yawGiveupRange = 20f;

            aimAnimator.giveupDuration = 3f;

            aimAnimator.raisedApproachSpeed = 720f;
            aimAnimator.loweredApproachSpeed = 360f;
            aimAnimator.smoothTime = 0.1f;

            aimAnimator.aimType = AimAnimator.AimType.Direct;

            aimAnimator.enableAimWeight = false;
            aimAnimator.UseTransformedAimVector = false;
            #endregion

            #region ChildLocator
            var childLocator = mdlMechanicalSpider.AddComponent<ChildLocator>();
            var ourChildLocator = mdlMechanicalSpider.GetComponent<OurChildLocator>();
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
            var hurtboxGroup = mdlMechanicalSpider.AddComponent<HurtBoxGroup>();
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
            if (!mdlMechanicalSpider.TryGetComponent<AnimationEvents>(out _))
            {
                mdlMechanicalSpider.AddComponent<AnimationEvents>();
            }
            #endregion

            #region DestroyOnUnseen
            mdlMechanicalSpider.AddComponent<DestroyOnUnseen>().cull = false;
            #endregion

            #region CharacterModel
            var characterModel = mdlMechanicalSpider.AddComponent<CharacterModel>();
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
                }
            };
            #endregion

            #region HitBoxGroupBite
            //var hbgBite = mdlMechanicalSpider.AddComponent<HitBoxGroup>();
            //hbgBite.groupName = "Bite";
            //hbgBite.hitBoxes = new HitBox[] { hitBox };
            #endregion

            #region FootstepHandler
            FootstepHandler footstepHandler = null;
            if (!mdlMechanicalSpider.TryGetComponent(out footstepHandler))
            {
                footstepHandler = mdlMechanicalSpider.AddComponent<FootstepHandler>();
            }
            footstepHandler.enableFootstepDust = true;
            footstepHandler.baseFootstepString = "Play_treeBot_step";
            footstepHandler.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion();
            #endregion

            #region ModelPanelParameters
            var modelPanelParameters = mdlMechanicalSpider.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focusPoint;
            modelPanelParameters.cameraPositionTransform = cameraPosition;
            modelPanelParameters.modelRotation = new Quaternion(0, 0, 0, 1);
            modelPanelParameters.minDistance = 1.5f;
            modelPanelParameters.maxDistance = 6f;
            #endregion

            #region SkinDefs
            SkinDefs.Default = CreateSkinDef("skinMechanicalSpiderDefault", mdlMechanicalSpider, characterModel.baseRendererInfos);

            CharacterModel.RendererInfo[] lakesRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matMechanicalSpider"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
            };

            var modelSkinController = mdlMechanicalSpider.AddComponent<ModelSkinController>();
            modelSkinController.skins = new SkinDef[]
            {
                SkinDefs.Default
            };
            #endregion

            //var helper = mdlMechanicalSpider.AddComponent<AnimationParameterHelper>();
            //helper.animator = modelTransform.gameObject.GetComponent<Animator>();
            //helper.animationParameters = new string[] { "walkSpeedDebug" };

            #endregion

            #region AimAssist
            var aimAssistTarget = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/AimAssist").gameObject.AddComponent<AimAssistTarget>();
            aimAssistTarget.point0 = headTransform;
            aimAssistTarget.point1 = rootTransform;
            aimAssistTarget.assistScale = 1f;
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

            //#region AISkillDriver_DashBecauseClose
            //var asdDashWhenClose = masterPrefab.AddComponent<AISkillDriver>();
            //asdDashWhenClose.customName = "DashWhenClose";
            //asdDashWhenClose.skillSlot = SkillSlot.Utility;

            //asdDashWhenClose.requiredSkill = null;
            //asdDashWhenClose.requireSkillReady = true;
            //asdDashWhenClose.requireEquipmentReady = false;
            //asdDashWhenClose.minUserHealthFraction = float.NegativeInfinity;
            //asdDashWhenClose.maxUserHealthFraction = float.PositiveInfinity;
            //asdDashWhenClose.minTargetHealthFraction = float.NegativeInfinity;
            //asdDashWhenClose.maxTargetHealthFraction = float.PositiveInfinity;
            //asdDashWhenClose.minDistance = 0f;
            //asdDashWhenClose.maxDistance = 60f;
            //asdDashWhenClose.selectionRequiresTargetLoS = false;
            //asdDashWhenClose.selectionRequiresOnGround = false;
            //asdDashWhenClose.selectionRequiresAimTarget = false;
            //asdDashWhenClose.maxTimesSelected = -1;

            //asdDashWhenClose.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //asdDashWhenClose.activationRequiresTargetLoS = false;
            //asdDashWhenClose.activationRequiresAimTargetLoS = false;
            //asdDashWhenClose.activationRequiresAimConfirmation = false;
            //asdDashWhenClose.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            //asdDashWhenClose.moveInputScale = 1;
            //asdDashWhenClose.aimType = AISkillDriver.AimType.AtMoveTarget;
            //asdDashWhenClose.ignoreNodeGraph = true;
            //asdDashWhenClose.shouldSprint = false;
            //asdDashWhenClose.shouldFireEquipment = false;
            //asdDashWhenClose.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            //asdDashWhenClose.driverUpdateTimerOverride = -1;
            //asdDashWhenClose.resetCurrentEnemyOnNextDriverSelection = false;
            //asdDashWhenClose.noRepeat = false;
            //asdDashWhenClose.nextHighPriorityOverride = null;
            //#endregion

            #region AISkillDriver_StrafeAndShoot
            var asdStrafeAndShoot = masterPrefab.AddComponent<AISkillDriver>();
            asdStrafeAndShoot.customName = "StrafeAndShoot";
            asdStrafeAndShoot.skillSlot = SkillSlot.Primary;

            asdStrafeAndShoot.requiredSkill = null;
            asdStrafeAndShoot.requireSkillReady = false;
            asdStrafeAndShoot.requireEquipmentReady = false;
            asdStrafeAndShoot.minUserHealthFraction = float.NegativeInfinity;
            asdStrafeAndShoot.maxUserHealthFraction = float.PositiveInfinity;
            asdStrafeAndShoot.minTargetHealthFraction = float.NegativeInfinity;
            asdStrafeAndShoot.maxTargetHealthFraction = float.PositiveInfinity;
            asdStrafeAndShoot.minDistance = 0f;
            asdStrafeAndShoot.maxDistance = 60f;
            asdStrafeAndShoot.selectionRequiresTargetLoS = true;
            asdStrafeAndShoot.selectionRequiresOnGround = false;
            asdStrafeAndShoot.selectionRequiresAimTarget = false;
            asdStrafeAndShoot.maxTimesSelected = -1;

            asdStrafeAndShoot.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdStrafeAndShoot.activationRequiresTargetLoS = true;
            asdStrafeAndShoot.activationRequiresAimTargetLoS = false;
            asdStrafeAndShoot.activationRequiresAimConfirmation = true;
            asdStrafeAndShoot.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            asdStrafeAndShoot.moveInputScale = 1;
            asdStrafeAndShoot.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdStrafeAndShoot.ignoreNodeGraph = true;
            asdStrafeAndShoot.shouldSprint = false;
            asdStrafeAndShoot.shouldFireEquipment = false;
            asdStrafeAndShoot.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdStrafeAndShoot.driverUpdateTimerOverride = -1;
            asdStrafeAndShoot.resetCurrentEnemyOnNextDriverSelection = false;
            asdStrafeAndShoot.noRepeat = false;
            asdStrafeAndShoot.nextHighPriorityOverride = null;
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

            #region AISkillDriver_DashToTraverse
            var asdDashToTraverse = masterPrefab.AddComponent<AISkillDriver>();
            asdDashToTraverse.customName = "DashToTraverse";
            asdDashToTraverse.skillSlot = SkillSlot.Utility;

            asdDashToTraverse.requiredSkill = null;
            asdDashToTraverse.requireSkillReady = true;
            asdDashToTraverse.requireEquipmentReady = false;
            asdDashToTraverse.minUserHealthFraction = float.NegativeInfinity;
            asdDashToTraverse.maxUserHealthFraction = float.PositiveInfinity;
            asdDashToTraverse.minTargetHealthFraction = float.NegativeInfinity;
            asdDashToTraverse.maxTargetHealthFraction = float.PositiveInfinity;
            asdDashToTraverse.minDistance = 50f;
            asdDashToTraverse.maxDistance = float.PositiveInfinity;
            asdDashToTraverse.selectionRequiresTargetLoS = false;
            asdDashToTraverse.selectionRequiresOnGround = false;
            asdDashToTraverse.selectionRequiresAimTarget = false;
            asdDashToTraverse.maxTimesSelected = -1;

            asdDashToTraverse.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdDashToTraverse.activationRequiresTargetLoS = true;
            asdDashToTraverse.activationRequiresAimTargetLoS = false;
            asdDashToTraverse.activationRequiresAimConfirmation = false;
            asdDashToTraverse.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdDashToTraverse.moveInputScale = 1;
            asdDashToTraverse.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdDashToTraverse.ignoreNodeGraph = true;
            asdDashToTraverse.shouldSprint = false;
            asdDashToTraverse.shouldFireEquipment = false;
            asdDashToTraverse.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdDashToTraverse.driverUpdateTimerOverride = -1;
            asdDashToTraverse.resetCurrentEnemyOnNextDriverSelection = false;
            asdDashToTraverse.noRepeat = false;
            asdDashToTraverse.nextHighPriorityOverride = null;
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

        #region SkillDefs

        internal SkillDef CreateDoubleShotSkill()
        {
            var bite = ScriptableObject.CreateInstance<SkillDef>();
            (bite as ScriptableObject).name = "MechanicalSpiderWeaponDoubleShot";
            bite.skillName = "DoubleShot";

            bite.skillNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DOUBLE_SHOT_NAME";
            bite.skillDescriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DOUBLE_SHOT_DESCRIPTION";
            //var acridBite = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoBite.asset").WaitForCompletion();
            //if (acridBite)
            //{
            //    bite.icon = acridBite.icon;
            //}

            bite.activationStateMachineName = "Weapon";
            bite.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.OpenHatch));
            bite.interruptPriority = EntityStates.InterruptPriority.Skill;

            bite.baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.DoubleShotCooldown.Value;
            bite.baseMaxStock = 1;
            bite.rechargeStock = 1;
            bite.requiredStock = 1;
            bite.stockToConsume = 1;

            bite.resetCooldownTimerOnUse = false;
            bite.fullRestockOnAssign = true;
            bite.dontAllowPastMaxStocks = false;
            bite.beginSkillCooldownOnSkillEnd = false;

            bite.cancelSprintingOnActivation = true;
            bite.forceSprintDuringState = false;
            bite.canceledFromSprinting = false;

            bite.isCombatSkill = true;
            bite.mustKeyPress = false;

            return bite;
        }

        internal SkillDef CreateDashSkill()
        {
            var spit = ScriptableObject.CreateInstance<SkillDef>();
            (spit as ScriptableObject).name = "MechanicalSpiderBodyDash";
            spit.skillName = "Dash";

            spit.skillNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DASH_NAME";
            spit.skillDescriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DASH_DESCRIPTION";
            //var crocoSpit = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoSpit.asset").WaitForCompletion();
            //if (crocoSpit)
            //{
            //    spit.icon = crocoSpit.icon;
            //}

            spit.activationStateMachineName = "Slide";
            spit.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.Dash));
            spit.interruptPriority = EntityStates.InterruptPriority.Any;

            spit.baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.DashCooldown.Value;
            spit.baseMaxStock = 1;
            spit.rechargeStock = 1;
            spit.requiredStock = 1;
            spit.stockToConsume = 1;

            spit.resetCooldownTimerOnUse = false;
            spit.fullRestockOnAssign = true;
            spit.dontAllowPastMaxStocks = false;
            spit.beginSkillCooldownOnSkillEnd = false;

            spit.cancelSprintingOnActivation = true;
            spit.forceSprintDuringState = false;
            spit.canceledFromSprinting = false;

            spit.isCombatSkill = true;
            spit.mustKeyPress = false;

            return spit;
        }

        //internal SkillDef CreateChargedSpitSkill()
        //{
        //    var chargedSpit = ScriptableObject.CreateInstance<SkillDef>();
        //    (chargedSpit as ScriptableObject).name = "SpitterBodyChargedSpit";
        //    chargedSpit.skillName = "Bite";

        //    chargedSpit.skillNameToken = "ENEMIES_RETURNS_SPITTER_CHARGED_SPIT_NAME";
        //    chargedSpit.skillDescriptionToken = "ENEMIES_RETURNS_SPITTER_CHARGED_SPIT_DESCRIPTION";
        //    var acridEpidemic = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoDisease.asset").WaitForCompletion();
        //    if (acridEpidemic)
        //    {
        //        chargedSpit.icon = acridEpidemic.icon;
        //    }

        //    chargedSpit.activationStateMachineName = "Body";
        //    chargedSpit.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.ChargeChargedSpit));
        //    chargedSpit.interruptPriority = EntityStates.InterruptPriority.Any; // not sure

        //    chargedSpit.baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.ChargedProjectileCooldown.Value;
        //    chargedSpit.baseMaxStock = 1;
        //    chargedSpit.rechargeStock = 1;
        //    chargedSpit.requiredStock = 1;
        //    chargedSpit.stockToConsume = 1;

        //    chargedSpit.resetCooldownTimerOnUse = false;
        //    chargedSpit.fullRestockOnAssign = true;
        //    chargedSpit.dontAllowPastMaxStocks = false;
        //    chargedSpit.beginSkillCooldownOnSkillEnd = false;

        //    chargedSpit.cancelSprintingOnActivation = true;
        //    chargedSpit.forceSprintDuringState = false;
        //    chargedSpit.canceledFromSprinting = false;

        //    chargedSpit.isCombatSkill = true;
        //    chargedSpit.mustKeyPress = false;

        //    return chargedSpit;
        //}

        #endregion

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
            card.directorCreditCost = EnemiesReturns.Configuration.MechanicalSpider.DirectorCost.Value;
            card.occupyPosition = false;
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
            (idrs as ScriptableObject).name = "idrsMechanicalSpider";
            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "JawR",
                localPos = new Vector3(-0.32686F, 2.51006F, -0.21041F),
                localAngles = new Vector3(354.7525F, 340F, 7.12234F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "JawL",
                localPos = new Vector3(-0.12522F, 2.55699F, -0.28728F),
                localAngles = new Vector3(354.7525F, 20.00001F, 351.6044F),
                localScale = new Vector3(-0.6F, 0.6F, 0.6F),
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
                localPos = new Vector3(0F, -0.29125F, -0.36034F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.43975F, 0.43975F, 0.43975F),
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
                localPos = new Vector3(-0.36417F, 4.08597F, -0.81975F),
                localAngles = new Vector3(88.15041F, 342.9204F, 152.0255F),
                localScale = new Vector3(0.28734F, 0.28734F, 0.28734F),
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
                childName = "TailEnd",
                localPos = new Vector3(-0.00302F, 0.77073F, 0.00143F),
                localAngles = new Vector3(284.2227F, 198.9412F, 159.205F),
                localScale = new Vector3(1.15579F, 1.15579F, 1.15579F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "JawL",
                localPos = new Vector3(-0.25677F, 2.49244F, -0.13195F),
                localAngles = new Vector3(323.8193F, 261.7038F, 7.48606F),
                localScale = new Vector3(1.45f, 1.45f, 1.45f),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "JawR",
                localPos = new Vector3(-0.00804F, 2.49258F, -0.13194F),
                localAngles = new Vector3(322.8305F, 89.99672F, 7F),
                localScale = new Vector3(1.45f, 1.45f, 1.45f),
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
                childName = "Head",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
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
                childName = "JawL",
                localPos = new Vector3(0F, 0.38638F, -0.00001F),
                localAngles = new Vector3(0F, 270F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "JawR",
                localPos = new Vector3(0F, 0.38638F, -0.00001F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                localPos = new Vector3(-0.12323F, 2.48183F, -0.47279F),
                localAngles = new Vector3(7.00848F, 2.28661F, 1.77239F),
                localScale = new Vector3(4.42437F, 4.42437F, 4.42437F),
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
                localPos = new Vector3(0F, 1.1304F, 0.00001F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.84412F, 0.84412F, 0.84412F),
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
                localPos = new Vector3(0.03071F, 0.76725F, -0.77548F),
                localAngles = new Vector3(285.3486F, 351.2853F, 11.4788F),
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
                localPos = new Vector3(-0.08902F, 0.78706F, 1.13371F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(3.40115F, 3.40115F, 3.40115F),
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

        private void Renamer(GameObject object1)
        {
        }
    }
}
