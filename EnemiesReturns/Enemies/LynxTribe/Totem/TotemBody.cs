using EnemiesReturns.Behaviors;
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

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class TotemBody : BodyBase
    {
        public struct SkillFamilies
        {
            public static SkillFamily Primary;
            public static SkillFamily Secondary;
            public static SkillFamily Utility;
            public static SkillFamily Special;
        }

        public struct Skills
        {
            public static SkillDef SummonTribe;
            public static SkillDef SummonStorms;
            public static SkillDef SummonFirewall;
            public static SkillDef Burrow;
            public static SkillDef Groundpound;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscLynxTotemDefault;
        }

        public static GameObject BodyPrefab;

        protected override bool AddSetStateOnHurt => false;

        protected override bool AddHitBoxes => true;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, ExplicitPickupDropTable droptable)
        {
            var prefab = base.AddBodyComponents(bodyPrefab, sprite, log, droptable);

            var groundpoundHitbox = prefab.transform.Find("ModelBase/mdlLynxTotem/LynxTotem/ROOT/GroundpoundHitbox");
            var scale = EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundRadius.Value;
            groundpoundHitbox.transform.localScale = new Vector3(scale, groundpoundHitbox.transform.localScale.y, scale);

            var components = prefab.transform.Find("ModelBase/mdlLynxTotem/LynxTotem/ROOT/Base_1").gameObject.GetComponentsInChildren<SurfaceDefProvider>();
            foreach(var component in components)
            {
                component.surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();
            }

            prefab.transform.Find("ModelBase/mdlLynxTotem/LynxTotem/ROOT/Base_1/Stomach/Chest/Neck/Head/Mask/HurtBox").GetComponent<SurfaceDefProvider>().surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Common/sdWood.asset").WaitForCompletion();

            return prefab;
        }

        public SkillDef CreateSummonTribeSkill()
        {
            // TODO: icon
            return CreateTotemSkill(new SkillParams("LynxTotemWeaponSummonTribe", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.SummonTribe)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_TRIBE_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_TRIBE_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonTribeCooldown.Value,
            });
        }

        public SkillDef CreateSummonStormsSkill()
        {
            // TODO: icon
            return CreateTotemSkill(new SkillParams("LynxTotemWeaponSummonStorms", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.SummonStorm)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_STORMS_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_STORMS_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonStormCooldown.Value,
            });
        }

        public SkillDef CreateSummonFirewallSkill()
        {
            // TODO: icon
            return CreateTotemSkill(new SkillParams("LynxTotemWeaponSummonFirewall", new EntityStates.SerializableEntityStateType(typeof(Junk.ModdedEntityStates.LynxTribe.Totem.SummonFirewall)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_FIREWALL_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_FIREWALL_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = 15f,
            });
        }

        public SkillDef CreateGroundpoundSkill()
        {
            // TODO: icon
            return CreateTotemSkill(new SkillParams("LynxTotemWeaponGroundpound", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_GROUNDPOUND_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_GROUNDPOUND_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundCooldown.Value,
            });
        }

        public SkillDef CreateBurrowSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxTotemBodyBurrow", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.Burrow.Burrow)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_BURROW_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_BURROW_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 0f
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.LynxTribe.LynxTotem.DirectorCost.Value)
            {
                hullSize = HullClassification.Golem,
                occupyPosition = true,
                skinDef = skin,
                bodyPrefab = bodyGameObject,
            });
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams() 
            { 
                assistScale = 4f,
                pathToPoint0 = "ModelBase/mdlLynxTotem/LynxTotem/ROOT",
                pathToPoint1 = "ModelBase/mdlLynxTotem/LynxTotem/ROOT/Base_1/Stomach/Chest/Neck/Head"
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.DeathState)));
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_LYNX_TOTEM_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                subtitleNameToken = "ENEMIES_RETURNS_LYNX_TITEM_BODY_SUBTITLE",
                bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage,
                mainRootSpeed = 7.5f,

                baseMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxTotem.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.LynxTribe.LynxTotem.BaseMoveSpeed.Value,
                baseAcceleration = 30f,
                baseJumpPower = EnemiesReturns.Configuration.LynxTribe.LynxTotem.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.LynxTribe.LynxTotem.BaseDamage.Value,
                baseArmor = EnemiesReturns.Configuration.LynxTribe.LynxTotem.BaseArmor.Value,
                hullClassification = HullClassification.BeetleQueen,
                bodyColor = new Color(64 / 255, 58 / 255, 48 / 255),
                isChampion = true,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.LynxTribe.LynxTotem.LevelArmor.Value
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var flowers = modelPrefab.transform.Find("Flowers").gameObject.GetComponent<SkinnedMeshRenderer>();
            var grass = modelPrefab.transform.Find("Grass").gameObject.GetComponent<SkinnedMeshRenderer>();
            var lynx = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var lynxMask = modelPrefab.transform.Find("LynxShamanMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var lynxWeapon = modelPrefab.transform.Find("LynxShamanWeapon").gameObject.GetComponent<SkinnedMeshRenderer>();
            var moss = modelPrefab.transform.Find("Moss").gameObject.GetComponent<SkinnedMeshRenderer>();
            var plant = modelPrefab.transform.Find("Plane").gameObject.GetComponent<SkinnedMeshRenderer>();
            var totem = modelPrefab.transform.Find("Totem").gameObject.GetComponent<SkinnedMeshRenderer>();
            var vines = modelPrefab.transform.Find("Vines").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = flowers,
                    defaultMaterial = flowers.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = grass,
                    defaultMaterial = grass.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = lynx,
                    defaultMaterial = lynx.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = lynxMask,
                    defaultMaterial = lynxMask.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = lynxWeapon,
                    defaultMaterial = lynxWeapon.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = moss,
                    defaultMaterial = moss.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = plant,
                    defaultMaterial = plant.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = totem,
                    defaultMaterial = totem.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = vines,
                    defaultMaterial = vines.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var flowers = modelPrefab.transform.Find("Flowers").gameObject.GetComponent<SkinnedMeshRenderer>();
            var grass = modelPrefab.transform.Find("Grass").gameObject.GetComponent<SkinnedMeshRenderer>();
            var lynx = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var lynxMask = modelPrefab.transform.Find("LynxShamanMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var lynxWeapon = modelPrefab.transform.Find("LynxShamanWeapon").gameObject.GetComponent<SkinnedMeshRenderer>();
            var moss = modelPrefab.transform.Find("Moss").gameObject.GetComponent<SkinnedMeshRenderer>();
            var plant = modelPrefab.transform.Find("Plane").gameObject.GetComponent<SkinnedMeshRenderer>();
            var totem = modelPrefab.transform.Find("Totem").gameObject.GetComponent<SkinnedMeshRenderer>();
            var vines = modelPrefab.transform.Find("Vines").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = flowers,
                    defaultMaterial = flowers.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = grass,
                    defaultMaterial = grass.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = lynx,
                    defaultMaterial = lynx.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = lynxMask,
                    defaultMaterial = lynxMask.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = lynxWeapon,
                    defaultMaterial = lynxWeapon.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = moss,
                    defaultMaterial = moss.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = plant,
                    defaultMaterial = plant.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = totem,
                    defaultMaterial = totem.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = vines,
                    defaultMaterial = vines.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
            };

            SkinDefs.Default = Utils.CreateSkinDef("skinLynxTotemDefault", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.MainState)),
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
                baseFootstepString = "Play_titanboss_step",
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion()
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Totem_Death_Play"
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "Groundpound", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "SummonTribe", SkillSlot.Secondary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Utility, "Burrow", SkillSlot.Utility),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Special, "SummonStorms", SkillSlot.Special),
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>(); // TODO
            (idrs as ScriptableObject).name = "idrsLynxTotem";

            return idrs;
        }

        protected override string ModelName() => "mdlLynxTotem";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams() // TODO
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
                    groupName = "Groundpound",
                    pathsToTransforms = new string[] { "LynxTotem/ROOT/GroundpoundHitbox" }
                }
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdGolem.asset").WaitForCompletion();

        protected SkillDef CreateTotemSkill(SkillParams skillParams)
        {
            var skill = ScriptableObject.CreateInstance<LynxTotemSkillDef>();
            (skill as ScriptableObject).name = skillParams.name;
            skill.skillName = skillParams.name;

            skill.skillNameToken = skillParams.nameToken;
            skill.skillDescriptionToken = skillParams.descriptionToken;
            skill.icon = skillParams.icon;

            skill.activationStateMachineName = skillParams.activationStateMachine;
            skill.activationState = skillParams.activationState;
            skill.interruptPriority = skillParams.interruptPriority;

            skill.baseRechargeInterval = skillParams.baseRechargeInterval;
            skill.baseMaxStock = skillParams.baseMaxStock;
            skill.rechargeStock = skillParams.rechargeStock;
            skill.requiredStock = skillParams.requiredStock;
            skill.stockToConsume = skillParams.stockToConsume;

            skill.resetCooldownTimerOnUse = skillParams.resetCooldownTimerOnUse;
            skill.fullRestockOnAssign = skillParams.fullRestockOnAssign;
            skill.dontAllowPastMaxStocks = skillParams.dontAllowPAstMaxStocks;
            skill.beginSkillCooldownOnSkillEnd = skillParams.beginSkillCooldownOnSkillEnd;

            skill.canceledFromSprinting = skillParams.canceledFromSprinting;
            skill.forceSprintDuringState = skillParams.forceSprintDuringState;
            skill.canceledFromSprinting = skillParams.canceledFromSprinting;

            skill.isCombatSkill = skillParams.isCombatSkill;
            skill.mustKeyPress = skillParams.mustKeyPress;

            return skill;
        }
    }
}
