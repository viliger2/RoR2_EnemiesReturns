using EnemiesReturns.Behaviors.JitterBonesStuff;
using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.ModdedEntityStates.SandCrab;
using EnemiesReturns.ModdedEntityStates.SandCrab.Snip;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Rewired.UI.ControlMapper.ControlMapper;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.SandCrab
{
    public class SandCrabBody : BodyBase
    {
        public struct Skills
        {
            public static SkillDef ClawSnip;

            public static SkillDef Bubbles;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;

            public static SkillFamily Secondary;

        }
        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscSandCrabDefault;
        }

        public static GameObject BodyPrefab;

        protected override bool AddHitBoxes => true;

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/mdlSandCrab/SandCrabArmature/Root/BaseButt/BaseMiddle1/BaseMiddle2/BaseHead",
                pathToPoint1 = "ModelBase/mdlSandCrab/SandCrabArmature/Root/BaseButt/BaseMiddle1"
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_SANDCRAB_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 20,
                baseAcceleration = 100f,
                baseMaxHealth = 480f,
                baseDamage = 16f,
                baseMoveSpeed = 7f,
                baseJumpCount = 1,
                baseJumpPower = 18f,
                hullClassification = HullClassification.Golem,
                isChampion = false,
                autoCalculateStats = true,
                levelMaxHealth = 144f,
                levelDamage = 3.2f,
            };
        }

        public SkillDef CreateClawSnipSkill()
        {
            var acridBite = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoBite.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("SandCrabClawSnip", new EntityStates.SerializableEntityStateType(typeof(ChargeSnip)))
            {
                nameToken = "ENEMIES_RETURNS_SANDCRAB_CLAW_SNIP_NAME",
                descriptionToken = "ENEMIES_RETURNS_SANDCRAB_CLAW_SNIP_DESCRIPTION",
                icon = acridBite.icon,
                activationStateMachine = "Weapon",
                baseRechargeInterval = 5f,
            });
        }

        public SkillDef CreateBubblesSkill()
        {
            return CreateSkill(new SkillParams("SandCrabBubbles", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.SandCrab.Bubbles.ChargeBubbles)))
            {
                nameToken = "ENEMIES_RETURNS_SANDCRAB_BUBBLES_NAME",
                descriptionToken = "ENEMIES_RETURNS_SANDCRAB_BUBBLES_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = 15f,
            });
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var sandCrabBodyRender = modelPrefab.transform.Find("Crabbo").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = sandCrabBodyRender,
                    defaultMaterial = sandCrabBodyRender.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender,
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var sandCrabBodyRender = modelPrefab.transform.Find("Crabbo").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = sandCrabBodyRender,
                    defaultMaterial = sandCrabBodyRender.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };
            SkinDefs.Default = Utils.CreateSkinDef("skinSandCrabDefault", modelPrefab, defaultRender);

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
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain)),
                },
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Weapon",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle))
                },
            };
        }

        protected override IFootStepHandler.FootstepHandlerParams FootstepHandlerParams()
        {
            return new IFootStepHandler.FootstepHandlerParams()
            {
                enableFootstepDust = true,
                //baseFootstepString = "Play_treeBot_step", // TODO: not treebot, its way too mechanic
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion()
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "ClawSnip", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "Bubbles", SkillSlot.Secondary)
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsSandCrab";

            return idrs;
        }

        protected override string ModelName() => "mdlSandCrab";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 6f,
                minDistance = 1.5f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return new IHitboxes.HitBoxesParams[]
            {
                new IHitboxes.HitBoxesParams
                {
                    groupName = "Snip",
                    pathsToTransforms = new string[] {"SandCrabArmature/HandIK.L/Hand.L/UpperClaw.L/Hitbox", "SandCrabArmature/HandIK.R/Hand.R/UpperClaw.R/Hitbox" } 
                    
                }
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Beetle/sdBeetleGuard.asset").WaitForCompletion();
    }
}
