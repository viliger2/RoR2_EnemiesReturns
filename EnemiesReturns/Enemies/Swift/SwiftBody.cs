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
using static RoR2.ItemDisplayRuleSet;

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

            public static CharacterSpawnCard cscSwiftRallypoint;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;

            public static SkinDef RallypointDelta;
        }

        protected override bool AddHitBoxes => true;

        protected override bool AddFootstepHandler => false;

        public static GameObject BodyPrefab; 

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            var result = base.AddBodyComponents(bodyPrefab, sprite, log);

            var modelTransform = result.transform.Find("ModelBase/mdlSwift");
            var childLocator = modelTransform.gameObject.GetComponent<ChildLocator>();
            var printController = modelTransform.gameObject.AddComponent<PrintController>();
            printController.printTime = 2.2f;
            printController.printCurve = acdLookup["acdSwiftPrint"].curve;
            printController.disableWhenFinished = true;

            printController.startingPrintHeight = 3f;
            printController.maxPrintHeight = -0.5f;
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
                baseMaxHealth = Configuration.Swift.BaseMaxHealth.Value,
                baseMoveSpeed = Configuration.Swift.BaseMoveSpeed.Value,
                baseAcceleration = 80f,
                baseJumpPower = Configuration.Swift.BaseJumpPower.Value,
                baseDamage = Configuration.Swift.BaseDamage.Value,
                baseAttackSpeed = 1f,
                baseArmor = Configuration.Swift.BaseArmor.Value,
                baseJumpCount = 1,
                autoCalculateStats = true,
                levelMaxHealth = Configuration.Swift.LevelMaxHealth.Value,
                levelDamage = Configuration.Swift.LevelDamage.Value,
                levelArmor = Configuration.Swift.LevelArmor.Value,
                hullClassification = HullClassification.Human,
                bodyColor = Color.green
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MeshSwift").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.CreateAndReplaceMaterial("matSwift", SwiftStuff.ModifySwiftMaterial),
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
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Swift.DeathState)));
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("MeshSwift").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.CreateAndReplaceMaterial("matSwift", SwiftStuff.ModifySwiftMaterial),
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            SkinDefs.Default = Utils.CreateSkinDef("skinSwiftDefault", modelPrefab, defaultRender);

            CharacterModel.RendererInfo[] rallyPointRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.CreateAndReplaceMaterial("matSwiftRallypoint", SwiftStuff.ModifySwiftMaterial),
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };

            SkinDefs.RallypointDelta = Utils.CreateSkinDef("skinSwiftRallypoint", modelPrefab, rallyPointRender, SkinDefs.Default);

            return new SkinDef[] { SkinDefs.Default, SkinDefs.RallypointDelta };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Swift.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Swift.SwiftMain))
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

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, Configuration.Swift.DirectorCost.Value)
            {
                skinDef = skin,
                graphType = RoR2.Navigation.MapNodeGroup.GraphType.Air,
                bodyPrefab = bodyPrefab,
            });
        }

        public SkillDef CreateDiveSkill()
        {
            var banditShiv = Addressables.LoadAssetAsync<SkillDef>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Bandit2.Bandit2SerratedShivs_asset).WaitForCompletion();

            var groundedSkillDef = CreateNonGroundedSkill(new SkillParams("SwiftBodyDive", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Swift.Dive.DivePrep)))
            {
                nameToken = "ENEMIES_RETURNS_SWIFT_DIVE_NAME",
                descriptionToken = "ENEMIES_RETURNS_SWIFT_DIVE_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = Configuration.Swift.DiveCooldown.Value,
                icon = banditShiv.icon
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

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Swift_Death_PLay"
            };
        }

        protected override string ModelName() => "mdlSwift";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 10f,
                minDistance = 4f,
                modelRotation = Quaternion.identity
            };
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdGolem.asset").WaitForCompletion();

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsSwift";
            #region FireElite
            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(-0.14121F, 0.71248F, -0.0144F),
                localAngles = new Vector3(0.76565F, 274.1009F, 338.8569F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(-0.13038F, 0.65779F, -0.04318F),
                localAngles = new Vector3(1.02898F, 265.2742F, 24.55365F),
                localScale = new Vector3(-0.3F, 0.3F, 0.3F),
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
                childName = "Tail",
                localPos = new Vector3(0F, 0.28924F, 0.0297F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(0.15155F, 0.15155F, 0.15155F),
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
                localPos = new Vector3(-0.09469F, 0.83848F, 0F),
                localAngles = new Vector3(0F, 90F, 180F),
                localScale = new Vector3(0.1242F, 0.1242F, 0.1242F),
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
                childName = "Head",
                localPos = new Vector3(-0.00508F, 1.19158F, 0F),
                localAngles = new Vector3(322.6912F, 89.99993F, 0.00003F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "Head",
                localPos = new Vector3(0.2188F, 0.73708F, 0F),
                localAngles = new Vector3(346.5779F, 90F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "Body",
                localPos = new Vector3(0F, -0.04878F, 0.24789F),
                localAngles = new Vector3(341.1554F, 0.00003F, 180F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "Body",
                localPos = new Vector3(0F, 0.81991F, 0.11058F),
                localAngles = new Vector3(341.1554F, 0.00003F, 180F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                localPos = new Vector3(-0.09537F, 0.90982F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(1.5F, 1.5F, 1.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "Tail",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                childName = "Body",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.14128F, 0.23763F, 0.38217F),
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
                localPos = new Vector3(-0.04571F, 1.06402F, 0F),
                localAngles = new Vector3(270F, 270F, 0F),
                localScale = new Vector3(3F, 3F, 3F),
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
                childName = "Body",
                localPos = new Vector3(0F, 0.00376F, 0.14477F),
                localAngles = new Vector3(82.00278F, -0.00012F, -0.00013F),
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
                childName = "Body",
                localPos = new Vector3(0F, 0.10515F, 0.17047F),
                localAngles = new Vector3(63.21401F, 0F, 0F),
                localScale = new Vector3(0.06876F, 0.06249F, 0.09921F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupBead,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(RoR2BepInExPack.GameAssetPathsBetter.RoR2_DLC2_Elites_EliteBead.EliteBeadEquipment_asset)
            });
            #endregion

            #region GoldElite
            var displayRuleGroupGold = new DisplayRuleGroup();
            displayRuleGroupGold.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteGold.DisplayEliteAurelioniteEquipment),
                childName = "Head",
                localPos = new Vector3(0.2902F, 0.97956F, 0.01291F),
                localAngles = new Vector3(333.8489F, 87.26634F, 1.20558F),
                localScale = new Vector3(1.5F, 1.5F, 1.5F),
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
                    childName = "Body",
                    localPos = new Vector3(0F, -0.06897F, 0.77451F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.12789F, 0.12789F, 0.12789F),
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
                    childName = "Head",
                    localPos = new Vector3(0.18094F, 0.86591F, 0F),
                    localAngles = new Vector3(0.00005F, 0.00001F, 294.8212F),
                    localScale = new Vector3(0.28734F, 0.28734F, 0.28734F),
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
    }
}
