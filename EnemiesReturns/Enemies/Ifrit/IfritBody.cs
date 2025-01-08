using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using EnemiesReturns.PrefabSetupComponents.ModelComponents;
using HG;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.Ifrit
{
    public class IfritBody : BodyBase
    {
        public struct Skills
        {
            public static SkillDef SummonPylon;
            public static SkillDef Hellzone;
            public static SkillDef FlameCharge;
            public static SkillDef Smash;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
            public static SkillFamily Special;
            public static SkillFamily Secondary;
            public static SkillFamily Utility;
        }

        public struct SkinDefs
        {
            public static SkinDef Default;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscIfritDefault;
        }

        public static GameObject BodyPrefab;

        protected override bool AddHitBoxes => true;

        protected override bool AddRandomBlinks => true;

        protected override bool AddSetStateOnHurt => false;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, ExplicitPickupDropTable droptable)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, log, droptable);

            var modelTransform = bodyPrefab.transform.Find("ModelBase/mdlIfrit");
            var mdlIfrit = modelTransform.gameObject;
            var childLocator = mdlIfrit.GetComponent<ChildLocator>();

            #region BisonSprintEffect
            var bisonBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bison/BisonBody.prefab").WaitForCompletion();
            var sprintEffectTransform = bisonBody.transform.Find("ModelBase/mdlBison/BisonArmature/ROOT/SprintEffect");
            var sprintEffectCopy = UnityEngine.GameObject.Instantiate(sprintEffectTransform.gameObject);
            sprintEffectCopy.transform.parent = modelTransform.Find("Armature");
            sprintEffectCopy.transform.localPosition = new Vector3(-0.7647f, 3.0132f, 0.0441f);
            sprintEffectCopy.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
            sprintEffectCopy.transform.localScale = new Vector3(2f, 2f, 2f);
            sprintEffectCopy.SetActive(false);

            ArrayUtils.ArrayAppend(ref childLocator.transformPairs, new ChildLocator.NameTransformPair { name = "SprintEffect", transform = sprintEffectCopy.transform });
            #endregion

            return body;
        }

        public SkillDef CreateHellzoneSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallAirstrikeAlt.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("IfritBodyHellzone", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.Hellzone.FireHellzoneStart)))
            {
                nameToken = "ENEMIES_RETURNS_IFRIT_HELLZONE_NAME",
                descriptionToken = "ENEMIES_RETURNS_IFRIT_HELLZONE_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.Ifrit.HellzoneCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateSummonPylonSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/Railgunner/RailgunnerBodyChargeSnipeSuper.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("IfritBodySummonPylon", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.SummonPylon)))
            {
                nameToken = "ENEMIES_RETURNS_IFRIT_SUMMON_PYLON_NAME",
                descriptionToken = "ENEMIES_RETURNS_IFRIT_SUMMON_PYLON_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.Ifrit.PillarCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateFlameChargeSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/Chef/ChefSear.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("IfritBodyFlameCharge", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.FlameCharge.FlameChargeBegin)))
            {
                nameToken = "ENEMIES_RETURNS_IFRIT_FLAME_CHARGE_NAME",
                descriptionToken = "ENEMIES_RETURNS_IFRIT_FLAME_CHARGE_DESCRIPTION",
                activationStateMachine = "Body",
                baseRechargeInterval = EnemiesReturns.Configuration.Ifrit.FlameChargeCooldown.Value,
                icon = iconSource.icon
            });
        }

        public SkillDef CreateSmashSkill()
        {
            var iconSource = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/FalseSon/FalseSonClubSlam.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("IfritWeaponSmash", new EntityStates.SerializableEntityStateType(typeof(Junk.ModdedEntityStates.Ifrit.Smash)))
            {
                nameToken = "ENEMIES_RETURNS_IFRIT_SMASH_NAME",
                descriptionToken = "ENEMIES_RETURNS_IFRIT_SMASH_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = 0f,
                icon = iconSource.icon
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyPrefab = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.Ifrit.DirectorCost.Value)
            {
                hullSize = HullClassification.BeetleQueen,
                forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn,
                occupyPosition = true,
                forbiddenAsBoss = false,
                skinDef = skin,
                bodyPrefab = bodyPrefab
            });
        }

        protected override IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return new IHitboxes.HitBoxesParams[]
            {
                new IHitboxes.HitBoxesParams
                {
                    groupName = "Smash",
                    pathsToTransforms = new string[] { "Armature/Root/Root_Pelvis_Control/SmashHitbox" }
                },
                new IHitboxes.HitBoxesParams
                {
                    groupName = "FlameCharge",
                    pathsToTransforms = new string[] { "Armature/Root/Root_Pelvis_Control/Spine/Spine.001/Neck/Head/Jaw/FlameChargeHitbox" }
                },
                new IAddHitboxes.HitBoxesParams
                {
                    groupName = "BodyCharge",
                    pathsToTransforms = new string[] { "Armature/Root/Root_Pelvis_Control/SmashHitbox" }
                }
            };
        }

        protected override ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.DeathState)));
        }

        protected override CharacterCameraParams CharacterCameraParams()
        {
            return Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardHuge.asset").WaitForCompletion();
        }

        protected override float CharacterDirectionTurnSpeed => EnemiesReturns.Configuration.Ifrit.TurnSpeed.Value;

        protected override IModelLocator.ModelLocatorParams ModelLocatorParams()
        {
            return new IModelLocator.ModelLocatorParams()
            {
                autoUpdateModelTransform = true,
                normalizeToFloor = true,
                normalSmoothdampTime = 0.5f,
                normalMaxAngleDelta = 35f
            };
        }

        protected override float MaxInteractionDistance => 4f;

        protected override ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams()
            {
                deathSound = "ER_Ifrit_Death_Play"
            };
        }

        protected override IKinematicCharacterMotor.KinemacitCharacterMotorParams KinemacitCharacterMotorParams()
        {
            return new IKinematicCharacterMotor.KinemacitCharacterMotorParams()
            {
                StableGroundLayers = LayerIndex.world.mask,
                MaxStepHeight = 1f,
            };
        }

        protected override IAimAnimator.AimAnimatorParams AimAnimatorParams()
        {
            return new IAimAnimator.AimAnimatorParams()
            {
                pitchRangeMin = -70f,
                pitchRangeMax = 70f,

                yawRangeMin = -135f,
                yawRangeMax = 135f,

                pitchGiveUpRange = 50f,
                yawGiveUpRange = 50f,

                giveUpDuration = 5f,

                raisedApproachSpeed = 180f,
                loweredApproachSpeed = 180f,
                smoothTime = 0.3f,
            };
        }

        protected override string ModelName() => "mdlIfrit";

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/Golem/sdLemurian.asset").WaitForCompletion();

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                pathToPoint0 = "ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Spine/Spine.001/Neck/Head",
                pathToPoint1 = "ModelBase/mdlIfrit/Armature/Root/Root_Pelvis_Control/Tail/Tail.001",
                assistScale = 3f
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_IFRIT_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                subtitleNameToken = "ENEMIES_RETURNS_IFRIT_BODY_SUBTITLE",
                bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage,
                mainRootSpeed = 33f,
                baseMaxHealth = EnemiesReturns.Configuration.Ifrit.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.Ifrit.BaseMoveSpeed.Value,
                baseAcceleration = 60f,
                baseJumpPower = EnemiesReturns.Configuration.Ifrit.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.Ifrit.BaseDamage.Value,
                baseArmor = EnemiesReturns.Configuration.Ifrit.BaseArmor.Value,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.Ifrit.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.Ifrit.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.Ifrit.LevelArmor.Value,
                bodyColor = new Color(1f, 0.6082f, 0f),
                isChampion = true
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            var particles = modelPrefab.gameObject.GetComponentsInChildren<ParticleSystemRenderer>();
            var material = ContentProvider.GetOrCreateMaterial("matIfritManeFire", CreateManeFiresMaterial);
            foreach (var particleComponent in particles)
            {
                particleComponent.material = material;
            }

            var modelRenderer = modelPrefab.transform.Find("Ifrit").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maneRenderer = modelPrefab.transform.Find("Flame mane").gameObject.GetComponent<SkinnedMeshRenderer>();
            var beardRenderer = modelPrefab.transform.Find("Flame beard").gameObject.GetComponent<SkinnedMeshRenderer>();

            var renderInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = maneRenderer,
                    defaultMaterial = maneRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = beardRenderer,
                    defaultMaterial = beardRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false
                }
            };
            foreach (var particleComponent in particles)
            {
                ArrayUtils.ArrayAppend(ref renderInfos, new CharacterModel.RendererInfo
                {
                    renderer = particleComponent,
                    defaultMaterial = particleComponent.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false,
                });
            }
            return new ICharacterModel.CharacterModelParams()
            {
                autoPopulateLightInfos = true,
                renderInfos = renderInfos,
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            var particles = modelPrefab.gameObject.GetComponentsInChildren<ParticleSystemRenderer>();

            var modelRenderer = modelPrefab.transform.Find("Ifrit").gameObject.GetComponent<SkinnedMeshRenderer>();
            var maneRenderer = modelPrefab.transform.Find("Flame mane").gameObject.GetComponent<SkinnedMeshRenderer>();
            var beardRenderer = modelPrefab.transform.Find("Flame beard").gameObject.GetComponent<SkinnedMeshRenderer>();

            var renderInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = maneRenderer,
                    defaultMaterial = maneRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                },
                new CharacterModel.RendererInfo
                {
                    renderer = beardRenderer,
                    defaultMaterial = beardRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false
                }
            };
            foreach (var particleComponent in particles)
            {
                ArrayUtils.ArrayAppend(ref renderInfos, new CharacterModel.RendererInfo
                {
                    renderer = particleComponent,
                    defaultMaterial = particleComponent.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off,
                    ignoreOverlays = true,
                    hideOnDeath = false,
                });
            }

            SkinDefs.Default = Utils.CreateSkinDef("skinIfritDefault", modelPrefab, renderInfos);
            return new SkinDef[] { SkinDefs.Default };
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams
                {
                    name = "Body",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.Ifrit.SpawnState)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterMain))
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
                baseFootstepString = "Play_beetle_queen_step",
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericHugeFootstepDust.prefab").WaitForCompletion()
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "Smash", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Secondary, "Hellzone", SkillSlot.Secondary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Utility, "FlameCharge", SkillSlot.Utility),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Special, "SummonPylon", SkillSlot.Special),
            };
        }

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 4f,
                maxDistance = 24f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

        protected override IRandomBlinkController.RandomBlinkParams RandomBlinkParams()
        {
            return new IRandomBlinkController.RandomBlinkParams(new string[] { "BlinkEye" })
            {
                blinkChancePerUpdate = 10f
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsIfrit";
            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "ShoulderR",
                localPos = new Vector3(-0.07618F, -0.22209F, -0.07389F),
                localAngles = new Vector3(356.5784F, 60.60427F, 303.4548F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "ShoulderL",
                localPos = new Vector3(0.02979F, -0.23431F, -0.0541F),
                localAngles = new Vector3(333.7023F, 319.7617F, 43.31199F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                childName = "Head",
                localPos = new Vector3(0.04621F, -0.3427F, -0.48813F),
                localAngles = new Vector3(329.0771F, 358.3423F, 183.6484F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                childName = "Head",
                localPos = new Vector3(0.06479F, -0.38449F, -0.49458F),
                localAngles = new Vector3(42.55269F, 173.9934F, 178.0236F),
                localScale = new Vector3(0.1F, 0.1F, 0.1F),
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
                childName = "Chest",
                localPos = new Vector3(0.1547F, -0.21215F, -0.27268F),
                localAngles = new Vector3(330.5753F, 141.3175F, 118.0065F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.12394F, 0.58573F, -0.3623F),
                localAngles = new Vector3(338.1842F, 225.1289F, 269.9162F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Pelvis",
                localPos = new Vector3(-0.11418F, 0.03336F, -0.08187F),
                localAngles = new Vector3(341.638F, 224.6317F, 258.2183F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.12395F, -0.24669F, -0.21695F),
                localAngles = new Vector3(338.1842F, 225.1289F, 269.9162F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(0.31959F, 0.56037F, -0.22525F),
                localAngles = new Vector3(336.4365F, 137.4821F, 109.3917F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "Pelvis",
                localPos = new Vector3(0.21452F, 0.02704F, -0.06636F),
                localAngles = new Vector3(357.0562F, 147.3994F, 121.6319F),
                localScale = new Vector3(0.7F, 0.7F, 0.7F),
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
                localPos = new Vector3(0F, 0.11885F, -0.18199F),
                localAngles = new Vector3(7.73144F, 180.0001F, 180F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.00001F, 0.75732F, -0.35454F),
                localAngles = new Vector3(7.73144F, 180.0001F, 180F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLunar.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLunar/DisplayEliteLunar, Fire.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(-0.00002F, -0.34168F, -0.561F),
                localAngles = new Vector3(7.73144F, 180.0001F, 180F),
                localScale = new Vector3(0.3F, 0.3F, 0.3F),
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
                childName = "Chest",
                localPos = new Vector3(0.02025F, 1.06304F, -0.35235F),
                localAngles = new Vector3(321.0952F, 175.1595F, 1.17263F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "Chest",
                localPos = new Vector3(0.00727F, 0.3707F, -0.44392F),
                localAngles = new Vector3(3.87917F, 179.9956F, 182.1576F),
                localScale = new Vector3(0.15F, 0.15F, 0.15F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "Pelvis",
                localPos = new Vector3(-0.22009F, 0.02119F, -0.10358F),
                localAngles = new Vector3(358.2295F, 178.8255F, 0.63453F),
                localScale = new Vector3(0.2F, 0.2F, 0.2F),
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
                localPos = new Vector3(-0.00464F, 0.09453F, 0.00552F),
                localAngles = new Vector3(296.9135F, 187.694F, 169.9966F),
                localScale = new Vector3(3F, 3F, 3F),
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
                childName = "Head",
                localPos = new Vector3(-0.01302F, 0.66335F, 0.04278F),
                localAngles = new Vector3(278.5223F, 352.4039F, 6.56596F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
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
                childName = "Chest",
                localPos = new Vector3(-0.03164F, 0.04501F, -0.42897F),
                localAngles = new Vector3(285.3486F, 351.2853F, 11.47881F),
                localScale = new Vector3(0.14951F, 0.14951F, 0.14951F),
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
                localPos = new Vector3(-0.01787F, 0.91068F, -0.17726F),
                localAngles = new Vector3(343.8055F, 180.3407F, 177.7121F),
                localScale = new Vector3(1.17694F, 1.17694F, 1.17694F),
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

        public Material CreateManeFiresMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/GreaterWisp/matGreaterWispFire.mat").WaitForCompletion());
            material.name = "matIfritManeFire";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            return material;
        }
    }
}
