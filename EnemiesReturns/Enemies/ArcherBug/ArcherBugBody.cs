using EnemiesReturns.Behaviors.JitterBonesStuff;
using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Rewired.UI.ControlMapper.ControlMapper;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.ArcherBug
{
    public class ArcherBugBody : BodyBase
    {
        public struct Skills
        {
            public static SkillDef CausticSpit;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
            public static SkinDef Jungle;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscArcherBugDefault;
            public static CharacterSpawnCard cscArcherBugJungle;
        }

        public static GameObject BodyPrefab;

        public static GameObject StadiaJungleMeshPrefab;

        protected override bool AddFootstepHandler => false;

        protected override bool AddCharacterMotor => false;

        protected override bool AddCharacterDirection => false;

        protected override bool AddRigidbodyMotor => true;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var bodyObject = base.AddBodyComponents(bodyPrefab, sprite, log);

            bodyObject.transform.Find("ModelBase/Bug/ArcherBugArmature/ROOT/Base").gameObject.AddComponent<JitterBoneBlacklist>();

            return bodyObject;
        }

        public SkillDef CreateCausticSpitSkill()
        {
            var acridEpidemic = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoDisease.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("ArcherBugBodyCausticSpit", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.FireCausticSpit)))
            {
                nameToken = "ENEMIES_RETURNS_ARCHERBUG_CAUSTIC_SPIT_NAME",
                descriptionToken = "ENEMIES_RETURNS_ARCHERBUG_CAUSTIC_SPIT_DESCRIPTION",
                icon = acridEpidemic.icon,
                activationStateMachine = "Weapon",
                baseRechargeInterval = Configuration.ArcherBug.CausticSpitCooldown.Value
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, Configuration.ArcherBug.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                graphType = RoR2.Navigation.MapNodeGroup.GraphType.Air,
                forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn,
                occupyPosition = true,
                forbiddenAsBoss = false,
                skinDef = skin,
                bodyPrefab = bodyPrefab
            });
        }

        protected override IRigidBodyDirection.RigidbodyDirectionParams RigidbodyDirectionParams()
        {
            return new IRigidBodyDirection.RigidbodyDirectionParams()
            {
                aimDirection = Vector3.one,
                angularVelocityPID = new QuaternionPIDParams()
                {
                    customName = "Angular Velocity PID",
                    PID = new Vector3(5f, 1f, 0f),
                    inputQuat = Quaternion.identity,
                    targetQuat = Quaternion.identity,
                    gain = 3f
                },
                torquePID = new VectorPIDParams()
                {
                    customName = "torquePID",
                    PID = new Vector3(2f, 1f, 0f),
                    isAngle = true,
                    gain = 3f
                },
            };
        }

        protected override IRigidbodyMotor.RigidbodyMotorParams RigidbodyMotorParams()
        {
            return new IRigidbodyMotor.RigidbodyMotorParams()
            {
                forcePID = new VectorPIDParams()
                {
                    customName = "Force PID",
                    PID = new Vector3(3f, 0f, 0f),
                    isAngle = false,
                    gain = 1f
                },
                canTakeImpactDamage = true
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_ARCHERBUG_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 0,
                baseAcceleration = 100f,
                baseMaxHealth = Configuration.ArcherBug.BaseMaxHealth.Value,
                levelMaxHealth = Configuration.ArcherBug.LevelMaxHealth.Value,
                baseDamage = Configuration.ArcherBug.BaseDamage.Value,
                levelDamage = Configuration.ArcherBug.LevelDamage.Value,
                baseArmor = Configuration.ArcherBug.BaseArmor.Value,
                levelArmor = Configuration.ArcherBug.LevelArmor.Value,
                baseMoveSpeed = Configuration.ArcherBug.BaseMoveSpeed.Value,
                baseJumpCount = 1,
                baseJumpPower = Configuration.ArcherBug.BaseJumpPower.Value,
                isChampion = false,
                autoCalculateStats = true,
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var bugBodyRenderer = modelPrefab.transform.Find("Bug").gameObject.GetComponent<SkinnedMeshRenderer>();
            var bugWingsRenderer = modelPrefab.transform.Find("Wings").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = bugBodyRenderer,
                    defaultMaterial = bugBodyRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = bugWingsRenderer,
                    defaultMaterial = bugWingsRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },

            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                aliveLoopStart = "Play_item_use_bugWingFlapLoop",
                aliveLoopStop = "Stop_item_use_bugWingFlapLoop"
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var bugBodyRenderer = modelPrefab.transform.Find("Bug").gameObject.GetComponent<SkinnedMeshRenderer>();
            var bugWingsRenderer = modelPrefab.transform.Find("Wings").gameObject.GetComponent<SkinnedMeshRenderer>();

            CharacterModel.RendererInfo[] defaultRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = bugBodyRenderer,
                    defaultMaterial = bugBodyRenderer.material,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = bugWingsRenderer,
                    defaultMaterial = bugWingsRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },

            };
            SkinDefs.Default = Utils.CreateSkinDef("skinArcherBugDefault", modelPrefab, defaultRender);

            CharacterModel.RendererInfo[] jungleRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = bugBodyRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matArcherBugBodyStadiaJungle"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = bugWingsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matArcherBugWingStadiaJungle"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
            };

            SkinDefParams.MeshReplacement[] meshReplacements = new SkinDefParams.MeshReplacement[]
            {
                new SkinDefParams.MeshReplacement
                {
                    renderer = bugBodyRenderer,
                    meshAddress = new AssetReferenceT<Mesh>(""),
                    mesh = StadiaJungleMeshPrefab.transform.Find("Bug").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh
                },
                new SkinDefParams.MeshReplacement
                {
                    renderer = bugWingsRenderer,
                    meshAddress = new AssetReferenceT<Mesh>(""),
                    mesh = StadiaJungleMeshPrefab.transform.Find("Wings").gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh
                },
            };

            SkinDefs.Jungle = Utils.CreateSkinDef("skinArcherBugJungle", modelPrefab, jungleRender, SkinDefs.Default, null, meshReplacements);

            return new SkinDef[] { SkinDefs.Default, SkinDefs.Jungle };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.MainState)),
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
            return new IFootStepHandler.FootstepHandlerParams();
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/Bug/ArcherBugArmature/ROOT/Base/Body/Head",
                pathToPoint1 = "ModelBase/Bug/ArcherBugArmature/ROOT/Abdomen1/Abdomen2/Abdomen3/Abdomen3_end"
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
             {
                 new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "CausticSpit", SkillSlot.Primary),
             };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsArcherBug";

            #region FireElite
            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(0.14363F, 0.68922F, -0.20783F),
                localAngles = new Vector3(307.2305F, 327.3106F, 39.84308F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "Head",
                localPos = new Vector3(-0.01859F, 0.72466F, -0.18266F),
                localAngles = new Vector3(311.8723F, 33.87458F, 326.1819F),
                localScale = new Vector3(-0.2F, 0.2F, 0.2F),
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
                childName = "BottomBody",
                localPos = new Vector3(0.01843F, 1.292F, -0.28706F),
                localAngles = new Vector3(52.67337F, 359.362F, 0.1409F),
                localScale = new Vector3(0.25F, 0.25F, 0.25F),
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
                localPos = new Vector3(-0.04036F, 0.35801F, -0.69943F),
                localAngles = new Vector3(3.535F, 185.4618F, 186.8777F),
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
                childName = "UpperBody",
                localPos = new Vector3(0.00177F, 0.80229F, -0.47328F),
                localAngles = new Vector3(352.9117F, 177.1099F, 177.786F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "UpperBody",
                localPos = new Vector3(0F, 0.42249F, -0.43617F),
                localAngles = new Vector3(9.48563F, 179.9999F, 180F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "UpperBody",
                localPos = new Vector3(0F, 0.0424F, -0.39181F),
                localAngles = new Vector3(32.88057F, 179.9999F, 180F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "BugButt",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(272.2996F, 320.5991F, 25.09467F),
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
                localPos = new Vector3(0.00819F, 0.08174F, 0.00748F),
                localAngles = new Vector3(90F, 0F, 0F),
                localScale = new Vector3(1F, 1F, 1F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "UpperBody",
                localPos = new Vector3(0F, -0.00847F, -0.3497F),
                localAngles = new Vector3(76.88484F, 179.9998F, 179.9998F),
                localScale = new Vector3(0.5F, 0.3F, 0.5F),
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
                childName = "BottomBody",
                localPos = new Vector3(-0.02994F, 1.15267F, -0.73712F),
                localAngles = new Vector3(34.80247F, 180.7755F, 178.1361F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "BottomBody",
                localPos = new Vector3(-0.02852F, 1.49305F, -0.08955F),
                localAngles = new Vector3(326.5202F, 359.1678F, 0.65932F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                localPos = new Vector3(-0.00188F, 0.73608F, -0.26081F),
                localAngles = new Vector3(286.265F, 0F, 0F),
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
                childName = "Head",
                localPos = new Vector3(0.02022F, 0.81777F, -0.24143F),
                localAngles = new Vector3(304.5165F, 355.7076F, 6.99781F),
                localScale = new Vector3(0.35F, 0.35F, 0.35F),
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
                childName = "UpperBody",
                localPos = new Vector3(-0.05061F, 0.2975F, -0.45238F),
                localAngles = new Vector3(284.7676F, 179.9998F, 180.0001F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
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
                localPos = new Vector3(0.03033F, 1.17444F, -0.00549F),
                localAngles = new Vector3(283.9268F, 86.01854F, 272.4486F),
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
                    childName = "Head",
                    localPos = new Vector3(-0.03223F, 0.66718F, -1.12624F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.12202F, 0.12202F, 0.12202F),
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
                    localPos = new Vector3(-0.0086F, 0.4594F, -0.35924F),
                    localAngles = new Vector3(270F, 0F, 0F),
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

        protected override string ModelName() => "Bug";

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 6f,
                minDistance = 1.5f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.ArcherBugs.DeathState)));
        }

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Beetle/sdBeetleGuard.asset").WaitForCompletion();
    }
}
