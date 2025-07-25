using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.ModdedEntityStates.Swift.Dive;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.Swift
{
    public class SwiftBody : BodyBase
    {
        public struct Skills
        {
            public static SkillDef Dive;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscSwiftDefault;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        protected override bool AddHitBoxes => true;

        public static GameObject BodyPrefab; 

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var result = base.AddBodyComponents(bodyPrefab, sprite, log);

            var modelTransform = result.transform.Find("ModelBase/mdlSwift");
            var childLocator = modelTransform.gameObject.GetComponent<ChildLocator>();
            var printController = modelTransform.gameObject.AddComponent<PrintController>();
            printController.printTime = 2f;
            printController.printCurve = acdLookup["acdSwiftPrint"].curve;
            printController.disableWhenFinished = true;
            printController.materialPrintCutoffPostSkinApplying = true;

            printController.startingPrintHeight = 3f;
            printController.maxPrintHeight = 0f;
            printController.startingPrintBias = 0f;
            printController.maxPrintBias = 0f;
            printController.animateFlowmapPower = false;

            #region BisonSprintEffect
            var bisonBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bison/BisonBody.prefab").WaitForCompletion();
            var sprintEffectTransform = bisonBody.transform.Find("ModelBase/mdlBison/BisonArmature/ROOT/SprintEffect");
            var sprintEffectCopy = UnityEngine.GameObject.Instantiate(sprintEffectTransform.gameObject);
            sprintEffectCopy.transform.parent = modelTransform.Find("SwiftArmature");
            sprintEffectCopy.transform.localPosition = new Vector3(0f, -2.4f, -0.6f);
            sprintEffectCopy.transform.localRotation = Quaternion.Euler(70f, 180f, 180f);
            sprintEffectCopy.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            sprintEffectCopy.SetActive(false);

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "SprintEffect", transform = sprintEffectCopy.transform });
            #endregion

            return result;
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 1f,
                pathToPoint0 = "ModelBase/mdlSwift/AimAssist",
                pathToPoint1 = "ModelBase/mdlSwift/AimAssist",
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_SWIFT_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 0f,
                baseMaxHealth = 140f,
                baseMoveSpeed = 7f,
                baseAcceleration = 80f,
                baseJumpPower = 15f,
                baseDamage = 15f,
                baseAttackSpeed = 1f,
                baseJumpCount = 1,
                autoCalculateStats = true,
                levelMaxHealth = 45f,
                levelDamage = 3f,
                hullClassification = HullClassification.Human,
                bodyColor = Color.green
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MeshSwift").gameObject.GetComponent<SkinnedMeshRenderer>();
            modelRenderer.material.SetTexture("_PrintRamp", Addressables.LoadAssetAsync<Texture2D>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_ColorRamps.texRampHuntressSoft2_png).WaitForCompletion());

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(EntityStates.Vulture.FallingDeath)));
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MeshSwift").gameObject.GetComponent<SkinnedMeshRenderer>();
            modelRenderer.material.SetTexture("_PrintRamp", Addressables.LoadAssetAsync<Texture2D>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Common_ColorRamps.texRampHuntressSoft2_png).WaitForCompletion());

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            SkinDefs.Default = Utils.CreateSkinDef("skinSwiftDefault", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.GenericSpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain))
                },
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Weapon",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle))
                },
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "GrantFlight",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.FlyingVermin.Mode.GrantFlight)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.FlyingVermin.Mode.GrantFlight)),
                }
            };
        }

        protected override IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return new IHitboxes.HitBoxesParams[]
            {
                new IHitboxes.HitBoxesParams
                {
                    groupName = "Dive",
                    pathsToTransforms = new string[]
                    {
                        "SwiftArmature/ROOT/Base/DiveHitbox"
                    }
                }
            };
        }

        public SkillDef CreateDiveSkill()
        {
            // TODO: icon
            var groundedSkillDef = CreateNonGroundedSkill(new SkillParams("SwiftBodyDive", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Swift.Dive.DivePrep)))
            {
                nameToken = "ENEMIES_RETURNS_SWIFT_DIVE_NAME",
                descriptionToken = "ENEMIES_RETURNS_SWIFT_DIVE_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 5f // TODO
            });
            groundedSkillDef.shouldBeGrounded = false;

            return groundedSkillDef;
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            return new IFootStepHandler.FootstepHandlerParams
            {
                enableFootstepDust = false,
                baseFootliftString = "",
                footstepDustPrefab = null
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[] 
            { 
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "Dive", SkillSlot.Primary)
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsSwift";

            return idrs;
        }

        protected override string ModelName() => "mdlSwift";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 6f,
                minDistance = 1.5f,
                modelRotation = Quaternion.identity
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdGolem.asset").WaitForCompletion();
    }
}
