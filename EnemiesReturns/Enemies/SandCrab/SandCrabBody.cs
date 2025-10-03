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
using EnemiesReturns.PrefabSetupComponents.ModelComponents;
using HG;
using RoR2;
using RoR2.Mecanim;
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

            public static SkinDef Sandy;

            public static SkinDef Grassy;

            public static SkinDef Sulfur;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscSandCrabDefault;

            public static CharacterSpawnCard cscSandCrabSandy;

            public static CharacterSpawnCard cscSandCrabGrassy;

            public static CharacterSpawnCard cscSandCrabSulfur;
        }

        public static GameObject BodyPrefab;

        protected override bool AddHitBoxes => true;

        protected override bool AddRandomBlinks => true;

        protected override IRandomBlinkController.RandomBlinkParams RandomBlinkParams()
        {
            return new IRandomBlinkController.RandomBlinkParams(new string[] {"Blink2"})
            {
                blinkChancePerUpdate = 3f,
            };
        }

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
                baseMaxHealth = Configuration.SandCrab.BaseMaxHealth.Value,
                baseDamage = Configuration.SandCrab.BaseDamage.Value,
                baseMoveSpeed = Configuration.SandCrab.BaseMoveSpeed.Value,
                baseJumpCount = 1,
                baseJumpPower = 18f,
                baseArmor = Configuration.SandCrab.BaseArmor.Value,
                hullClassification = HullClassification.Golem,
                isChampion = false,
                autoCalculateStats = true,
                levelArmor = Configuration.SandCrab.LevelArmor.Value,
                levelMaxHealth = Configuration.SandCrab.LevelMaxHealth.Value,
                levelDamage = Configuration.SandCrab.LevelDamage.Value,
            };
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, Configuration.Spitter.DirectorCost.Value)
            {
                hullSize = HullClassification.Golem,
                skinDef = skin,
                bodyPrefab = bodyPrefab,
            });
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
                baseRechargeInterval = Configuration.SandCrab.SnipCooldown.Value,
            });
        }

        public SkillDef CreateBubblesSkill()
        {
            var mageSecondary = Addressables.LoadAssetAsync<SkillDef>(RoR2BepInExPack.GameAssetPaths.RoR2_Base_Mage.MageBodyNovaBomb_asset).WaitForCompletion();
            return CreateSkill(new SkillParams("SandCrabBubbles", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.SandCrab.Bubbles.ChargeBubbles)))
            {
                nameToken = "ENEMIES_RETURNS_SANDCRAB_BUBBLES_NAME",
                descriptionToken = "ENEMIES_RETURNS_SANDCRAB_BUBBLES_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = Configuration.SandCrab.BubbleCooldown.Value,
                icon = mageSecondary.icon,
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

            CharacterModel.RendererInfo[] sandyRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = sandCrabBodyRender,
                    defaultMaterial = ContentProvider.MaterialCache["mat3DSandCrabSandy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };
            SkinDefs.Sandy = Utils.CreateSkinDef("skinSandCrabSandy", modelPrefab, sandyRender, SkinDefs.Default);

            CharacterModel.RendererInfo[] grassyRenderer = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = sandCrabBodyRender,
                    defaultMaterial = ContentProvider.MaterialCache["mat3DSandCrabGrassy"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };
            SkinDefs.Grassy = Utils.CreateSkinDef("skinSandCrabGrassy", modelPrefab, grassyRenderer, SkinDefs.Default);

            CharacterModel.RendererInfo[] sulfurRenderer = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = sandCrabBodyRender,
                    defaultMaterial = ContentProvider.MaterialCache["mat3DSandCrabSulfur"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                }
            };
            SkinDefs.Sulfur = Utils.CreateSkinDef("skinSandCrabSulfur", modelPrefab, sulfurRenderer, SkinDefs.Default);
            return new SkinDef[] { SkinDefs.Default, SkinDefs.Sulfur, SkinDefs.Sandy, SkinDefs.Grassy };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.SandCrab.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.SandCrab.SandCrabMain)),
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
                baseFootstepString = "ER_SandCrab_Footstep_Play",
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion()
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_SandCrab_Death_Play"
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

            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(1.32436F, 2.02271F, 0.65471F),
                localAngles = new Vector3(0.64552F, 332.8454F, 2.59704F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(-1.37162F, 1.92747F, 0.63035F),
                localAngles = new Vector3(0.01501F, 19.59324F, 356.8479F),
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
                childName = "Body",
                localPos = new Vector3(-0.02235F, 0.36953F, -0.14712F),
                localAngles = new Vector3(270.1856F, 79.54275F, 280.4755F),
                localScale = new Vector3(0.62278F, 0.62278F, 0.62278F),
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
                childName = "HandL",
                localPos = new Vector3(0.00003F, 1.62468F, -0.09394F),
                localAngles = new Vector3(270F, 180F, 0F),
                localScale = new Vector3(0.21843F, 0.21843F, 0.21843F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupIce.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteIce.DisplayEliteIceCrown),
                childName = "HandR",
                localPos = new Vector3(0.00003F, 1.62468F, -0.09394F),
                localAngles = new Vector3(270F, 180F, 0F),
                localScale = new Vector3(0.21843F, 0.21843F, 0.21843F),
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
                childName = "LowerClawR",
                localPos = new Vector3(-0.17979F, 1.20928F, -0.11295F),
                localAngles = new Vector3(332.4663F, 270F, 180F),
                localScale = new Vector3(0.83845F, 0.83845F, 0.83845F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "UpperClawR",
                localPos = new Vector3(0.30083F, 1.46326F, -0.04479F),
                localAngles = new Vector3(346.7745F, 90F, 190.9407F),
                localScale = new Vector3(0.83845F, 0.83845F, 0.83845F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "LowerClawL",
                localPos = new Vector3(0.33626F, 1.32991F, -0.09579F),
                localAngles = new Vector3(346.228F, 90F, 180F),
                localScale = new Vector3(0.83845F, 0.83845F, 0.83845F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "UpperClawL",
                localPos = new Vector3(-0.45322F, 1.50453F, -0.07023F),
                localAngles = new Vector3(353.8856F, 270F, 180F),
                localScale = new Vector3(0.83845F, 0.83845F, 0.83845F),
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
                childName = "Body",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(2.67929F, 2.67929F, 2.67929F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "Head",
                localPos = new Vector3(0.00129F, 0.47192F, -0.75906F),
                localAngles = new Vector3(3.2099F, 180F, 180F),
                localScale = new Vector3(0.4F, 0.8F, 1.25926F),
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
                childName = "Head",
                localPos = new Vector3(-1.36749F, 2.14488F, -0.00409F),
                localAngles = new Vector3(298.5784F, 270F, 90F),
                localScale = new Vector3(0.45169F, 0.45169F, 0.45169F),
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
                localPos = new Vector3(0.00024F, 1.90176F, 0.13083F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(7.71541F, 7.71541F, 7.71541F),
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
                childName = "EyeL",
                localPos = new Vector3(-0.00013F, 0.04369F, -0.05252F),
                localAngles = new Vector3(54.35627F, 359.9225F, 0.2262F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupVoid.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteVoid.DisplayAffixVoid),
                childName = "EyeR",
                localPos = new Vector3(-0.00013F, 0.04369F, -0.05252F),
                localAngles = new Vector3(54.35627F, 359.9225F, 0.2262F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                childName = "Head",
                localPos = new Vector3(1.39025F, 2.00989F, -0.16073F),
                localAngles = new Vector3(359.4834F, 357.4366F, 335.0326F),
                localScale = new Vector3(0.2361F, 0.1F, 0.26953F),
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
                localPos = new Vector3(-0.00293F, 2.50199F, 1.92886F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(2.7016F, 2.7016F, 2.7016F),
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
                    childName = "Head",
                    localPos = new Vector3(0.00019F, 2.91577F, -0.4538F),
                    localAngles = new Vector3(270F, 0F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F),
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
                    localPos = new Vector3(0.0006F, 2.38265F, 0.1529F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(1F, 1F, 1F),
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

        protected override string ModelName() => "mdlSandCrab";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 15f,
                minDistance = 6f,
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
