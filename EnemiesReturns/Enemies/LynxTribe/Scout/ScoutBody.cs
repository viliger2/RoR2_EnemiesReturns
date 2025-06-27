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
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

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

            var fixer = result.transform.Find("ModelBase/mdlScout").gameObject.AddComponent<FixJitterBones>();
            fixer.bonesToFix = new string[] { "Mask" };

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
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsLynxScout";
            #region FireElite
            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(0.38985F, 0.73397F, 0.04572F),
                localAngles = new Vector3(354.7525F, 340F, 7.12234F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(-0.28959F, 0.62969F, 0.03302F),
                localAngles = new Vector3(354.7525F, 20.00001F, 351.6044F),
                localScale = new Vector3(-0.6F, 0.6F, 0.6F),
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
                childName = "Head",
                localPos = new Vector3(0F, -0.29125F, -0.36034F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.43975F, 0.43975F, 0.43975F),
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
                childName = "Head",
                localPos = new Vector3(0.00642F, 2.14257F, 0.05624F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.28734F, 0.28734F, 0.28734F),
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
                childName = "HandL",
                localPos = new Vector3(0.06583F, 0.69607F, 0.02299F),
                localAngles = new Vector3(304.4485F, 101.4077F, 352.4068F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "HandR",
                localPos = new Vector3(-0.04096F, 0.72314F, 0.02788F),
                localAngles = new Vector3(309.1044F, 268.8004F, 11.44461F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
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
                childName = "Head",
                localPos = new Vector3(-0.00128F, -0.27535F, -0.06339F),
                localAngles = new Vector3(79.43568F, 180F, 180F),
                localScale = new Vector3(1.3106F, 1.3106F, 1.3106F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "Chest",
                localPos = new Vector3(-0.00001F, -1.52425F, -0.86948F),
                localAngles = new Vector3(55.49585F, 180F, 180F),
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
                childName = "Mask",
                localPos = new Vector3(-1.09346F, 0.41562F, 0.00196F),
                localAngles = new Vector3(0F, 0F, 296.6296F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "Mask",
                localPos = new Vector3(0.9198F, 0.34908F, 0.00087F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F),
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
                childName = "Head",
                localPos = new Vector3(0.01316F, 0.53914F, -0.06672F),
                localAngles = new Vector3(7.00848F, 2.28661F, 1.77239F),
                localScale = new Vector3(5.14048F, 5.14048F, 5.14048F),
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
                childName = "Mask",
                localPos = new Vector3(0.00022F, 0.40399F, -0.09158F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.67812F, 0.67812F, 0.67812F),
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
                childName = "Mask",
                localPos = new Vector3(0.91919F, 0.55826F, -0.20917F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.1296F, 0.1F, 0.1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupBead.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteBead.DisplayEliteBeadSpike),
                childName = "Mask",
                localPos = new Vector3(-1.07526F, 0.62036F, -0.14901F),
                localAngles = new Vector3(29.24141F, 103.6988F, 80.57337F),
                localScale = new Vector3(0.1296F, 0.1F, 0.1F),
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
                childName = "Head",
                localPos = new Vector3(-0.0078F, 0.54248F, 1.55665F),
                localAngles = new Vector3(348.3606F, 359.6882F, 358.4651F),
                localScale = new Vector3(3.40115F, 3.40115F, 3.40115F),
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
                    followerPrefab = Enemies.Judgement.SetupJudgementPath.AeonianAnointedItemDisplay,
                    followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                    childName = "Head",
                    localPos = new Vector3(0.00642F, 2.14257F, 0.05624F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.28734F, 0.28734F, 0.28734F),
                    limbMask = LimbFlags.None
                });

                ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
                {
                    displayRuleGroup = displayRuleGroupAeonian,
                    keyAsset = Content.Equipment.EliteAeonian
                });
            }
            #endregion

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
