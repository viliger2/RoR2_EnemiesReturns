using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.Junk.ModdedEntityStates.Spitter;
using EnemiesReturns.ModdedEntityStates.Spitter;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Spitter
{
    public class SpitterBody : BodyBase
    {
        public struct Skills
        {
            public static SkillDef NormalSpit;
            public static SkillDef Bite;
            public static SkillDef ChargedSpit;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
            public static SkillFamily Secondary;
            public static SkillFamily Special;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
            public static SkinDef Lakes;
            public static SkinDef Sulfur;
            public static SkinDef Depths;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscSpitterDefault;
            public static CharacterSpawnCard cscSpitterLakes;
            public static CharacterSpawnCard cscSpitterSulfur;
            public static CharacterSpawnCard cscSpitterDepths;
        }

        public static GameObject BodyPrefab;

        protected override bool AddHitBoxes => true;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, log);

            var danceController = body.AddComponent<SpitterDeathDanceController>();
            danceController.body = body.GetComponent<CharacterBody>();
            danceController.modelLocator = body.GetComponent<ModelLocator>();

            return body;
        }

        public SkillDef CreateChargedSpitSkill()
        {
            var acridEpidemic = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoDisease.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("SpitterBodyChargedSpit", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.ChargeChargedSpit)))
            {
                nameToken = "ENEMIES_RETURNS_SPITTER_CHARGED_SPIT_NAME",
                descriptionToken = "ENEMIES_RETURNS_SPITTER_CHARGED_SPIT_DESCRIPTION",
                icon = acridEpidemic.icon,
                activationStateMachine = "Body",
                baseRechargeInterval = Configuration.Spitter.ChargedProjectileCooldown.Value
            });
        }

        public SkillDef CreateNormalSpitSkill()
        {
            var crocoSpit = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoSpit.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("SpitterBodyNormalSpit", new EntityStates.SerializableEntityStateType(typeof(NormalSpit)))
            {
                nameToken = "ENEMIES_RETURNS_SPITTER_NORMAL_SPIT_NAME",
                descriptionToken = "ENEMIES_RETURNS_SPITTER_NORMAL_SPIT_DESCRIPTION",
                icon = crocoSpit.icon,
                activationStateMachine = "Weapon",
                baseRechargeInterval = 0f
            });
        }

        public SkillDef CreateBiteSkill()
        {
            var acridBite = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoBite.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("SpitterBodyBite", new EntityStates.SerializableEntityStateType(typeof(Bite)))
            {
                nameToken = "ENEMIES_RETURNS_SPITTER_BITE_NAME",
                descriptionToken = "ENEMIES_RETURNS_SPITTER_BITE_DESCRIPTION",
                icon = acridBite.icon,
                activationStateMachine = "Weapon",
                baseRechargeInterval = Configuration.Spitter.BiteCooldown.Value
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, Configuration.Spitter.DirectorCost.Value)
            {
                skinDef = skin,
                bodyPrefab = bodyPrefab,
            });
        }

        protected override string ModelName() => "mdlSpitter";

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Lemurian/sdLemurian.asset").WaitForCompletion();

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_SPITTER_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                mainRootSpeed = 33f,
                baseMaxHealth = Configuration.Spitter.BaseMaxHealth.Value,
                baseMoveSpeed = Configuration.Spitter.BaseMoveSpeed.Value,
                baseAcceleration = 40f,
                baseJumpPower = Configuration.Spitter.BaseJumpPower.Value,
                baseDamage = Configuration.Spitter.BaseDamage.Value,
                baseArmor = Configuration.Spitter.BaseArmor.Value,
                hullClassification = HullClassification.Golem,
                bodyColor = new Color(0.737f, 0.682f, 0.588f),
                isChampion = false,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.Spitter.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.Spitter.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.Spitter.LevelArmor.Value
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Spitter").gameObject.GetComponent<SkinnedMeshRenderer>();
            var gumsRenderer = modelPrefab.transform.Find("Spitter Gums").gameObject.GetComponent<SkinnedMeshRenderer>();
            var teethRenderer = modelPrefab.transform.Find("Spitter Teeth").gameObject.GetComponent<SkinnedMeshRenderer>();

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
                    renderer = gumsRenderer,
                    defaultMaterial = gumsRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethRenderer,
                    defaultMaterial = teethRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
            };

            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = defaultRender
            };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Spitter.SpitterMain)),
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
            return new IFootStepHandler.FootstepHandlerParams
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
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "NormalSpit", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "Bite", SkillSlot.Secondary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Special, "ChargedSpit", SkillSlot.Special),
            };
        }

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                maxDistance = 6f,
                minDistance = 1.5f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var modelRenderer = modelPrefab.transform.Find("Spitter").gameObject.GetComponent<SkinnedMeshRenderer>();
            var gumsRenderer = modelPrefab.transform.Find("Spitter Gums").gameObject.GetComponent<SkinnedMeshRenderer>();
            var teethRenderer = modelPrefab.transform.Find("Spitter Teeth").gameObject.GetComponent<SkinnedMeshRenderer>();

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
                    renderer = gumsRenderer,
                    defaultMaterial = gumsRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethRenderer,
                    defaultMaterial = teethRenderer.material,
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
            };
            SkinDefs.Default = Utils.CreateSkinDef("skinSpitterDefault", modelPrefab, defaultRender);

            CharacterModel.RendererInfo[] lakesRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterLakes"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = gumsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterGutsLakes"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterLakesTeeth"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
            };
            SkinDefs.Lakes = Utils.CreateSkinDef("skinSpitterLakes", modelPrefab, lakesRender, SkinDefs.Default);

            CharacterModel.RendererInfo[] sulfurRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterSulfur"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = gumsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterGutsSulfur"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterSulfurTeeth"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
            };
            SkinDefs.Sulfur = Utils.CreateSkinDef("skinSpitterSulfur", modelPrefab, sulfurRender, SkinDefs.Default);

            CharacterModel.RendererInfo[] depthsRender = new CharacterModel.RendererInfo[]
{
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterDepths"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = gumsRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterGutsDepths"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = teethRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matSpitterDepthsTeeth"],
                    ignoreOverlays = true,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    hideOnDeath = false
                }
};
            SkinDefs.Depths = Utils.CreateSkinDef("skinSpitterDepths", modelPrefab, depthsRender, SkinDefs.Default);

            return new SkinDef[] { SkinDefs.Default, SkinDefs.Lakes, SkinDefs.Sulfur, SkinDefs.Depths };
        }

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Spitter_Death_Play"
            };
        }

        protected override IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return new IHitboxes.HitBoxesParams[]
            {
                new IHitboxes.HitBoxesParams
                {
                    groupName = "Bite",
                    pathsToTransforms = new string[] {"Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head/Hitbox" }
                }
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsSpitter";
            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "JawR",
                localPos = new Vector3(-0.32686F, 2.51006F, -0.21041F),
                localAngles = new Vector3(354.7525F, 340F, 7.12234F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteFire.DisplayEliteHorn),
                childName = "JawL",
                localPos = new Vector3(-0.12522F, 2.55699F, -0.28728F),
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
                localPos = new Vector3(-0.36417F, 4.08597F, -0.81975F),
                localAngles = new Vector3(88.15041F, 342.9204F, 152.0255F),
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
                childName = "TailEnd",
                localPos = new Vector3(-0.00302F, 0.77073F, 0.00143F),
                localAngles = new Vector3(284.2227F, 198.9412F, 159.205F),
                localScale = new Vector3(1.15579F, 1.15579F, 1.15579F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "JawL",
                localPos = new Vector3(-0.25677F, 2.49244F, -0.13195F),
                localAngles = new Vector3(323.8193F, 261.7038F, 7.48606F),
                localScale = new Vector3(1.45f, 1.45f, 1.45f),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLightning.DisplayEliteRhinoHorn),
                childName = "JawR",
                localPos = new Vector3(-0.00804F, 2.49258F, -0.13194F),
                localAngles = new Vector3(322.8305F, 89.99672F, 7F),
                localScale = new Vector3(1.45f, 1.45f, 1.45f),
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
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteLunar.DisplayEliteLunarFire),
                childName = "Head",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
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
                childName = "JawL",
                localPos = new Vector3(0F, 0.38638F, -0.00001F),
                localAngles = new Vector3(0F, 270F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.ElitePoison.DisplayEliteUrchinCrown),
                childName = "JawR",
                localPos = new Vector3(0F, 0.38638F, -0.00001F),
                localAngles = new Vector3(0F, 90F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                localPos = new Vector3(-0.12323F, 2.48183F, -0.47279F),
                localAngles = new Vector3(7.00848F, 2.28661F, 1.77239F),
                localScale = new Vector3(4.42437F, 4.42437F, 4.42437F),
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
                localPos = new Vector3(0F, 1.1304F, 0.00001F),
                localAngles = new Vector3(0F, 0F, 0F),
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
                childName = "Chest",
                localPos = new Vector3(0.03071F, 0.76725F, -0.77548F),
                localAngles = new Vector3(285.3486F, 351.2853F, 11.4788F),
                localScale = new Vector3(0.14951F, 0.14951F, 0.14951F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupBead,
                keyAssetAddress = new RoR2.AddressableAssets.IDRSKeyAssetReference(ThanksRandy.EliteBead.EliteBeadEquipment)
            });
            #endregion

            #region GoldElite
            var displayRuleGroupGold = new DisplayRuleGroup();
            displayRuleGroupGold.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefabAddress = new AssetReferenceGameObject(ThanksRandy.EliteGold.DisplayEliteAurelioniteEquipment),
                childName = "Head",
                localPos = new Vector3(-0.08902F, 0.78706F, 1.13371F),
                localAngles = new Vector3(0F, 0F, 0F),
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
            var displayRuleGroupAeonian = new DisplayRuleGroup();
            displayRuleGroupAeonian.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Enemies.Judgement.SetupJudgementPath.AeonianAnointedItemDisplay,
                followerPrefabAddress = new UnityEngine.AddressableAssets.AssetReferenceGameObject(""),
                childName = "Head",
                localPos = new Vector3(-0.36417F, 4.08597F, -0.81975F),
                localAngles = new Vector3(88.15041F, 342.9204F, 152.0255F),
                localScale = new Vector3(0.28734F, 0.28734F, 0.28734F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupAeonian,
                keyAsset = Content.Equipment.EliteAeonian
            });
            #endregion

            return idrs;
        }

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                assistScale = 2f,
                pathToPoint0 = "ModelBase/mdlSpitter/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head",
                pathToPoint1 = "ModelBase/mdlSpitter/Armature/Root"
            };
        }
    }
}
