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
using Newtonsoft.Json.Utilities;
using R2API;
using RoR2.CharacterAI;
using EnemiesReturns.EditorHelpers;
using RoR2.Skills;
using RoR2.Projectile;
using HG;
using static RoR2.ItemDisplayRuleSet;
using EnemiesReturns.Projectiles;
using ThreeEyedGames;
using static EnemiesReturns.Utils;
using EnemiesReturns.ModdedEntityStates.Junk.Spitter;

namespace EnemiesReturns.Enemies.Spitter
{
    public class SpitterFactory
    {
        public struct Skills
        {
            //public static SkillDef NormalSpit;

            public static SkillDef Bite;

            public static SkillDef ChargedSpit;
        }

        public struct SkillFamilies
        {
            //public static SkillFamily Primary;

            public static SkillFamily Secondary;

            public static SkillFamily Special;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
            public static SkinDef Lakes;
            public static SkinDef Sulfur;
            public static SkinDef Depths;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscSpitterDefault;
            public static CharacterSpawnCard cscSpitterLakes;
            public static CharacterSpawnCard cscSpitterSulfur;
            public static CharacterSpawnCard cscSpitterDepths;
        }

        public static GameObject SpitterBody;

        public static GameObject SpitterMaster;

        public GameObject CreateSpitterBody(GameObject bodyPrefab, Texture2D icon, UnlockableDef log, Dictionary<string, Material> skinsLookup)
        {
            var aimOrigin = bodyPrefab.transform.Find("AimOrigin");
            var modelTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter");
            var modelBase = bodyPrefab.transform.Find("ModelBase");

            var focusPoint = bodyPrefab.transform.Find("ModelBase/mdlSpitter/LogBookTarget");
            var cameraPosition = bodyPrefab.transform.Find("ModelBase/mdlSpitter/LogBookTarget/LogBookCamera");

            var modelRenderer = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Spitter").gameObject.GetComponent<SkinnedMeshRenderer>();
            var gumsRenderer = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Spitter Gums").gameObject.GetComponent<SkinnedMeshRenderer>();
            var teethenderer = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Spitter Teeth").gameObject.GetComponent<SkinnedMeshRenderer>();

            var headTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head");
            var rootTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root");

            #region SpitterBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterDirection
            var characterDirection = bodyPrefab.AddComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBase;
            characterDirection.turnSpeed = 720f;
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
            characterBody.baseNameToken = "ENEMIES_RETURNS_SPITTER_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.None;
            characterBody.rootMotionInMainState = false;
            characterBody.mainRootSpeed = 33f;

            characterBody.baseMaxHealth = EnemiesReturnsConfiguration.Spitter.BaseMaxHealth.Value;
            characterBody.baseRegen = 0f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = EnemiesReturnsConfiguration.Spitter.BaseMoveSpeed.Value;
            characterBody.baseAcceleration = 40f;
            characterBody.baseJumpPower = EnemiesReturnsConfiguration.Spitter.BaseJumpPower.Value;
            characterBody.baseDamage = EnemiesReturnsConfiguration.Spitter.BaseDamage.Value;
            characterBody.baseAttackSpeed = 1f;
            characterBody.baseCrit = 0f;
            characterBody.baseArmor = EnemiesReturnsConfiguration.Spitter.BaseArmor.Value;
            characterBody.baseVisionDistance = float.PositiveInfinity;
            characterBody.baseJumpCount = 1;
            characterBody.sprintingSpeedMultiplier = 1.45f;

            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturnsConfiguration.Spitter.LevelMaxHealth.Value;
            characterBody.levelDamage = EnemiesReturnsConfiguration.Spitter.LevelDamage.Value;
            characterBody.levelArmor = EnemiesReturnsConfiguration.Spitter.LevelArmor.Value;

            characterBody.wasLucky = false;
            characterBody.spreadBloomDecayTime = 0.45f;
            characterBody._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            characterBody.aimOriginTransform = aimOrigin;
            characterBody.hullClassification = HullClassification.Golem;
            characterBody.portraitIcon = icon;
            characterBody.bodyColor = new Color(0.737f, 0.682f, 0.588f);
            characterBody.isChampion = false;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized));
            #endregion

            #region CameraTargetParams
            var cameraTargetParams = bodyPrefab.AddComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardTall.asset").WaitForCompletion();
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
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.SpawnState));
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
            //var gsPrimary = spitterPrefab.AddComponent<GenericSkill>();
            //gsPrimary._skillFamily = SkillFamilies.Primary;
            //gsPrimary.skillName = "NormalSpit";
            //gsPrimary.hideInCharacterSelect = false;
            #endregion

            #region Secondary
            var gsSecondary = bodyPrefab.AddComponent<GenericSkill>();
            gsSecondary._skillFamily = SkillFamilies.Secondary;
            gsSecondary.skillName = "Bite";
            gsSecondary.hideInCharacterSelect = false;
            #endregion

            #region Special
            var gsSpecial = bodyPrefab.AddComponent<GenericSkill>();
            gsSpecial._skillFamily = SkillFamilies.Special;
            gsSpecial.skillName = "ChargedSpit";
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
            bodyPrefab.AddComponent<Interactor>().maxInteractionDistance = 3f;
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
            bodyPrefab.AddComponent<DeathRewards>().logUnlockableDef = log;
            #endregion

            #region EquipmentSlot
            bodyPrefab.AddComponent<EquipmentSlot>();
            #endregion

            #region SfxLocator
            var sfxLocator = bodyPrefab.AddComponent<SfxLocator>();
            sfxLocator.deathSound = "ER_Spitter_Death_Play";
            sfxLocator.barkSound = "";
            #endregion

            #region KinematicCharacterMotor
            var capsuleCollider = bodyPrefab.GetComponent<CapsuleCollider>();

            var kinematicCharacterMotor = bodyPrefab.AddComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor.Rigidbody = bodyPrefab.GetComponent<Rigidbody>();

            kinematicCharacterMotor.CapsuleRadius = capsuleCollider.radius;
            kinematicCharacterMotor.CapsuleHeight = capsuleCollider.height;
            if (capsuleCollider.center != Vector3.zero)
            {
                Log.Error("CapsuleCollider for " + bodyPrefab + " has non-zero center. This WILL result in pathing issues for AI.");
            }
            kinematicCharacterMotor.CapsuleYOffset = 0f;

            kinematicCharacterMotor.DetectDiscreteCollisions = false;
            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.PreventSnappingOnLedges = false;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;

            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;

            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = new Vector3(0f, 0f, 1f);

            kinematicCharacterMotor.StepHandling = StepHandlingMethod.Standard;
            kinematicCharacterMotor.LedgeHandling = true;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.SafeMovement = false;
            #endregion

            #region SpitterDeathDanceController
            var spitterDeathDance = bodyPrefab.AddComponent<SpitterDeathDanceController>();
            spitterDeathDance.body = characterBody;
            spitterDeathDance.modelLocator = modelLocator;
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

            #endregion

            #region SetupBoxes

            var lemurianSurfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();

            var hurtBoxesTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "Hurtbox").ToArray();
            List<HurtBox> hurtBoxes = new List<HurtBox>();
            foreach (Transform t in hurtBoxesTransform)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = lemurianSurfaceDef;
            }

            var sniperHurtBoxes = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "SniperHurtbox").ToArray();
            foreach (Transform t in sniperHurtBoxes)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBox.isSniperTarget = true;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = lemurianSurfaceDef;
            }

            var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/MainHurtbox");
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = lemurianSurfaceDef;

            var hitBox = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head/Hitbox").gameObject.AddComponent<HitBox>();
            #endregion

            #region mdlSpitter
            var mdlSpitter = modelTransform.gameObject;

            #region AimAnimator
            // if you are having issues with AimAnimator,
            // * just add Additive Reference Pose for your pitch and yaw animations in the middle of the animation
            // * make both animations loop
            // * set them both to zero speed in your animation controller
            // * I haven't found how to add poses to "separate" animation files, so those have to be in fbx
            var aimAnimator = mdlSpitter.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBank;
            aimAnimator.directionComponent = characterDirection;

            aimAnimator.pitchRangeMin = -65f; // its looking up, not down, for fuck sake
            aimAnimator.pitchRangeMax = 65f;

            aimAnimator.yawRangeMin = -15f;
            aimAnimator.yawRangeMax = 15f;

            aimAnimator.pitchGiveupRange = 40f;
            aimAnimator.yawGiveupRange = 20f;

            aimAnimator.giveupDuration = 3f;

            aimAnimator.raisedApproachSpeed = 720f;
            aimAnimator.loweredApproachSpeed = 360f;
            aimAnimator.smoothTime = 0.1f;

            aimAnimator.fullYaw = false;
            aimAnimator.aimType = AimAnimator.AimType.Direct;

            aimAnimator.enableAimWeight = false;
            aimAnimator.UseTransformedAimVector = false;
            #endregion

            #region ChildLocator
            var childLocator = mdlSpitter.AddComponent<ChildLocator>();
            var ourChildLocator = mdlSpitter.GetComponent<OurChildLocator>();
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
            var hurtboxGroup = mdlSpitter.AddComponent<HurtBoxGroup>();
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
            if (!mdlSpitter.TryGetComponent<AnimationEvents>(out _))
            {
                mdlSpitter.AddComponent<AnimationEvents>();
            }
            #endregion

            #region DestroyOnUnseen
            mdlSpitter.AddComponent<DestroyOnUnseen>().cull = false;
            #endregion

            #region CharacterModel
            var characterModel = mdlSpitter.AddComponent<CharacterModel>();
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
                    renderer = gumsRenderer,
                    defaultMaterial = gumsRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethenderer,
                    defaultMaterial = teethenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = true,
                    hideOnDeath = false
                }
            };
            #endregion

            #region HitBoxGroupBite
            var hbgBite = mdlSpitter.AddComponent<HitBoxGroup>();
            hbgBite.groupName = "Bite";
            hbgBite.hitBoxes = new HitBox[] { hitBox };
            #endregion

            #region FootstepHandler
            FootstepHandler footstepHandler = null;
            if (!mdlSpitter.TryGetComponent(out footstepHandler))
            {
                footstepHandler = mdlSpitter.AddComponent<FootstepHandler>();
            }
            footstepHandler.enableFootstepDust = true;
            footstepHandler.baseFootstepString = "Play_lemurian_step";
            footstepHandler.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion();
            #endregion

            #region ModelPanelParameters
            var modelPanelParameters = mdlSpitter.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focusPoint;
            modelPanelParameters.cameraPositionTransform = cameraPosition;
            modelPanelParameters.modelRotation = new Quaternion(0, 0, 0, 1);
            modelPanelParameters.minDistance = 1.5f;
            modelPanelParameters.maxDistance = 6f;
            #endregion

            #region SkinDefs

            RenderInfo[] defaultRender = Array.ConvertAll(characterModel.baseRendererInfos, item => new RenderInfo
            {
                renderer = (SkinnedMeshRenderer)item.renderer,
                material = item.defaultMaterial,
                ignoreOverlays = item.ignoreOverlays

            });
            SkinDefs.Default = CreateSkinDef("skinSpitterDefault", mdlSpitter, defaultRender);

            RenderInfo[] lakesRender = new RenderInfo[]
            {
                new RenderInfo
                {
                    renderer = modelRenderer,
                    material = skinsLookup["matSpitterLakes"],
                    ignoreOverlays = false
                },
                new RenderInfo
                {
                    renderer = gumsRenderer,
                    material = skinsLookup["matSpitterGutsLakes"],
                    ignoreOverlays = true
                },
                new RenderInfo
                {
                    renderer = teethenderer,
                    material = skinsLookup["matSpitterLakes"],
                    ignoreOverlays = true
                }
            };
            SkinDefs.Lakes = CreateSkinDef("skinSpitterLakes", mdlSpitter, lakesRender, SkinDefs.Default);

            RenderInfo[] sulfurRender = new RenderInfo[]
            {
                new RenderInfo
                {
                    renderer = modelRenderer,
                    material = skinsLookup["matSpitterSulfur"],
                    ignoreOverlays = false
                },
                new RenderInfo
                {
                    renderer = gumsRenderer,
                    material = skinsLookup["matSpitterGutsSulfur"],
                    ignoreOverlays = true
                },
                new RenderInfo
                {
                    renderer = teethenderer,
                    material = skinsLookup["matSpitterSulfur"],
                    ignoreOverlays = true
                }
            };
            SkinDefs.Sulfur = CreateSkinDef("skinSpitterSulfur", mdlSpitter, sulfurRender, SkinDefs.Default);

            RenderInfo[] depthsRender = new RenderInfo[]
{
                new RenderInfo
                {
                    renderer = modelRenderer,
                    material = skinsLookup["matSpitterDepths"],
                    ignoreOverlays = false
                },
                new RenderInfo
                {
                    renderer = gumsRenderer,
                    material = skinsLookup["matSpitterGutsDepths"],
                    ignoreOverlays = true
                },
                new RenderInfo
                {
                    renderer = teethenderer,
                    material = skinsLookup["matSpitterDepths"],
                    ignoreOverlays = true
                }
};
            SkinDefs.Depths = CreateSkinDef("skinSpitterDepths", mdlSpitter, depthsRender, SkinDefs.Default);

            var modelSkinController = mdlSpitter.AddComponent<ModelSkinController>();
            modelSkinController.skins = new SkinDef[]
            {
                SkinDefs.Default,
                SkinDefs.Lakes,
                SkinDefs.Sulfur,
                SkinDefs.Depths
            };
            #endregion

            #endregion

            #region AimAssist
            var aimAssistTarget = bodyPrefab.transform.Find("ModelBase/mdlSpitter/AimAssist").gameObject.AddComponent<AimAssistTarget>();
            aimAssistTarget.point0 = headTransform;
            aimAssistTarget.point1 = rootTransform;
            aimAssistTarget.assistScale = 1f;
            aimAssistTarget.healthComponent = healthComponent;
            aimAssistTarget.teamComponent = teamComponent;
            #endregion

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public GameObject CreateSpitterMaster(GameObject masterPrefab, GameObject bodyPrefab)
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

            #region AISkillDriver_ChaseAndBiteOffNodegraphWhileSlowingDown
            var asdBiteSlowDown = masterPrefab.AddComponent<AISkillDriver>();
            asdBiteSlowDown.customName = "ChaseAndBiteOffNodegraphWhileSlowingDown";
            asdBiteSlowDown.skillSlot = SkillSlot.Secondary;

            asdBiteSlowDown.requiredSkill = null;
            asdBiteSlowDown.requireSkillReady = false;
            asdBiteSlowDown.requireEquipmentReady = false;
            asdBiteSlowDown.minUserHealthFraction = float.NegativeInfinity;
            asdBiteSlowDown.maxUserHealthFraction = float.PositiveInfinity;
            asdBiteSlowDown.minTargetHealthFraction = float.NegativeInfinity;
            asdBiteSlowDown.maxTargetHealthFraction = float.PositiveInfinity;
            asdBiteSlowDown.minDistance = 0f;
            asdBiteSlowDown.maxDistance = 3f;
            asdBiteSlowDown.selectionRequiresTargetLoS = true;
            asdBiteSlowDown.selectionRequiresOnGround = false;
            asdBiteSlowDown.selectionRequiresAimTarget = false;
            asdBiteSlowDown.maxTimesSelected = -1;

            asdBiteSlowDown.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdBiteSlowDown.activationRequiresTargetLoS = false;
            asdBiteSlowDown.activationRequiresAimTargetLoS = false;
            asdBiteSlowDown.activationRequiresAimConfirmation = false;
            asdBiteSlowDown.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdBiteSlowDown.moveInputScale = 0.4f;
            asdBiteSlowDown.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdBiteSlowDown.ignoreNodeGraph = true;
            asdBiteSlowDown.shouldSprint = false;
            asdBiteSlowDown.shouldFireEquipment = false;
            asdBiteSlowDown.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdBiteSlowDown.driverUpdateTimerOverride = 0.5f;
            asdBiteSlowDown.resetCurrentEnemyOnNextDriverSelection = false;
            asdBiteSlowDown.noRepeat = false;
            asdBiteSlowDown.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_ChaseAndBiteOffNodegraph
            var asdBite = masterPrefab.AddComponent<AISkillDriver>();
            asdBite.customName = "ChaseAndBiteOffNodegraph";
            asdBite.skillSlot = SkillSlot.Secondary;

            asdBite.requiredSkill = null;
            asdBite.requireSkillReady = false;
            asdBite.requireEquipmentReady = false;
            asdBite.minUserHealthFraction = float.NegativeInfinity;
            asdBite.maxUserHealthFraction = float.PositiveInfinity;
            asdBite.minTargetHealthFraction = float.NegativeInfinity;
            asdBite.maxTargetHealthFraction = float.PositiveInfinity;
            asdBite.minDistance = 0f;
            asdBite.maxDistance = 6f;
            asdBite.selectionRequiresTargetLoS = true;
            asdBite.selectionRequiresOnGround = false;
            asdBite.selectionRequiresAimTarget = false;
            asdBite.maxTimesSelected = -1;

            asdBite.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdBite.activationRequiresTargetLoS = false;
            asdBite.activationRequiresAimTargetLoS = false;
            asdBite.activationRequiresAimConfirmation = false;
            asdBite.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdBite.moveInputScale = 1f;
            asdBite.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdBite.ignoreNodeGraph = true;
            asdBite.shouldSprint = false;
            asdBite.shouldFireEquipment = false;
            asdBite.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdBite.driverUpdateTimerOverride = 0.5f;
            asdBite.resetCurrentEnemyOnNextDriverSelection = false;
            asdBite.noRepeat = false;
            asdBite.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_StrafeAndShootChargedSpit
            var asdChargedSpit = masterPrefab.AddComponent<AISkillDriver>();
            asdChargedSpit.customName = "StrafeAndShootChargedSpit";
            asdChargedSpit.skillSlot = SkillSlot.Special;

            asdChargedSpit.requiredSkill = null;
            asdChargedSpit.requireSkillReady = false;
            asdChargedSpit.requireEquipmentReady = false;
            asdChargedSpit.minUserHealthFraction = float.NegativeInfinity;
            asdChargedSpit.maxUserHealthFraction = float.PositiveInfinity;
            asdChargedSpit.minTargetHealthFraction = float.NegativeInfinity;
            asdChargedSpit.maxTargetHealthFraction = float.PositiveInfinity;
            asdChargedSpit.minDistance = 15f;
            asdChargedSpit.maxDistance = 60f;
            asdChargedSpit.selectionRequiresTargetLoS = true;
            asdChargedSpit.selectionRequiresOnGround = false;
            asdChargedSpit.selectionRequiresAimTarget = false;
            asdChargedSpit.maxTimesSelected = -1;

            asdChargedSpit.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdChargedSpit.activationRequiresTargetLoS = false;
            asdChargedSpit.activationRequiresAimTargetLoS = false;
            asdChargedSpit.activationRequiresAimConfirmation = true;
            asdChargedSpit.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            asdChargedSpit.moveInputScale = 1f;
            asdChargedSpit.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdChargedSpit.ignoreNodeGraph = true;
            asdChargedSpit.shouldSprint = false;
            asdChargedSpit.shouldFireEquipment = false;
            asdChargedSpit.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            asdChargedSpit.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdChargedSpit.driverUpdateTimerOverride = -1f;
            asdChargedSpit.resetCurrentEnemyOnNextDriverSelection = false;
            asdChargedSpit.noRepeat = false;
            asdChargedSpit.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_StrafeAndShootNormalSpit
            //var asdNormalSpit = masterPrefab.AddComponent<AISkillDriver>();
            //asdNormalSpit.customName = "StrafeAndShootNormalSpit";
            //asdNormalSpit.skillSlot = SkillSlot.Primary;

            //asdNormalSpit.requiredSkill = null;
            //asdNormalSpit.requireSkillReady = false;
            //asdNormalSpit.requireEquipmentReady = false;
            //asdNormalSpit.minUserHealthFraction = float.NegativeInfinity;
            //asdNormalSpit.maxUserHealthFraction = float.PositiveInfinity;
            //asdNormalSpit.minTargetHealthFraction = float.NegativeInfinity;
            //asdNormalSpit.maxTargetHealthFraction = float.PositiveInfinity;
            //asdNormalSpit.minDistance = 15f;
            //asdNormalSpit.maxDistance = 35f;
            //asdNormalSpit.selectionRequiresTargetLoS = true;
            //asdNormalSpit.selectionRequiresOnGround = false;
            //asdNormalSpit.selectionRequiresAimTarget = false;
            //asdNormalSpit.maxTimesSelected = -1;

            //asdNormalSpit.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            //asdNormalSpit.activationRequiresTargetLoS = false;
            //asdNormalSpit.activationRequiresAimTargetLoS = false;
            //asdNormalSpit.activationRequiresAimConfirmation = true;
            //asdNormalSpit.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            //asdNormalSpit.moveInputScale = 1f;
            //asdNormalSpit.aimType = AISkillDriver.AimType.AtMoveTarget;
            //asdNormalSpit.ignoreNodeGraph = true;
            //asdNormalSpit.shouldSprint = false;
            //asdNormalSpit.shouldFireEquipment = false;
            //asdNormalSpit.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            //asdNormalSpit.driverUpdateTimerOverride = -1f;
            //asdNormalSpit.resetCurrentEnemyOnNextDriverSelection = false;
            //asdNormalSpit.noRepeat = false;
            //asdNormalSpit.nextHighPriorityOverride = null;
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

        public GameObject CreateBiteEffect()
        {
            var clonedEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion().InstantiateClone("SpitterBiteEffect", false);
            var particleSystem = clonedEffect.GetComponentInChildren<ParticleSystem>();
            var main = particleSystem.main;
            main.startRotationX = new ParticleSystem.MinMaxCurve(0f, 0f);
            main.startRotationY = new ParticleSystem.MinMaxCurve(140f, 140f);

            return clonedEffect;
        }

        public GameObject CreateNormalSpitProjectile()
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("SpitterSimpleSpitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = EnemiesReturnsConfiguration.Spitter.NormalSpitSpeed.Value;

            return clonedProjectile;
        }

        public GameObject CreateChargedSpitProjectile(GameObject bigDotZone, GameObject chunk)
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 55f;

            if (clonedProjectile.TryGetComponent<ProjectileImpactExplosion>(out var component))
            {
                UnityEngine.Object.DestroyImmediate(component);
            };

            if (clonedProjectile.TryGetComponent<ProjectileSingleTargetImpact>(out var component2))
            {
                UnityEngine.Object.DestroyImmediate(component2);
            }

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = GetRecoloredSpitProjectileGhost();


            #region MainSpitZone

            var explosion = clonedProjectile.AddComponent<ProjectileImpactExplosionWithChildrenArray>();
            explosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            explosion.blastRadius = 6.5f * EnemiesReturnsConfiguration.Spitter.ChargedProjectileLargeDoTZoneScale.Value;
            explosion.blastDamageCoefficient = 1f;
            explosion.blastProcCoefficient = 1f;
            explosion.blastAttackerFiltering = AttackerFiltering.Default;
            explosion.bonusBlastForce = new Vector3(0f, 0f, 0f);
            explosion.canRejectForce = false;
            explosion.projectileHealthComponent = null;
            explosion.explosionEffect = null;

            explosion.fireChildren = true;
            explosion.childrenProjectilePrefab = chunk;
            explosion.childrenCount = 3;
            explosion.childrenDamageCoefficient = 1f;
            explosion.childredMinRollDegrees = 0f;
            explosion.childrenRangeRollDegrees = 360f;
            explosion.childrenMinPitchDegrees = 40f;
            explosion.childrenRangePitchDegrees = 150f;

            explosion.fireDoTZone = true;
            explosion.dotZoneProjectilePrefab = bigDotZone;
            explosion.dotZoneDamageCoefficient = 1f;
            explosion.dotZoneMinRollDegrees = 0f;
            explosion.dotZoneRangeRollDegrees = 0f;
            explosion.dotZoneMinPitchDegrees = 0f;
            explosion.dotZoneRangePitchDegrees = 0f;

            explosion.applyDot = false;

            explosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleSpitExplosion.prefab").WaitForCompletion();
            explosion.lifetimeExpiredSound = null;
            explosion.offsetForLifetimeExpiredSound = 0f;
            explosion.destroyOnEnemy = true;
            explosion.destroyOnWorld = true;
            explosion.impactOnWorld = true;
            explosion.timerAfterImpact = true;
            explosion.lifetime = 10f;
            explosion.lifetimeAfterImpact = 0f;
            explosion.lifetimeRandomOffset = 0f;
            explosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            #endregion

            return clonedProjectile;
        }

        public GameObject CreateChargedSpitSplitProjectile(GameObject smallPool)
        {
            var clonedProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitSplitProjectile", true);
            clonedProjectile.GetComponent<ProjectileSimple>().desiredForwardSpeed = 15f;

            clonedProjectile.layer = LayerIndex.fakeActor.intVal; // TODO: check it later

            if (clonedProjectile.TryGetComponent<ProjectileImpactExplosion>(out var component))
            {
                UnityEngine.Object.DestroyImmediate(component);
            };

            if (clonedProjectile.TryGetComponent<ProjectileSingleTargetImpact>(out var component2))
            {
                UnityEngine.Object.DestroyImmediate(component2);
            }

            clonedProjectile.GetComponent<ProjectileController>().ghostPrefab = GetRecoloredSpitProjectileGhost();

            var explosion = clonedProjectile.AddComponent<ProjectileImpactExplosion>();
            explosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            explosion.blastRadius = 6.5f * EnemiesReturnsConfiguration.Spitter.ChargedProjectileSmallDoTZoneScale.Value;
            explosion.blastDamageCoefficient = 1f;
            explosion.blastProcCoefficient = 1f;
            explosion.blastAttackerFiltering = AttackerFiltering.Default;
            explosion.bonusBlastForce = new Vector3(0f, 0f, 0f);
            explosion.canRejectForce = false;
            explosion.projectileHealthComponent = null;
            explosion.explosionEffect = null;

            explosion.fireChildren = true;
            explosion.childrenProjectilePrefab = smallPool;
            explosion.childrenCount = 1;
            explosion.childrenDamageCoefficient = 1f;
            explosion.minRollDegrees = 0f;
            explosion.rangeRollDegrees = 0;
            explosion.minPitchDegrees = 0;
            explosion.rangePitchDegrees = 0;

            explosion.applyDot = false;

            explosion.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleSpitExplosion.prefab").WaitForCompletion();
            explosion.lifetimeExpiredSound = null;
            explosion.offsetForLifetimeExpiredSound = 0f;
            explosion.destroyOnEnemy = false;
            explosion.destroyOnWorld = true;
            explosion.impactOnWorld = true;
            explosion.timerAfterImpact = true;
            explosion.lifetime = 10f;
            explosion.lifetimeAfterImpact = 0f;
            explosion.lifetimeRandomOffset = 0f;
            explosion.transformSpace = ProjectileImpactExplosion.TransformSpace.World;

            return clonedProjectile;
        }

        public GameObject CreateChargedSpitDoTZone()
        {
            var child = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitDoTZone", true);
            var dotZone = child.GetComponent<ProjectileDotZone>();
            dotZone.damageCoefficient = EnemiesReturnsConfiguration.Spitter.ChargedProjectileLargeDoTZoneDamage.Value;

            var value = EnemiesReturnsConfiguration.Spitter.ChargedProjectileLargeDoTZoneScale.Value;

            var projectileDamage = child.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            var decal = child.GetComponentInChildren<Decal>();
            if (decal)
            {
                decal.Material = SetupDoTZoneDecalMaterial();
            }

            child.transform.localScale = new Vector3(value, value, value);

            return child;
        }

        public GameObject CreatedChargedSpitSmallDoTZone()
        {
            var child = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitSmallDoTZone", true);
            var dotZone = child.GetComponent<ProjectileDotZone>();
            dotZone.damageCoefficient = EnemiesReturnsConfiguration.Spitter.ChargedProjectileSmallDoTZoneDamage.Value;

            var value = EnemiesReturnsConfiguration.Spitter.ChargedProjectileSmallDoTZoneScale.Value;

            var projectileDamage = child.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;


            var decal = child.GetComponentInChildren<Decal>();
            if (decal)
            {
                decal.Material = SetupDoTZoneDecalMaterial();
            }

            child.transform.localScale = new Vector3(value, value, value);

            return child;
        }

        private GameObject GetRecoloredSpitProjectileGhost()
        {
            var projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpitGhost.prefab").WaitForCompletion().InstantiateClone("SpitterChargedSpitProjectileGhost", false);
            var particle = projectileGhost.GetComponentInChildren<ParticleSystem>();
            if (particle)
            {
                var renderer = particle.gameObject.GetComponent<Renderer>();
                if (renderer)
                {
                    Material newMaterial = ContentProvider.MaterialCache.Find(item => item.name == "matSpitterSpit");
                    if (newMaterial == default(Material))
                    {
                        newMaterial = UnityEngine.Object.Instantiate(renderer.material);
                        newMaterial.name = "matSpitterSpit";
                        newMaterial.SetColor("_TintColor", new Color(1f, 0.1764f, 0f));
                        ContentProvider.MaterialCache.Add(newMaterial); // most likely need it because it will get destroyed otherwise
                    }

                    renderer.material = newMaterial;
                }
            }

            return projectileGhost;

        }
        
        private Material SetupDoTZoneDecalMaterial()
        {
            Material material = ContentProvider.MaterialCache.Find(item => item.name == "matSpitterAcidDecal");
            if (material == default(Material))
            {
                material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetleQueenAcidDecal.mat").WaitForCompletion());
                material.name = "matSpitterAcidDecal";
                material.SetColor("_Color", new Color(1f, 140f / 255f, 0f));
                ContentProvider.MaterialCache.Add(material);
            }

            return material;
        }

        #endregion

        #region SkillDefs

        internal SkillDef CreateBiteSkill()
        {
            var bite = ScriptableObject.CreateInstance<SkillDef>();
            (bite as ScriptableObject).name = "SpitterBodyBite";
            bite.skillName = "Bite";

            bite.skillNameToken = "ENEMIES_RETURNS_SPITTER_BITE_NAME";
            bite.skillDescriptionToken = "ENEMIES_RETURNS_SPITTER_BITE_DESCRIPTION";
            //bite.icon = ; yeah, right

            bite.activationStateMachineName = "Weapon";
            bite.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.Bite));
            bite.interruptPriority = EntityStates.InterruptPriority.Skill;

            bite.baseRechargeInterval = 1f;
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

        internal SkillDef CreateNormalSpitSkill()
        {
            var spit = ScriptableObject.CreateInstance<SkillDef>();
            (spit as ScriptableObject).name = "SpitterBodyNormalSpit";
            spit.skillName = "NormalSpit";

            spit.skillNameToken = "ENEMIES_RETURNS_SPITTER_NORMAL_SPIT_NAME";
            spit.skillDescriptionToken = "ENEMIES_RETURNS_SPITTER_NORMAL_SPIT_DESCRIPTION";
            //bite.icon = ; yeah, right

            spit.activationStateMachineName = "Weapon";
            spit.activationState = new EntityStates.SerializableEntityStateType(typeof(NormalSpit));
            spit.interruptPriority = EntityStates.InterruptPriority.Any;

            spit.baseRechargeInterval = 0f;
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

        internal SkillDef CreateChargedSpitSkill()
        {
            var chargedSpit = ScriptableObject.CreateInstance<SkillDef>();
            (chargedSpit as ScriptableObject).name = "SpitterBodyChargedSpit";
            chargedSpit.skillName = "Bite";

            chargedSpit.skillNameToken = "ENEMIES_RETURNS_SPITTER_CHARGED_SPIT_NAME";
            chargedSpit.skillDescriptionToken = "ENEMIES_RETURNS_SPITTER_CHARGED_SPIT_DESCRIPTION";
            //bite.icon = ; yeah, right

            chargedSpit.activationStateMachineName = "Weapon";
            chargedSpit.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.ChargeChargedSpit));
            chargedSpit.interruptPriority = EntityStates.InterruptPriority.Any; // not sure

            chargedSpit.baseRechargeInterval = EnemiesReturnsConfiguration.Spitter.ChargedProjectileCooldown.Value;
            chargedSpit.baseMaxStock = 1;
            chargedSpit.rechargeStock = 1;
            chargedSpit.requiredStock = 1;
            chargedSpit.stockToConsume = 1;

            chargedSpit.resetCooldownTimerOnUse = false;
            chargedSpit.fullRestockOnAssign = true;
            chargedSpit.dontAllowPastMaxStocks = false;
            chargedSpit.beginSkillCooldownOnSkillEnd = false;

            chargedSpit.cancelSprintingOnActivation = true;
            chargedSpit.forceSprintDuringState = false;
            chargedSpit.canceledFromSprinting = false;

            chargedSpit.isCombatSkill = true;
            chargedSpit.mustKeyPress = false;

            return chargedSpit;
        }

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
            card.directorCreditCost = EnemiesReturnsConfiguration.Spitter.DirectorCost.Value;
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

            return idrs;
        }

        private void Renamer(GameObject object1)
        {
        }
    }
}

