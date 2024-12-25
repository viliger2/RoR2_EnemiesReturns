using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman;
using EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Enemies.LynxTribe.Shaman
{
    // TODO: maybe visual effects on death, like splinters flying off the mask and ground impact
    public class ShamanBody : BodyBase
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
            public static SkillDef SummonProjectiles;
            public static SkillDef TeleportFriend;
            public static SkillDef SummonStorm;
            public static SkillDef Teleport;
            public static SkillDef SummonLightning;
            public static SkillDef PushBack;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscLynxShamanDefault;
        }

        protected override bool AddRemoveJitterBones => true;

        public static GameObject BodyPrefab;

        public SkillDef CreateSummonProjectilesSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxShamanBodySummonProjectiles", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_PROJECTILES_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_PROJECTILES_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesCooldown.Value,
            });
        }

        public SkillDef CreatePushBackSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxShamanBodyPushBack", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.PushBack)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_PUSH_BACK_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_PUSH_BACK_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackCooldown.Value,
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.LynxTribe.LynxShaman.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                occupyPosition = false
            });
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/mdlLynxShaman/LynxArmature/Pelvis/Stomach/Chest/Neck/Head/Head_end",
                pathToPoint1 = "ModelBase/mdlLynxShaman/LynxArmature/Base"
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_LYNX_SHAMAN_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 33f,
                baseMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxShaman.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.LynxTribe.LynxShaman.BaseMoveSpeed.Value,
                baseAcceleration = 40f,
                baseJumpPower = EnemiesReturns.Configuration.LynxTribe.LynxShaman.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.LynxTribe.LynxShaman.BaseDamage.Value,
                baseArmor = EnemiesReturns.Configuration.LynxTribe.LynxShaman.BaseArmor.Value,
                hullClassification = HullClassification.Human,
                bodyColor = new Color(72 / 255, 73 / 255, 109 / 255),
                isChampion = false, 
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.LynxTribe.LynxShaman.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.LynxTribe.LynxShaman.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.LynxTribe.LynxShaman.LevelArmor.Value
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxShamanMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxShamanWeapon").gameObject.GetComponent<SkinnedMeshRenderer>();

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

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Lynx").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maskRenderer = modelPrefab.transform.Find("LynxShamanMask").gameObject.GetComponent<SkinnedMeshRenderer>();
            var weaponRenderer = modelPrefab.transform.Find("LynxShamanWeapon").gameObject.GetComponent<SkinnedMeshRenderer>();

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

            SkinDefs.Default = Utils.CreateSkinDef("skinLynxShamanDefault", modelPrefab, defaultRender);

            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.ShamanMainState)),
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
                new IGenericSkill.GenericSkillParams(SkillFamilies.Utility, "Teleport", SkillSlot.Utility),
                //new IGenericSkill.GenericSkillParams(SkillFamilies.Utility, "SummonLightning", SkillSlot.Utility),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Special, "SummonStorm", SkillSlot.Special),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "SummonProjectiles", SkillSlot.Primary),
                //new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "TeleportFriend", SkillSlot.Secondary)
                new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "PushBack", SkillSlot.Secondary)
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>(); // TODO
            (idrs as ScriptableObject).name = "idrsLynxShaman";

            return idrs;
        }

        protected override string ModelName() => "mdlLynxShaman";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 1.5f,
                maxDistance = 6f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Shaman_Death_Play"
            };
        }

        // TODO: separate mask and body surface defs
        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();

        public SkillDef CreateTeleportSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxShamanBodyTeleport", new EntityStates.SerializableEntityStateType(typeof(TeleportStart)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_TELEPORT_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_TELEPORT_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 15f,
                baseMaxStock = 1,
                fullRestockOnAssign = true,
                stockToConsume = 1,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill
            });
        }

        public SkillDef CreateSummonStormSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxShamanBodySummonStorm", new EntityStates.SerializableEntityStateType(typeof(SummonStormSkill)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_STORM_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_STORM_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormCooldown.Value,
            });
        }

        public SkillDef CreateTeleportFriendSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxShamanBodyTeleportFriend", new EntityStates.SerializableEntityStateType(typeof(TeleportFriend)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_TELEPORT_FRIEND_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_TELEPORT_FRIEND_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 15f,
            });
        }

        public SkillDef CreateSummonLightningSkill()
        {
            // TODO: icon
            return CreateSkill(new SkillParams("LynxShamanBodySummonLightning", new EntityStates.SerializableEntityStateType(typeof(SummonLightning)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_LIGHTNING_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_LIGHTNING_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 15f,
            });
        }

    }
}
