using EnemiesReturns.Behaviors.JitterBonesStuff;
using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

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
            foreach (var component in components)
            {
                component.surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();
            }

            prefab.transform.Find("ModelBase/mdlLynxTotem/LynxTotem/ROOT/Base_1/Stomach/Chest/Neck/Head/Mask/HurtBox").GetComponent<SurfaceDefProvider>().surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Common/sdWood.asset").WaitForCompletion();

            prefab.transform.Find("ModelBase/mdlLynxTotem/LynxTotem/ROOT/Base_1/Stomach/Chest/Neck/Head/Mask/").gameObject.AddComponent<JitterBoneBlacklist>();

            return prefab;
        }

        public SkillDef CreateSummonTribeSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoPassivePoison.asset").WaitForCompletion();
            return CreateSummonTribeSkill(new SkillParams("LynxTotemWeaponSummonTribe", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.SummonTribe)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_TRIBE_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_TRIBE_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonTribeCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateSummonStormsSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/Chef/ChefRolyPolyBoosted.asset").WaitForCompletion();
            return CreateTotemSkill(new SkillParams("LynxTotemWeaponSummonStorms", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.SummonStorm)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_STORMS_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_SUMMON_STORMS_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.SummonStormCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateSummonFirewallSkill()
        {
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
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallAirstrike.asset").WaitForCompletion();
            return CreateTotemSkill(new SkillParams("LynxTotemWeaponGroundpound", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.GroundpoundProjectile)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_GROUNDPOUND_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_GROUNDPOUND_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxTotem.GroundpoundCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateBurrowSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodySwap.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("LynxTotemBodyBurrow", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Totem.Burrow.Burrow)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_TOTEM_BURROW_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_TOTEM_BURROW_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 0f,
                icon = iconSource.icon
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
                deathSound = "ER_Totem_Death_Play",
                landingSound = "ER_Totem_Landing_PLay"
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
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsLynxTotem";
            #region FireElite
            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "BodyTop",
                localPos = new Vector3(0.76329F, 0.62547F, 0.00056F),
                localAngles = new Vector3(359.9644F, 344.8509F, 359.742F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "BodyTop",
                localPos = new Vector3(-0.82379F, 0.43643F, 0.00621F),
                localAngles = new Vector3(-0.00311F, 11.37079F, 0.04467F),
                localScale = new Vector3(-0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "ShamanHead",
                localPos = new Vector3(0.07161F, 0.00957F, 0.11201F),
                localAngles = new Vector3(12.68712F, 343.7514F, 356.4883F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "ShamanHead",
                localPos = new Vector3(-0.23389F, 0.00073F, 0.17249F),
                localAngles = new Vector3(12.92738F, 29.19057F, 6.99305F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteFire.EliteFireEquipment),
                displayRuleGroup = displayRuleGroupFire,
            });
            #endregion

            #region HauntedElite
            var displayRuleGroupHaunted = new DisplayRuleGroup();
            displayRuleGroupHaunted.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteHaunted.DisplayEliteStealthCrown),
                childName = "BodyMiddle2",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.62069F, 0.43975F, 0.43975F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupHaunted.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteHaunted.DisplayEliteStealthCrown),
                childName = "ShamanChest",
                localPos = new Vector3(0F, 0.75248F, 0.04488F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupHaunted,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteHaunted.EliteHauntedEquipment),
            });
            #endregion

            #region IceElite
            var displayRuleGroupIce = new DisplayRuleGroup();
            displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteIce.DisplayEliteIceCrown),
                childName = "BodyLow",
                localPos = new Vector3(0F, 2.79952F, -0.43159F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.28734F, 0.28734F, 0.28734F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteIce.DisplayEliteIceCrown),
                childName = "ShamanHead",
                localPos = new Vector3(0F, 0.47267F, 0.00298F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupIce,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteIce.EliteIceEquipment)
            });
            #endregion

            #region LightningElite
            var displayRuleGroupLightning = new DisplayRuleGroup();
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BodyLow",
                localPos = new Vector3(1.99687F, 0.75009F, 0.00357F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(1.15579F, 1.15579F, 1.15579F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BodyLow",
                localPos = new Vector3(-1.95838F, 0.86168F, 0.00179F),
                localAngles = new Vector3(0F, 270F, 0F),
                localScale = new Vector3(1.15F, 1.15F, 1.15F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BodyMiddle1",
                localPos = new Vector3(1.29815F, 0.25205F, 0.00479F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(1.45F, 1.45F, 1.45F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BodyMiddle1",
                localPos = new Vector3(-1.28672F, 0.20786F, 0.00435F),
                localAngles = new Vector3(0F, 270F, 0F),
                localScale = new Vector3(1.45F, 1.45F, 1.45F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BodyMiddle2",
                localPos = new Vector3(1.1576F, -0.00002F, 0F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(1.45F, 1.45F, 1.45F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BodyMiddle2",
                localPos = new Vector3(-1.21847F, -0.00001F, 0F),
                localAngles = new Vector3(0F, 270F, 0F),
                localScale = new Vector3(1.45F, 1.45F, 1.45F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLightning,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteLightning.EliteLightningEquipment)
            });
            #endregion

            #region LunarElite
            var displayRuleGroupLunar = new DisplayRuleGroup();
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarEye),
                childName = "BodyTop",
                localPos = new Vector3(0F, 0.077F, 0F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(3F, 2F, 3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "BodyLow",
                localPos = new Vector3(0F, 1.19209F, -1.54352F),
                localAngles = new Vector3(-0.00001F, 180F, 180F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupLunar,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteLunar.EliteLunarEquipment)
            });
            #endregion

            #region PoisonElite
            var displayRuleGroupPoison = new DisplayRuleGroup();
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "BodyLow",
                localPos = new Vector3(0F, 1.97459F, 0.53381F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "ShamanMask",
                localPos = new Vector3(0F, -0.24309F, -0.00397F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.15F, 0.15F, 0.15F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupPoison,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.ElitePoison.ElitePoisonEquipment)
            });
            #endregion

            #region EliteEarth
            var displayRuleGroupEarth = new DisplayRuleGroup();
            displayRuleGroupEarth.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteEarth.DisplayEliteMendingAntlers),
                childName = "BodyMiddle1",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(7.13422F, 7.13422F, 7.13422F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupEarth.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteEarth.DisplayEliteMendingAntlers),
                childName = "ShamanHead",
                localPos = new Vector3(0.00093F, 0.14427F, 0.04535F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(2F, 2F, 2F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupEarth,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteEarth.EliteEarthEquipment)
            });
            #endregion

            #region VoidElite
            var displayRuleGroupVoid = new DisplayRuleGroup();
            displayRuleGroupVoid.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteVoid.DisplayAffixVoid),
                childName = "BodyMiddle1",
                localPos = new Vector3(0F, 0.54748F, 0.55694F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.84412F, 0.84412F, 0.84412F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupVoid,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteVoid.EliteVoidEquipment)
            });
            #endregion

            #region BeadElite
            var displayRuleGroupBead = new DisplayRuleGroup();
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "BodyLow",
                localPos = new Vector3(-2.27061F, 0.63286F, -0.12408F),
                localAngles = new Vector3(0.00868F, 0.06395F, 63.83193F),
                localScale = new Vector3(0.14951F, 0.08494F, 0.1116F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "BodyLow",
                localPos = new Vector3(2.39226F, 0.50044F, 0.00747F),
                localAngles = new Vector3(0.18867F, 359.4086F, 306.608F),
                localScale = new Vector3(0.14951F, 0.08494F, 0.1116F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "ShamanMask",
                localPos = new Vector3(0F, -0.1428F, -0.00477F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.07196F, 0.05F, 0.06047F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupBead,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(RoR2BepInExPack.GameAssetPaths.RoR2_DLC2_Elites_EliteBead.EliteBeadEquipment_asset)
            });
            #endregion

            #region GoldElite
            var displayRuleGroupGold = new DisplayRuleGroup();
            displayRuleGroupGold.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteGold.DisplayEliteAurelioniteEquipment),
                childName = "BodyLow",
                localPos = new Vector3(-0.08902F, 0.78706F, 1.13371F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(4.38704F, 3.40115F, 3.40115F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupGold.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteGold.DisplayEliteAurelioniteEquipment),
                childName = "ShamanMask",
                localPos = new Vector3(-0.00864F, -0.33865F, 0.26125F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupGold,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteGold.EliteAurelioniteEquipment)
            });
            #endregion

            #region AeonianElite
            if (Configuration.Judgement.Judgement.Enabled.Value)
            {
                var displayRuleGroupAeonian = new DisplayRuleGroup();
                displayRuleGroupAeonian.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Enemies.Judgement.AnointedSkins.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "ShamanHead",
                    localPos = new Vector3(-0.0082F, 0.80843F, -0.41071F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.25063F, 0.25063F, 0.25063F),
                    limbMask = LimbFlags.None
                });

                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupAeonian,
                    keyAsset = Content.Equipment.EliteAeonian
                });
            }
            #endregion

            #region PartyHat
            if (Items.PartyHat.PartyHatFactory.ShouldThrowParty())
            {
                var displayRuleGroupPartyHat = new DisplayRuleGroup();
                displayRuleGroupPartyHat.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Items.PartyHat.PartyHatFactory.PartyHatDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "BodyTop",
                    localPos = new Vector3(-1.05297F, 0.35721F, 0.0011F),
                    localAngles = new Vector3(0.07354F, 359.789F, 45.20722F),
                    localScale = new Vector3(0.94643F, 0.94643F, 0.94643F),
                    limbMask = LimbFlags.None
                });
                displayRuleGroupPartyHat.AddDisplayRule(new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Items.PartyHat.PartyHatFactory.PartyHatDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "ShamanHead",
                    localPos = new Vector3(0F, 0.23787F, 0.00319F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.12214F, 0.17446F, 0.12214F),
                    limbMask = LimbFlags.None
                });
                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupPartyHat,
                    keyAsset = Content.Items.PartyHat
                });
            }
            #endregion

            return idrs;
        }

        protected override string ModelName() => "mdlLynxTotem";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 7.5f,
                maxDistance = 22f,
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
    }
}
