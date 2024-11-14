using EnemiesReturns.PrefabAPICompat;
using HG;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies
{
    public class TestFactory : CharacterFactory
    {
        public struct SkillFamilies
        {
            public static SkillFamily Primary;

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

        public override GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var modelBase = bodyPrefab.transform.Find("ModelBase");
            var modelTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter");
            var aimOrigin = bodyPrefab.transform.Find("AimOrigin");
            var capsuleCollider = bodyPrefab.GetComponent<CapsuleCollider>();
            var rigidBody = bodyPrefab.GetComponent<Rigidbody>();

            var animator = modelTransform.gameObject.GetComponent<Animator>();

            #region Body
            var crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            var cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardTall.asset").WaitForCompletion();

            AddNetworkIdentity(bodyPrefab);
            var direction = AddCharacterDirection(bodyPrefab, modelBase, animator, 120f);
            var characterMotor = AddCharacterMotor(bodyPrefab, new CharacterMotorParams(direction)
            {
                mass = 100f
            });
            var inputBank = AddInputBankTest(bodyPrefab);
            var characterBody = AddCharacterBody(bodyPrefab, new CharacterBodyParams("ENEMIES_RETURNS_SPITTER_BODY_NAME", crosshair, aimOrigin, sprite.texture, new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized)))
            {
                mainRootSpeed = 33f,
                baseMaxHealth = EnemiesReturns.Configuration.Spitter.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.Spitter.BaseMoveSpeed.Value,
                baseAcceleration = 40f,
                baseJumpPower = EnemiesReturns.Configuration.Spitter.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.Spitter.BaseDamage.Value,
                baseArmor = EnemiesReturns.Configuration.Spitter.BaseArmor.Value,

                levelMaxHealth = EnemiesReturns.Configuration.Spitter.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.Spitter.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.Spitter.LevelArmor.Value,

                hullClassification = HullClassification.Golem,
                bodyColor = new Color(0.737f, 0.682f, 0.588f)
            });
            AddCameraTargetParams(bodyPrefab, cameraParams);
            AddModelLocator(bodyPrefab, new ModelLocatorParams(modelTransform, modelBase));
            var esmBody = AddEntityStateMachine(bodyPrefab, "Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.SpawnState)), new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.SpitterMain)));
            var esmWeapon = AddEntityStateMachine(bodyPrefab, "Weapon", new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)), new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)));
            var primarySkill = AddGenericSkill(bodyPrefab, SkillFamilies.Primary, "NormalSpit", false);
            var secondarySkill = AddGenericSkill(bodyPrefab, SkillFamilies.Secondary, "Bite", false);
            var specialSkill = AddGenericSkill(bodyPrefab, SkillFamilies.Special, "ChargedSpit", false);
            AddSkillLocator(bodyPrefab, primarySkill, secondarySkill, null, specialSkill);
            var teamComponent = AddTeamComponent(bodyPrefab);
            var healthComponent = AddHealthComponent(bodyPrefab);
            AddInteractor(bodyPrefab, 3f);
            AddInteractionDriver(bodyPrefab);
            AddCharacterDeathBehavior(bodyPrefab, esmBody, new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath)), esmWeapon);
            AddCharacterNetworkTransform(bodyPrefab);
            AddNetworkStateMachine(bodyPrefab, esmBody, esmWeapon);
            AddDeathRewards(bodyPrefab, log);
            AddEquipmentSlot(bodyPrefab);
            AddKinematicCharacterMotor(bodyPrefab, new KinemacitCharacterMotorParams(capsuleCollider, rigidBody, characterMotor));
            AddSetStateOnHurt(bodyPrefab, new SetStateOnHurtParams(esmBody, new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtState)), esmWeapon));
            #endregion

            #region SetupHurtboxes
            var surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();
            var hurtBoxes = SetupHurtboxes(bodyPrefab, surfaceDef, healthComponent);
            #endregion

            #region Model
            var mdlSpitter = modelTransform.gameObject;

            var focusPoint = bodyPrefab.transform.Find("ModelBase/mdlSpitter/LogBookTarget");
            var cameraPosition = bodyPrefab.transform.Find("ModelBase/mdlSpitter/LogBookTarget/LogBookCamera");

            var modelRenderer = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Spitter").gameObject.GetComponent<SkinnedMeshRenderer>();
            var gumsRenderer = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Spitter Gums").gameObject.GetComponent<SkinnedMeshRenderer>();
            var teethenderer = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Spitter Teeth").gameObject.GetComponent<SkinnedMeshRenderer>();

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
                    renderer = gumsRenderer,
                    defaultMaterial = gumsRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethenderer,
                    defaultMaterial = teethenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false
                }
            };

            var hitBoxBite = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head/Hitbox").gameObject.AddComponent<HitBox>();

            AddAimAnimator(mdlSpitter, new AimAnimatorParams(inputBank, direction)
            {
                pitchRangeMin = -65f,
                pitchRangeMax = 65f,

                yawRangeMin = -60f,
                yawRangeMax = 60f,

                pitchGiveUpRange = 40f,
                yawGiveUpRange = 20f,

                giveUpDuration = 3f
            });
            AddChildLocator(mdlSpitter);
            AddHurtBoxGroup(mdlSpitter, hurtBoxes);
            AddAnimationEvents(mdlSpitter);
            AddDestroyOnUnseen(mdlSpitter);
            AddCharacterModel(mdlSpitter, characterBody, CreateItemDisplayRuleSet(), renderInfos);
            AddHitBoxGroup(mdlSpitter, "Bite", hitBoxBite);
            AddModelPanelParameters(mdlSpitter, focusPoint, cameraPosition, new Quaternion(0, 0, 0, 1f));
            AddFootstepHandler(mdlSpitter, new FootstepHandlerParams
            {
                baseFootstepString = "Play_lemurian_step",
                enableFootstepDust = true,
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion(),
            });
            AddModelSkinController(mdlSpitter, new SkinDef[]
            {
                SkinDefs.Default,
                SkinDefs.Sulfur,
                SkinDefs.Lakes,
                SkinDefs.Lakes
            });
            #endregion

            var aimAssist = bodyPrefab.transform.Find("ModelBase/mdlSpitter/AimAssist").gameObject;

            var headTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head");
            var rootTransform = bodyPrefab.transform.Find("ModelBase/mdlSpitter/Armature/Root");

            AddAimAssist(aimAssist, headTransform, rootTransform, healthComponent, teamComponent);

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public override GameObject CreateMaster(GameObject masterPrefab, GameObject bodyPrefab)
        {
            AddNetworkIdentity(masterPrefab);
            AddCharacterMaster(masterPrefab, new CharacterMasterParams(bodyPrefab)
            {
                teamIndex = TeamIndex.Monster
            });
            AddInventory(masterPrefab);
            var esmAI = AddEntityStateMachine(masterPrefab, "AI", new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander)), new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander)));
            AddBaseAI(masterPrefab, new BaseAIParams(RoR2.Navigation.MapNodeGroup.GraphType.Ground, esmAI, new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander))));
            AddMinionOwnership(masterPrefab);
            AddAISkillDriver(masterPrefab, new AISkillDriverParams("ChaseAndBiteOffNodegraphWhileSlowingDown")
            {
                skillSlot = SkillSlot.Secondary,
                minDistance = 0f,
                maxDistance = 3f,
                selectionRequiresTargetLoS = true,
                moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                moveInputScale = 0.4f,
                aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                ignoreNodeGraph = true,
                driverUpdateTimerOverride = 0.5f
            });
            AddAISkillDriver(masterPrefab, new AISkillDriverParams("ChaseAndBiteOffNodegraph")
            {
                skillSlot = SkillSlot.Secondary,
                minDistance = 0f,
                maxDistance = 6f,
                selectionRequiresTargetLoS = true,
                moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                ignoreNodeGraph = true,
                driverUpdateTimerOverride = 0.5f
            });
            AddAISkillDriver(masterPrefab, new AISkillDriverParams("StrafeAndShootChargedSpit")
            {
                skillSlot = SkillSlot.Special,
                minDistance = 15f,
                maxDistance = 60f,
                selectionRequiresTargetLoS = true,
                moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                activationRequiresAimConfirmation = true,
                movementType = RoR2.CharacterAI.AISkillDriver.MovementType.StrafeMovetarget,
                moveInputScale = 0.7f,
                aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
            });
            AddAISkillDriver(masterPrefab, new AISkillDriverParams("ChaseOffNodegraph")
            {
                skillSlot = SkillSlot.None,
                minDistance = 0f,
                maxDistance = 7f,
                selectionRequiresTargetLoS = true,
                moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget,
                ignoreNodeGraph = true
            });
            AddAISkillDriver(masterPrefab, new AISkillDriverParams("PathFromAfar")
            {
                skillSlot = SkillSlot.None,
                minDistance = 0f,
                maxDistance = float.PositiveInfinity,
                moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.CurrentEnemy,
                movementType = RoR2.CharacterAI.AISkillDriver.MovementType.ChaseMoveTarget,
                aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtMoveTarget
            });

            return masterPrefab;
        }

        private void CreateSkinDefs(GameObject mdlSpitter, CharacterModel characterModel, SkinnedMeshRenderer modelRenderer, SkinnedMeshRenderer gumsRenderer, SkinnedMeshRenderer teethenderer)
        {
            SkinDefs.Default = Utils.CreateSkinDef("skinSpitterDefault", mdlSpitter, characterModel.baseRendererInfos);

            CharacterModel.RendererInfo[] lakesRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterLakes"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = gumsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterGutsLakes"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterLakesTeeth"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
            };
            SkinDefs.Lakes = Utils.CreateSkinDef("skinSpitterLakes", mdlSpitter, lakesRender, SkinDefs.Default);

            CharacterModel.RendererInfo[] sulfurRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterSulfur"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = gumsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterGutsSulfur"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterSulfurTeeth"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
            };
            SkinDefs.Sulfur = Utils.CreateSkinDef("skinSpitterSulfur", mdlSpitter, sulfurRender, SkinDefs.Default);

            CharacterModel.RendererInfo[] depthsRender = new CharacterModel.RendererInfo[]
{
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterDepths"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = gumsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterGutsDepths"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterDepthsTeeth"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
};
            SkinDefs.Depths = Utils.CreateSkinDef("skinSpitterDepths", mdlSpitter, depthsRender, SkinDefs.Default);

        }

        private ItemDisplayRuleSet CreateItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsSpitter";
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

    }
}
