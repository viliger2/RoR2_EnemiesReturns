using EnemiesReturns.Components;
using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Helpers;
using EnemiesReturns.ModdedEntityStates.MechanicalSpider.Dash;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using HG;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.ItemDisplayRuleSet;

namespace EnemiesReturns.Enemies.MechanicalSpider
{
    public abstract class MechanicalSpiderBodyBase : BodyBase
    {
        public struct Skills
        {
            public static SkillDef DoubleShot;
            public static SkillDef Dash;
        }

        public struct SkillFamilies
        {
            public static SkillFamily Primary;
            public static SkillFamily Utility;
        }

        protected override bool AddExecuteSkillOnDamage => true;

        public override GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var body = base.AddBodyComponents(bodyPrefab, sprite, log);
            var model = body.transform.Find("ModelBase/mdlMechanicalSpider").gameObject;
            model.AddComponent<RemoveJitterBones>(); // TODO: in the future move it to IModel

            #region ParticleEffects
            var rightFrontLeg = body.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg1.1/SparkRightFrontLeg");
            var backLeftLeg = body.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg3.1/SparkBackLeftLeg");

            var brokenMissileDrone = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MissileDroneBroken.prefab").WaitForCompletion();
            var sparkGameObject = brokenMissileDrone.transform.Find("ModelBase/BrokenDroneVFX/Damage Point/Small Sparks, Point").gameObject;
            var smallSparksRight = UnityEngine.GameObject.Instantiate(sparkGameObject);
            smallSparksRight.transform.parent = rightFrontLeg;
            smallSparksRight.transform.localPosition = Vector3.zero;
            smallSparksRight.transform.localScale = new Vector3(2f, 2f, 2f);

            var smallSparksLeft = UnityEngine.GameObject.Instantiate(sparkGameObject);
            smallSparksLeft.transform.parent = backLeftLeg;
            smallSparksLeft.transform.localPosition = Vector3.zero;
            smallSparksLeft.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            smallSparksLeft.transform.localRotation = Quaternion.Euler(0f, 0f, 180f);

            var smoke = body.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Smoke");
            var smokeGameObject = brokenMissileDrone.transform.Find("ModelBase/BrokenDroneVFX/Damage Point/Smoke, Point").gameObject;
            var smokeCenter = UnityEngine.GameObject.Instantiate(smokeGameObject);
            smokeCenter.transform.parent = smoke;
            smokeCenter.transform.localPosition = Vector3.zero;
            smokeCenter.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            #endregion

            return body;
        }

        public SkillDef CreateDoubleShotSkill()
        {
            var tazer = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CaptainTazer.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("MechanicalSpiderWeaponDoubleShot", new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.OpenHatch)))
            {
                nameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DOUBLE_SHOT_NAME",
                descriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DOUBLE_SHOT_DESCRIPTION",
                activationStateMachine = "Weapon",
                baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.DoubleShotCooldown.Value,
                icon = tazer.icon
            });
        }

        public SkillDef CreateDashSkill()
        {
            var bodyRoll = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Commando/CommandoBodyRoll.asset").WaitForCompletion();
            return CreateSkill(new SkillParams("MechanicalSpiderSlideDash", new EntityStates.SerializableEntityStateType(typeof(DashStart)))
            {
                nameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DASH_NAME",
                descriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DASH_DESCRIPTION",
                activationStateMachine = "Slide",
                baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.DashCooldown.Value,
                icon = bodyRoll.icon
            });
        }

        public CharacterSpawnCard CreateCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            return CreateCard(new SpawnCardParams(name, master, EnemiesReturns.Configuration.MechanicalSpider.DirectorCost.Value)
            {
                hullSize = HullClassification.Human,
                occupyPosition = false
            });
        }

        protected override IAimAnimator.AimAnimatorParams AimAnimatorParams()
        {
            return new IAimAnimator.AimAnimatorParams()
            {
                pitchRangeMin = -65f,
                pitchRangeMax = 65f,

                yawRangeMin = -180f,
                yawRangeMax = 180f,
                fullYaw = true,

                pitchGiveUpRange = 40f,
                yawGiveUpRange = 20f,

                giveUpDuration = 3f,

                raisedApproachSpeed = 720f,
                loweredApproachSpeed = 360f,
                smoothTime = 0.1f,

                aimType = AimAnimator.AimType.Direct
            };
        }

        protected override IExecuteSkillOnDamage.ExecuteSkillOnDamageParams ExecuteSkillOnDamage()
        {
            return new IExecuteSkillOnDamage.ExecuteSkillOnDamageParams()
            {
                mainStateMachineName = "Body",
                skillToExecute = SkillSlot.Utility
            };
        }

        protected override string ModelName() => "mdlMechanicalSpider";

        protected override SurfaceDef SurfaceDef() => Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/RoboBallBoss/sdRoboBall.asset").WaitForCompletion();

        protected override IAimAssist.AimAssistTargetParams AimAssistTargetParams()
        {
            return new IAimAssist.AimAssistTargetParams()
            {
                pathToPoint0 = "ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Gun",
                pathToPoint1 = "ModelBase/mdlMechanicalSpider/SpiderArmature/Root",
                assistScale = 2f
            };
        }

        protected override ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Texture icon)
        {
            return new ICharacterBody.CharacterBodyParams("ENEMIES_RETURNS_MECHANICAL_SPIDER_BODY_NAME", GetCrosshair(), aimOrigin, icon, GetInitialBodyState())
            {
                bodyFlags = CharacterBody.BodyFlags.Mechanical,
                mainRootSpeed = 33f,
                baseMaxHealth = EnemiesReturns.Configuration.MechanicalSpider.BaseMaxHealth.Value,
                baseMoveSpeed = EnemiesReturns.Configuration.MechanicalSpider.BaseMoveSpeed.Value,
                baseAcceleration = 40f,
                baseJumpPower = EnemiesReturns.Configuration.MechanicalSpider.BaseJumpPower.Value,
                baseDamage = EnemiesReturns.Configuration.MechanicalSpider.BaseDamage.Value,
                baseArmor = EnemiesReturns.Configuration.MechanicalSpider.BaseArmor.Value,
                autoCalculateStats = true,
                levelMaxHealth = EnemiesReturns.Configuration.MechanicalSpider.LevelMaxHealth.Value,
                levelDamage = EnemiesReturns.Configuration.MechanicalSpider.LevelDamage.Value,
                levelArmor = EnemiesReturns.Configuration.MechanicalSpider.LevelArmor.Value,
                hullClassification = HullClassification.Human,
                bodyColor = new Color(0.5568628f, 0.627451f, 0.6745098f)
            };
        }

        protected override ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab)
        {
            return new ICharacterModel.CharacterModelParams();
        }

        protected override ICharacterMotor.CharacterMotorParams CharacterMotorParams()
        {
            return new ICharacterMotor.CharacterMotorParams()
            {
                mass = 100f
            };
        }

        protected override IModelLocator.ModelLocatorParams ModelLocatorParams()
        {
            return new IModelLocator.ModelLocatorParams()
            {
                normalizeToFloor = true,
                normalSmoothdampTime = 0.3f,
                normalMaxAngleDelta = 55f
            };
        }

        protected override IKinematicCharacterMotor.KinemacitCharacterMotorParams KinemacitCharacterMotorParams()
        {
            return new IKinematicCharacterMotor.KinemacitCharacterMotorParams()
            {
                MaxStepHeight = 0.2f,
                playerCharacter = true // thanks Randy
            };
        }

        protected override SkinDef[] CreateSkinDefs(GameObject modelPrefab)
        {
            return Array.Empty<SkinDef>();
        }

        protected override IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams()
        {
            return new IEntityStateMachine.EntityStateMachineParams[]
            {
                new IEntityStateMachine.EntityStateMachineParams()
                {
                    name = "Weapon",
                    initialState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                    mainState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle))
                },
                new IEntityStateMachine.EntityStateMachineParams()
                {
                    name = "Slide",
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
                baseFootstepString = "Play_treeBot_step",
                footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion()
            };
        }

        protected override IGenericSkill.GenericSkillParams[] GenericSkillParams()
        {
            return new IGenericSkill.GenericSkillParams[]
            {
                new IGenericSkill.GenericSkillParams(SkillFamilies.Primary, "DoubleShot", SkillSlot.Primary),
                new IGenericSkill.GenericSkillParams(SkillFamilies.Utility, "Dash", SkillSlot.Utility)
            };
        }

        protected override ItemDisplayRuleSet ItemDisplayRuleSet()
        {
            var idrs = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            (idrs as ScriptableObject).name = "idrsMechanicalSpider";

            #region FireElite
            var fireEquipDisplay = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteFire/DisplayEliteHorn.prefab").WaitForCompletion();

            var displayRuleGroupFire = new DisplayRuleGroup();
            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Root",
                localPos = new Vector3(0.24068F, 0.63384F, -0.21041F),
                localAngles = new Vector3(354.7525F, 340F, 7.12234F),
                localScale = new Vector3(0.4F, 0.4F, 0.4F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "Root",
                localPos = new Vector3(-0.12522F, 0.69532F, -0.28728F),
                localAngles = new Vector3(354.7525F, 20.00001F, 351.6044F),
                localScale = new Vector3(-0.4F, 0.4F, 0.4F),
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
                childName = "Body",
                localPos = new Vector3(-0.02389F, -0.00796F, -0.08039F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.35F, 0.35F, 0.35F),
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
                childName = "Root",
                localPos = new Vector3(0F, 0F, -0.09389F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.2F, 0.2F, 0.1F),
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
                childName = "FrontRightLeg",
                localPos = new Vector3(-0.00631F, 1.28985F, -0.03396F),
                localAngles = new Vector3(358.3381F, 234.5306F, 359.3984F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "BackRightLeg",
                localPos = new Vector3(0.00288F, 1.26802F, -0.00083F),
                localAngles = new Vector3(358.8173F, 330.7685F, 3.19376F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "FrontLeftLeg",
                localPos = new Vector3(0.01729F, 1.26377F, 0.00951F),
                localAngles = new Vector3(353.36F, 52.70969F, 0.77622F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "BackLeftLeg",
                localPos = new Vector3(-0.03144F, 1.28574F, 0.01988F),
                localAngles = new Vector3(357.4552F, 39.61292F, 353.5382F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                childName = "Root",
                localPos = new Vector3(0F, 0.6047F, -0.10589F),
                localAngles = new Vector3(3.2099F, 180F, 180F),
                localScale = new Vector3(0.4F, 0.8F, 0.4F),
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
                childName = "Root",
                localPos = new Vector3(0F, 0.42445F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.25F, 0.25F, 0.5F),
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
                childName = "Body",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(7.00848F, 2.28661F, 1.77239F),
                localScale = new Vector3(4F, 3F, 3F),
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
                childName = "Gun",
                localPos = new Vector3(0.00011F, 0.05419F, 0.0124F),
                localAngles = new Vector3(285.478F, 179.8397F, 180.3337F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
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
                childName = "Root",
                localPos = new Vector3(-0.07959F, 0.63481F, -0.08875F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.15F, 0.1F, 0.15F),
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
                childName = "Body",
                localPos = new Vector3(-0.07278F, 0.02729F, 1.08914F),
                localAngles = new Vector3(359.7283F, 356.8857F, 0.96676F),
                localScale = new Vector3(1.5F, 1F, 1F),
                limbMask = LimbFlags.None
            });

            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupGold,
                keyAsset = Addressables.LoadAssetAsync<EquipmentDef>("RoR2/DLC2/Elites/EliteAurelionite/EliteAurelioniteEquipment.asset").WaitForCompletion()
            });
            #endregion

            #region DroneWeaponDisplay1
            var displayRuleGroupDroneWeapon1 = new DisplayRuleGroup();
            displayRuleGroupDroneWeapon1.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponLauncher.prefab").WaitForCompletion(),
                childName = "Body",
                localPos = new Vector3(-0.85193F, -0.00001F, -0.01456F),
                localAngles = new Vector3(-0.00003F, 272.4402F, -0.00004F),
                localScale = new Vector3(0.8F, 0.5F, 0.8F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupDroneWeapon1.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponMinigun.prefab").WaitForCompletion(),
                childName = "Gun",
                localPos = new Vector3(0.1839F, 0.00148F, 0.00531F),
                localAngles = new Vector3(1.64742F, 6.47228F, 70.50268F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupDroneWeapon1,
                keyAsset = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/DroneWeapons/DroneWeaponsDisplay1.asset").WaitForCompletion()
            });
            #endregion

            #region DroneWeaponDisplay2
            var displayRuleGroupDroneWeapon2 = new DisplayRuleGroup();
            displayRuleGroupDroneWeapon2.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponLauncher.prefab").WaitForCompletion(),
                childName = "Body",
                localPos = new Vector3(-0.85193F, -0.00001F, -0.01456F),
                localAngles = new Vector3(-0.00003F, 272.4402F, -0.00004F),
                localScale = new Vector3(0.8F, 0.5F, 0.8F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupDroneWeapon2.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/DisplayDroneWeaponRobotArm.prefab").WaitForCompletion(),
                childName = "Gun",
                localPos = new Vector3(0F, -0.05897F, 0.13338F),
                localAngles = new Vector3(354.3834F, 233.6984F, 58.97221F),
                localScale = new Vector3(1F, 1F, 1F),
                limbMask = LimbFlags.None
            });
            ArrayUtils.ArrayAppend(ref idrs.keyAssetRuleGroups, new KeyAssetRuleGroup
            {
                displayRuleGroup = displayRuleGroupDroneWeapon2,
                keyAsset = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/DroneWeapons/DroneWeaponsDisplay2.asset").WaitForCompletion()
            });
            #endregion

            return idrs;
        }

        protected override CharacterCameraParams CharacterCameraParams()
        {
            return Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardTall.asset").WaitForCompletion();
        }

        protected override IModelPanelParameters.ModelPanelParams ModelPanelParams()
        {
            return new IModelPanelParameters.ModelPanelParams()
            {
                minDistance = 1.5f,
                maxDistance = 6f,
                modelRotation = new Quaternion(0, 0, 0, 1)
            };
        }

    }
}
