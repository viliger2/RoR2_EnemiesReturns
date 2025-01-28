using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Scout
{
    public class ScoutBody : BodyBase
    {
        public struct SkillFamilies
        {
            public static SkillFamily Primary;
        }

        public struct Skills
        {
            public static SkillDef DoubleSlash;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscLynxScoutDefault;
        }

        protected override bool AddHitBoxes => true;

        public static GameObject BodyPrefab;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite)
        {
            var result = base.AddBodyComponents(bodyPrefab, sprite);

            result.transform.Find("ModelBase/mdlScout/LynxScout/ROOT/Base/Spine2/Spine3/Neck/Head/Mask/HurtBox").GetComponent<SurfaceDefProvider>().surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Common/sdWood.asset").WaitForCompletion();

            return result;
        }

        protected override ISetStateOnHurt.SetStateOnHurtParams SetStateOnHurtParams()
        {
            return new ISetStateOnHurt.SetStateOnHurtParams("Body", new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtState)))
            {
                hitThreshold = 0.2f
            };
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/mdlScout/LynxScout/ROOT/Base/Spine2/Spine3/Neck/Head",
                pathToPoint1 = "ModelBase/mdlScout/LynxScout/ROOT"
            };
        }

        public SkillDef CreateDoubleSlashSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Croco/CrocoSlash.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("LynxScoutWeaponDoubleSlash", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Scout.DoubleSlash)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SCOUT_DOUBLE_SLASH_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SCOUT_DOUBLE_SLASH_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxScout.DoubleSlashCooldown.Value,
                icon = iconSource.icon
            });
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Scout_Death_Play"
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Scout.DeathState)));
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_LYNX_SCOUT_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 33f,
                baseMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxScout.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.LynxTribe.LynxScout.BaseMoveSpeed.Value,
                baseAcceleration = 30f,
                baseJumpPower = EnemiesReturns.Configuration.LynxTribe.LynxScout.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.LynxTribe.LynxScout.BaseDamage.Value,
                baseArmor = 0f,
                hullClassification = HullClassification.Human,
                bodyColor = new Color(72 / 255, 73 / 255, 109 / 255),
                isChampion = false,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxScout.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.LynxTribe.LynxScout.LevelDamage.Value,
                levelArmor = 0f
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxScoutMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxScoutClaws").gameObject.GetComponent<SkinnedMeshRenderer>();

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
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.LynxTribe.LynxScout.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                occupyPosition = false,
                skinDef = skin,
                bodyPrefab = bodyGameObject,
            });
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxScoutMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxScoutClaws").gameObject.GetComponent<SkinnedMeshRenderer>();

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
                }
            };

            SkinDefs.Default = Utils.CreateSkinDef("skinLynxScoutDefault", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Scout.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Scout.MainState)),
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
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "DoubleSlash", SkillSlot.Primary),
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>(); // TODO
            (idrs as ScriptableObject).name = "idrsLynxScout";

            return idrs;
        }

        protected override string ModelName() => "mdlScout";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 1.5f,
                maxDistance = 6f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return new IHitboxes.HitBoxesParams[]
            {
                new IHitboxes.HitBoxesParams
                {
                    groupName = "LeftSlash",
                    pathsToTransforms = new string[] { "LynxScout/ROOT/LeftSlashHitbox" }
                },
                new IHitboxes.HitBoxesParams
                {
                    groupName = "RightSlash",
                    pathsToTransforms = new string[] { "LynxScout/ROOT/RightSlashHitbox" }
                },
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();
    }
}
