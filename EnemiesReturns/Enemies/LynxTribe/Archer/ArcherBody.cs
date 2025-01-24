using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Archer
{
    public class ArcherBody : BodyBase
    {
        public struct SkillFamilies
        {
            public static SkillFamily Primary;
        }

        public struct Skills
        {
            public static SkillDef Shot;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscLynxArcherDefault;
        }

        public static GameObject BodyPrefab;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite)
        {
            var result = base.AddBodyComponents(bodyPrefab, sprite);

            //var walkSpeed = result.AddComponent<WalkSpeedDebugHelper>();
            //walkSpeed.animator = bodyPrefab.GetComponentInChildren<Animator>();
            //walkSpeed.animationParameters = new string[] { "walkSpeedDebug" };

            return result;
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Archer_Death_Play"
            };
        }

        public SkillDef CreateShotSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxArcherWeaponShot", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Archer.FireArrow)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_ARCHER_SHOT_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_ARCHER_SHOT_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxArcher.FireArrowCooldown.Value,
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.LynxTribe.LynxArcher.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                occupyPosition = false,
                skinDef = skin,
                bodyPrefab = bodyGameObject,
            });
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/mdlLynxArcher/LynxArcher/Root/Base/Stomach/Chest/Neck/Head",
                pathToPoint1 = "ModelBase/mdlLynxArcher/LynxArcher/Root"
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Archer.DeathState)));
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_LYNX_ARCHER_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 33f,
                baseMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxArcher.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.LynxTribe.LynxArcher.BaseMoveSpeed.Value,
                baseAcceleration = 30f,
                baseJumpPower = EnemiesReturns.Configuration.LynxTribe.LynxArcher.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.LynxTribe.LynxArcher.BaseDamage.Value,
                baseArmor = 0f,
                hullClassification = HullClassification.Human,
                bodyColor = new Color(72 / 255, 73 / 255, 109 / 255),
                isChampion = false,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxArcher.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.LynxTribe.LynxArcher.LevelDamage.Value,
                levelArmor = 0f
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxArcherMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxArcherBow").gameObject.GetComponent<SkinnedMeshRenderer>();
            var arrowQuad1 = modelPrefab.transform.Find("LynxArcher/Root/Base/Stomach/Chest/Shoulder_L/UpperArm_L/LowerArm_L/Hand_L/BowBaseU/String/Arrow/ArrowQuads/Quad1").gameObject.GetComponent<MeshRenderer>();
            var arrowQuad2 = modelPrefab.transform.Find("LynxArcher/Root/Base/Stomach/Chest/Shoulder_L/UpperArm_L/LowerArm_L/Hand_L/BowBaseU/String/Arrow/ArrowQuads/Quad2").gameObject.GetComponent<MeshRenderer>();
            var matHuntressArrow = ContentProvider.MaterialCache.GetValueOrDefault("matLynxArcherArrow");

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = maskRenderer,
                    defaultMaterial = maskRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = weaponRenderer,
                    defaultMaterial = weaponRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = arrowQuad1,
                    defaultMaterial = matHuntressArrow,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                },
                new CharacterModel.RendererInfo
                {
                    renderer = arrowQuad2,
                    defaultMaterial = matHuntressArrow,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxArcherMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxArcherBow").gameObject.GetComponent<SkinnedMeshRenderer>();
            var arrowQuad1 = modelPrefab.transform.Find("LynxArcher/Root/Base/Stomach/Chest/Shoulder_L/UpperArm_L/LowerArm_L/Hand_L/BowBaseU/String/Arrow/ArrowQuads/Quad1").gameObject.GetComponent<MeshRenderer>();
            var arrowQuad2 = modelPrefab.transform.Find("LynxArcher/Root/Base/Stomach/Chest/Shoulder_L/UpperArm_L/LowerArm_L/Hand_L/BowBaseU/String/Arrow/ArrowQuads/Quad2").gameObject.GetComponent<MeshRenderer>();
            var matHuntressArrow = ContentProvider.MaterialCache.GetValueOrDefault("matLynxArcherArrow");

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = maskRenderer,
                    defaultMaterial = maskRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = weaponRenderer,
                    defaultMaterial = weaponRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = arrowQuad1,
                    defaultMaterial = matHuntressArrow,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                },
                new CharacterModel.RendererInfo
                {
                    renderer = arrowQuad2,
                    defaultMaterial = matHuntressArrow,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = true
                }
            };

            SkinDefs.Default = Utils.CreateSkinDef("skinLynxArcherDefault", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Archer.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Archer.MainState)),
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
            return new IFootStepHandler.FootstepHandlerParams()
            {
                enableFootstepDust = true,
                baseFootstepString = "Play_lemurian_step",
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion()
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "Shot", SkillSlot.Primary),
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>(); // TODO
            (idrs as ScriptableObject).name = "idrsLynxArcher";

            return idrs;
        }

        protected override string ModelName() => "mdlLynxArcher";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 1.5f,
                maxDistance = 6f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();
        
        
    }
}
