using EnemiesReturns.Behaviors;
using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman;
using EnemiesReturns.Junk.ModdedEntityStates.LynxTribe.Shaman.Teleport;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

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

        public static GameObject BodyPrefab;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var prefab = base.AddBodyComponents(bodyPrefab, sprite, log);

            prefab.transform.Find("ModelBase/mdlLynxShaman/LynxArmature/Pelvis/Stomach/HurtBox").GetComponent<SurfaceDefProvider>().surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Common/sdWood.asset").WaitForCompletion();

            var fixer = prefab.transform.Find("ModelBase/mdlLynxShaman").gameObject.AddComponent<FixJitterBones>();
            fixer.bonesToFix = new string[] { "Mask" };

            return prefab;
        }

        public SkillDef CreateSummonProjectilesSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<VoidSurvivorSkillDef>("RoR2/DLC1/VoidSurvivor/CrushCorruption.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("LynxShamanBodySummonProjectiles", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.SummonTrackingProjectilesShotgun)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_PROJECTILES_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_PROJECTILES_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonProjectilesCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreatePushBackSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Treebot/TreebotBodyAimMortar2.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("LynxShamanBodyPushBack", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.PushBack)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_PUSH_BACK_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_PUSH_BACK_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.LynxTribe.LynxShaman.PushBackCooldown.Value,
                icon = iconSource.icon
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.LynxTribe.LynxShaman.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                occupyPosition = false,
                skinDef = skin,
                bodyPrefab = bodyGameObject
            });
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/mdlLynxShaman/LynxArmature/Pelvis/Stomach/Chest/Neck/Head",
                pathToPoint1 = "ModelBase/mdlLynxShaman/LynxArmature/Base"
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.LynxTribe.Shaman.InitialDeathState)));
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_LYNX_SHAMAN_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
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
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsLynxShaman";
            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Head",
                localPos = new Vector3(0.52546F, 0.66611F, 0.06048F),
                localAngles = new Vector3(7.99479F, 339.8835F, 2.29952F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Head",
                localPos = new Vector3(-0.31792F, 0.59662F, 0.01843F),
                localAngles = new Vector3(13.20294F, 20.47717F, 358.386F),
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
                childName = "StaffUpperPoint",
                localPos = new Vector3(0.00002F, 0.68331F, 2.0131F),
                localAngles = new Vector3(0F, 0F, 180F),
                localScale = new Vector3(0.57631F, 0.57631F, 0.57631F),
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
                childName = "StaffUpperPoint",
                localPos = new Vector3(0.00001F, 0.64719F, 2.87467F),
                localAngles = new Vector3(32.5183F, 180F, 180F),
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
                childName = "StaffUpperPoint",
                localPos = new Vector3(0.05823F, 1.04502F, 1.60727F),
                localAngles = new Vector3(314.7455F, 173.8412F, 184.827F),
                localScale = new Vector3(1F, 1F, 1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "StaffUpperPoint",
                localPos = new Vector3(0.01863F, 1.20251F, 2.31134F),
                localAngles = new Vector3(306.6501F, 174.53F, 183.1826F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "StaffUpperPoint",
                localPos = new Vector3(0.0066F, 1.4297F, 2.89564F),
                localAngles = new Vector3(301.8946F, 175.0792F, 181.8468F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "StaffUpperPoint",
                localPos = new Vector3(-0.01124F, 0.52077F, 1.79858F),
                localAngles = new Vector3(74.48669F, 301.0346F, 128.3235F),
                localScale = new Vector3(1F, 1F, 1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "StaffUpperPoint",
                localPos = new Vector3(-0.00062F, 0.866F, 2.4543F),
                localAngles = new Vector3(74.00014F, 301F, 128F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "StaffUpperPoint",
                localPos = new Vector3(0.01897F, 1.2743F, 3.09738F),
                localAngles = new Vector3(74.00003F, 301F, 128F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
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
                childName = "Chest",
                localPos = new Vector3(0F, -0.00002F, -2.05892F),
                localAngles = new Vector3(-0.00001F, 180F, 180F),
                localScale = new Vector3(1F, 1F, 1F),
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
                childName = "Head",
                localPos = new Vector3(0F, -0.5623F, 0.5377F),
                localAngles = new Vector3(0F, 0F, 0F),
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
                localPos = new Vector3(-0.12323F, 0.1773F, 0.12377F),
                localAngles = new Vector3(7.00848F, 2.28661F, 1.77239F),
                localScale = new Vector3(6.18577F, 6.18577F, 6.18577F),
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
                childName = "Mask",
                localPos = new Vector3(-1.0458F, -0.19581F, -0.15273F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.84337F, 0.84337F, 0.84337F),
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
                childName = "Mask",
                localPos = new Vector3(0F, 0.34524F, 0.03753F),
                localAngles = new Vector3(79.25854F, 179.9995F, 179.9996F),
                localScale = new Vector3(0.25F, 0.06993F, 0.1794F),
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
                localPos = new Vector3(-0.08902F, -1.73393F, 1.04336F),
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
                deathSound = ""
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();

        public SkillDef CreateTeleportSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFlyUp.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("LynxShamanBodyTeleport", new EntityStates.SerializableEntityStateType(typeof(TeleportStart)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_TELEPORT_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_TELEPORT_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 15f,
                baseMaxStock = 1,
                fullRestockOnAssign = true,
                stockToConsume = 1,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateSummonStormSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/Chef/ChefRolyPolyBoosted.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("LynxShamanBodySummonStorm", new EntityStates.SerializableEntityStateType(typeof(SummonStormSkill)))
            {
                nameToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_STORM_NAME",
                descriptionToken = "ENEMIES_RETURNS_LYNX_SHAMAN_SUMMON_STORM_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = 30f,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateTeleportFriendSkill()
        {
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
