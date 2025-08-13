﻿using EnemiesReturns.Components.BodyComponents;
using EnemiesReturns.Components.BodyComponents.CharacterMotor;
using EnemiesReturns.Components.BodyComponents.NetworkedEntityStateMachine;
using EnemiesReturns.Components.BodyComponents.Skills;
using EnemiesReturns.Components.GeneralComponents;
using EnemiesReturns.Components.ModelComponents;
using EnemiesReturns.Components.ModelComponents.Hitboxes;
using EnemiesReturns.Components.ModelComponents.Hurtboxes;
using EnemiesReturns.Components.ModelComponents.Skins;
using EnemiesReturns.EditorHelpers;
using EnemiesReturns.Enemies.LynxTribe.Totem;
using EnemiesReturns.Enemies.Swift;
using EnemiesReturns.PrefabSetupComponents.BodyComponents;
using EnemiesReturns.PrefabSetupComponents.ModelComponents;
using RoR2;
using RoR2.Navigation;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.Components
{
    public abstract class BodyBase : IBody
    {
        protected abstract string ModelName();

        protected abstract SurfaceDef SurfaceDef();

        protected abstract SkinDef[] CreateSkinDefs(GameObject modelPrefab);

        protected abstract ItemDisplayRuleSet ItemDisplayRuleSet();

        protected abstract IGenericSkill.GenericSkillParams[] GenericSkillParams();

        protected abstract ICharacterModel.CharacterModelParams CharacterModelParams(GameObject modelPrefab);

        protected abstract ICharacterBody.CharacterBodyParams CharacterBodyParams(Transform aimOrigin, Sprite icon);

        protected abstract IEntityStateMachine.EntityStateMachineParams[] EntityStateMachineParams();

        protected abstract IFootStepHandler.FootstepHandlerParams FootstepHandlerParams();

        protected abstract IModelPanelParameters.ModelPanelParams ModelPanelParams();

        protected abstract IAimAssist.AimAssistTargetParams AimAssistTargetParams();

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab)
        {
            return AddBodyComponents(bodyPrefab, null, null, null, null);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite)
        {
            return AddBodyComponents(bodyPrefab, sprite, null, null, null);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            return AddBodyComponents(bodyPrefab, null, null, null, acdLookup);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            return AddBodyComponents(bodyPrefab, sprite, log, null, null);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, ExplicitPickupDropTable droptable)
        {
            return AddBodyComponents(bodyPrefab, sprite, log, droptable, null);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            return AddBodyComponents(bodyPrefab, sprite, null, null, acdLookup);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log, Dictionary<string, AnimationCurveDef> acdLookup)
        {
            return AddBodyComponents(bodyPrefab, sprite, log, null, acdLookup);
        }

        public virtual GameObject AddBodyComponents(GameObject bodyPrefab, Sprite sprite = null, UnlockableDef log = null, ExplicitPickupDropTable droptable = null, Dictionary<string, AnimationCurveDef> acdLookup = null)
        {
            var body = (this as IBody).CreateBody(bodyPrefab, sprite, log, droptable);
            return body;
        }

        protected virtual bool AddAimAnimator => true;
        protected virtual bool AddAnimationEvents => true;
        protected virtual bool AddCameraTargetParams => true;
        protected virtual bool AddCharacterBody => true;
        protected virtual bool AddCharacterDeathBehavior => true;
        protected virtual bool AddCharacterDirection => true;
        protected virtual bool AddCharacterModel => true;
        protected virtual bool AddCharacterMotor => true;
        protected virtual bool AddCharacterNetworkTransform => true;
        protected virtual bool AddChildLocator => true;
        protected virtual bool AddDeathRewards => true;
        protected virtual bool AddDestroyOnUnseen => true;
        protected virtual bool AddEntityStateMachines => true;
        protected virtual bool AddEquipmentSlot => true;
        protected virtual bool AddFootstepHandler => true;
        protected virtual bool AddSkills => true;
        protected virtual bool AddHealthComponent => true;
        protected virtual bool AddHitBoxes => false;
        protected virtual bool AddHurtBoxes => true;
        protected virtual bool AddInputBankTest => true;
        protected virtual bool AddInteractionDriver => true;
        protected virtual bool AddInteractor => true;
        protected virtual bool AddModelLocator => true;
        protected virtual bool AddModelPanelParameters => true;
        protected virtual bool AddModelSkinController => true;
        protected virtual bool AddNetworkIdentity => true;
        protected virtual bool AddSetStateOnHurt => true;
        protected virtual bool AddTeamComponent => true;
        protected virtual bool AddSfxLocator => true;
        protected virtual bool AddAimAssistScale => true;
        protected virtual bool AddCrouchMecanim => false;
        protected virtual bool AddRandomBlinks => false;
        protected virtual bool AddDeployable => false;
        protected virtual bool AddExecuteSkillOnDamage => false;
        protected virtual bool AddRemoveJitterBones => false;
        protected virtual bool AddRigidbodyMotor => false;

        protected class SkillParams
        {
            public SkillParams(string name, EntityStates.SerializableEntityStateType activationState)
            {
                this.name = name;
                this.activationState = activationState;
            }

            public string name;
            public string nameToken = "ENEMIES_RETURNS_SKILL_NO_NAME";
            public string descriptionToken = "ENEMIES_RETURNS_SKILL_NO_DESCRIPTION";
            public Sprite icon = null;
            public string activationStateMachine = "Body";
            public EntityStates.SerializableEntityStateType activationState;
            public EntityStates.InterruptPriority interruptPriority = EntityStates.InterruptPriority.Skill;
            public float baseRechargeInterval = 1f;
            public int baseMaxStock = 1;
            public int rechargeStock = 1;
            public int requiredStock = 1;
            public int stockToConsume = 1;
            public bool resetCooldownTimerOnUse = false;
            public bool fullRestockOnAssign = true;
            public bool dontAllowPAstMaxStocks = false;
            public bool beginSkillCooldownOnSkillEnd = false;
            public bool cancelSprintingOnActivation = true;
            public bool forceSprintDuringState = false;
            public bool canceledFromSprinting = false;
            public bool isCombatSkill = true;
            public bool mustKeyPress = false;
        }

        protected class SpawnCardParams
        {
            public SpawnCardParams(string name, GameObject master, int directorCost)
            {
                this.name = name;
                this.masterPrefab = master;
                this.directorCreditCost = directorCost;
            }

            public string name;
            public GameObject masterPrefab;
            public bool sendOverNetwork = true;
            public HullClassification hullSize = HullClassification.Golem;
            public MapNodeGroup.GraphType graphType = MapNodeGroup.GraphType.Ground;
            public NodeFlags requiredFlags = NodeFlags.None;
            public NodeFlags forbiddenFlags = NodeFlags.NoCharacterSpawn;
            public int directorCreditCost;
            public bool occupyPosition = false;
            public SpawnCard.EliteRules eliteRules = SpawnCard.EliteRules.Default;
            public bool noElites = false;
            public bool forbiddenAsBoss = false;
            public SkinDef skinDef;
            public GameObject bodyPrefab;
        }

        protected SkillDef CreateSkill(SkillParams skillParams)
        {
            var skill = ScriptableObject.CreateInstance<SkillDef>();
            FillSkillDefValues(skillParams, skill);

            return skill;
        }

        protected ShouldBeGroundedSkillDef CreateNonGroundedSkill(SkillParams skillParams)
        {
            var skill = ScriptableObject.CreateInstance<ShouldBeGroundedSkillDef>();
            FillSkillDefValues(skillParams, skill);

            return skill;
        }

        protected LynxTotemSkillDef CreateTotemSkill(SkillParams skillParams)
        {
            var skill = ScriptableObject.CreateInstance<LynxTotemSkillDef>();
            FillSkillDefValues(skillParams, skill);

            return skill;
        }

        private static void FillSkillDefValues(SkillParams skillParams, SkillDef skill)
        {
            (skill as ScriptableObject).name = skillParams.name;
            skill.skillName = skillParams.name;

            skill.skillNameToken = skillParams.nameToken;
            skill.skillDescriptionToken = skillParams.descriptionToken;
            skill.icon = skillParams.icon;

            skill.activationStateMachineName = skillParams.activationStateMachine;
            skill.activationState = skillParams.activationState;
            skill.interruptPriority = skillParams.interruptPriority;

            skill.baseRechargeInterval = skillParams.baseRechargeInterval;
            skill.baseMaxStock = skillParams.baseMaxStock;
            skill.rechargeStock = skillParams.rechargeStock;
            skill.requiredStock = skillParams.requiredStock;
            skill.stockToConsume = skillParams.stockToConsume;

            skill.resetCooldownTimerOnUse = skillParams.resetCooldownTimerOnUse;
            skill.fullRestockOnAssign = skillParams.fullRestockOnAssign;
            skill.dontAllowPastMaxStocks = skillParams.dontAllowPAstMaxStocks;
            skill.beginSkillCooldownOnSkillEnd = skillParams.beginSkillCooldownOnSkillEnd;

            skill.canceledFromSprinting = skillParams.canceledFromSprinting;
            skill.forceSprintDuringState = skillParams.forceSprintDuringState;
            skill.canceledFromSprinting = skillParams.canceledFromSprinting;

            skill.isCombatSkill = skillParams.isCombatSkill;
            skill.mustKeyPress = skillParams.mustKeyPress;
        }

        protected SummonTribeSkillDef CreateSummonTribeSkill(SkillParams skillParams)
        {
            var skill = ScriptableObject.CreateInstance<SummonTribeSkillDef>();
            (skill as ScriptableObject).name = skillParams.name;
            skill.skillName = skillParams.name;

            skill.skillNameToken = skillParams.nameToken;
            skill.skillDescriptionToken = skillParams.descriptionToken;
            skill.icon = skillParams.icon;

            skill.activationStateMachineName = skillParams.activationStateMachine;
            skill.activationState = skillParams.activationState;
            skill.interruptPriority = skillParams.interruptPriority;

            skill.baseRechargeInterval = skillParams.baseRechargeInterval;
            skill.baseMaxStock = skillParams.baseMaxStock;
            skill.rechargeStock = skillParams.rechargeStock;
            skill.requiredStock = skillParams.requiredStock;
            skill.stockToConsume = skillParams.stockToConsume;

            skill.resetCooldownTimerOnUse = skillParams.resetCooldownTimerOnUse;
            skill.fullRestockOnAssign = skillParams.fullRestockOnAssign;
            skill.dontAllowPastMaxStocks = skillParams.dontAllowPAstMaxStocks;
            skill.beginSkillCooldownOnSkillEnd = skillParams.beginSkillCooldownOnSkillEnd;

            skill.canceledFromSprinting = skillParams.canceledFromSprinting;
            skill.forceSprintDuringState = skillParams.forceSprintDuringState;
            skill.canceledFromSprinting = skillParams.canceledFromSprinting;

            skill.isCombatSkill = skillParams.isCombatSkill;
            skill.mustKeyPress = skillParams.mustKeyPress;

            return skill;
        }

        protected CharacterSpawnCard CreateCard(SpawnCardParams spawnCardParams)
        {
            var card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            (card as ScriptableObject).name = spawnCardParams.name;
            card.prefab = spawnCardParams.masterPrefab;
            card.sendOverNetwork = spawnCardParams.sendOverNetwork;
            card.hullSize = spawnCardParams.hullSize;
            card.nodeGraphType = spawnCardParams.graphType;
            card.requiredFlags = spawnCardParams.requiredFlags;
            card.forbiddenFlags = spawnCardParams.forbiddenFlags;
            card.directorCreditCost = spawnCardParams.directorCreditCost;
            card.occupyPosition = spawnCardParams.occupyPosition;
            card.eliteRules = spawnCardParams.eliteRules;
            card.noElites = spawnCardParams.noElites;
            card.forbiddenAsBoss = spawnCardParams.forbiddenAsBoss;
            if (spawnCardParams.skinDef && spawnCardParams.bodyPrefab && spawnCardParams.bodyPrefab.TryGetComponent<CharacterBody>(out var body))
            {
                card.loadout = new SerializableLoadout
                {
                    bodyLoadouts = new SerializableLoadout.BodyLoadout[]
                    {
                        new SerializableLoadout.BodyLoadout()
                        {
                            body = body,
                            skinChoice = spawnCardParams.skinDef,
                            skillChoices = Array.Empty<SerializableLoadout.BodyLoadout.SkillChoice>() // yes, we need it
                        }
                    }
                };
            };

            return card;
        }

        protected virtual GameObject GetCrosshair()
        {
            return Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
        }

        protected virtual EntityStates.SerializableEntityStateType GetInitialBodyState()
        {
            return new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized));
        }

        protected virtual IAimAnimator.AimAnimatorParams AimAnimatorParams()
        {
            return new IAimAnimator.AimAnimatorParams
            {
                pitchRangeMin = -65f, // TODO: maybe give another pass for every monster
                pitchRangeMax = 65f,

                yawRangeMin = -60f,
                yawRangeMax = 60f,

                pitchGiveUpRange = 40f,
                yawGiveUpRange = 20f,
                giveUpDuration = 3f,

                raisedApproachSpeed = 720f,
                loweredApproachSpeed = 360f,
                smoothTime = 0.1f,

                fullYaw = false,
                aimType = AimAnimator.AimType.Direct,

                enableAimWeight = false,
                UseTransformedAimVector = false
            };
        }

        protected virtual ITeamComponent.TeamComponentParams TeamComponentParams()
        {
            return new ITeamComponent.TeamComponentParams
            {
                hideAllyCardDisplay = false,
                teamIndex = TeamIndex.None
            };
        }

        protected virtual ICharacterDeathBehavior.CharacterDeathBehaviorParams CharacterDeathBehaviorParams()
        {
            return new ICharacterDeathBehavior.CharacterDeathBehaviorParams("Body", new EntityStates.SerializableEntityStateType(typeof(EntityStates.GenericCharacterDeath)));
        }

        protected virtual CharacterCameraParams CharacterCameraParams()
        {
            return Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardTall.asset").WaitForCompletion();
        }

        protected virtual ICharacterMotor.CharacterMotorParams CharacterMotorParams()
        {
            return new ICharacterMotor.CharacterMotorParams()
            {
                airControl = 0.25f,
                disableAirControl = false,
                generateParametersOnAwake = true,
                muteWalkMotion = false
            };
        }

        protected virtual ICharacterNetworkTransform.CharacterNetworkTransformParams CharacterNetworkTransformParams()
        {
            return new ICharacterNetworkTransform.CharacterNetworkTransformParams();
        }

        protected virtual IHealthComponent.HealthComponentParams HealthComponentParams()
        {
            return new IHealthComponent.HealthComponentParams();
        }

        protected virtual IHitboxes.HitBoxesParams[] HitBoxesParams()
        {
            return Array.Empty<IHitboxes.HitBoxesParams>();
        }

        protected virtual IKinematicCharacterMotor.KinemacitCharacterMotorParams KinemacitCharacterMotorParams()
        {
            return new IKinematicCharacterMotor.KinemacitCharacterMotorParams();
        }

        protected virtual IModelLocator.ModelLocatorParams ModelLocatorParams()
        {
            return new IModelLocator.ModelLocatorParams();
        }

        protected virtual ISfxLocator.SfxLocatorParams SfxLocatorParams()
        {
            return new ISfxLocator.SfxLocatorParams();
        }

        protected virtual float MaxInteractionDistance => 3f;

        protected virtual INetworkIdentity.NetworkIdentityParams NetworkIdentityParams()
        {
            return new INetworkIdentity.NetworkIdentityParams();
        }

        protected virtual ISetStateOnHurt.SetStateOnHurtParams SetStateOnHurtParams()
        {
            var hurtState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtState));
            return new ISetStateOnHurt.SetStateOnHurtParams("Body", hurtState)
            {
                hitThreshold = 0.3f,
            };
        }

        protected virtual float CharacterDirectionTurnSpeed => 720f;

        protected virtual ICrouchMecanim.CrouchMecanimParams CrouchMecanimParams()
        {
            return new ICrouchMecanim.CrouchMecanimParams()
            {
                duckHeight = 25f,
                initialverticalOffset = 0f,
                smoothdamp = 0.1f
            };
        }

        protected virtual IRandomBlinkController.RandomBlinkParams RandomBlinkParams()
        {
            return new IRandomBlinkController.RandomBlinkParams(new string[] { "BlinkEye" })
            {
                blinkChancePerUpdate = 10f
            };
        }

        protected virtual IRigidbodyMotor.RigidbodyMotorParams RigidbodyMotorParams()
        {
            return new IRigidbodyMotor.RigidbodyMotorParams();
        }

        protected virtual IRigidBodyDirection.RigidbodyDirectionParams RigidbodyDirectionParams()
        {
            return new IRigidBodyDirection.RigidbodyDirectionParams();
        }

        protected virtual IExecuteSkillOnDamage.ExecuteSkillOnDamageParams ExecuteSkillOnDamage()
        {
            return new IExecuteSkillOnDamage.ExecuteSkillOnDamageParams()
            {
                mainStateMachineName = "Body",
                skillToExecute = SkillSlot.Primary
            };
        }

        string IBody.ModelName() => ModelName();

        SurfaceDef ISetupHurtboxes.GetSurfaceDef() => SurfaceDef();

        ITeamComponent.TeamComponentParams ITeamComponent.GetTeamComponentParams() => TeamComponentParams();

        ICharacterDeathBehavior.CharacterDeathBehaviorParams ICharacterDeathBehavior.GetCharacterDeathBehaviorParams() => CharacterDeathBehaviorParams();

        SkinDef[] ICreateSkinDefs.CreateSkinDefs(GameObject modelPrefab) => CreateSkinDefs(modelPrefab);

        IAimAnimator.AimAnimatorParams IAimAnimator.GetAimAnimatorParams() => AimAnimatorParams();

        ICharacterBody.CharacterBodyParams ICharacterBody.GetCharacterBodyParams(Transform aimOrigin, Sprite icon) => CharacterBodyParams(aimOrigin, icon);

        CharacterCameraParams ICameraTargetParams.GetCharacterCameraParams() => CharacterCameraParams();

        ICharacterModel.CharacterModelParams ICharacterModel.GetCharacterModelParams(GameObject modelPrefab) => CharacterModelParams(modelPrefab);

        ICharacterMotor.CharacterMotorParams ICharacterMotor.GetCharacterMotorParams() => CharacterMotorParams();

        ICharacterNetworkTransform.CharacterNetworkTransformParams ICharacterNetworkTransform.GetCharacterNetworkTransformParams() => CharacterNetworkTransformParams();

        IEntityStateMachine.EntityStateMachineParams[] IEntityStateMachine.GetEntityStateMachineParams() => EntityStateMachineParams();

        IFootStepHandler.FootstepHandlerParams IFootStepHandler.GetFootstepHandlerParams() => FootstepHandlerParams();

        IGenericSkill.GenericSkillParams[] IGenericSkill.GetGenericSkillParams() => GenericSkillParams();

        IHealthComponent.HealthComponentParams IHealthComponent.GetHealthComponentParams() => HealthComponentParams();

        IHitboxes.HitBoxesParams[] IHitboxes.GetHitBoxesParams() => HitBoxesParams();

        float IInteractor.GetInteractionDistance() => MaxInteractionDistance;

        IKinematicCharacterMotor.KinemacitCharacterMotorParams IKinematicCharacterMotor.GetKinematicCharacterMotorParams() => KinemacitCharacterMotorParams();

        IModelLocator.ModelLocatorParams IModelLocator.GetModelLocatorParams() => ModelLocatorParams();

        IModelPanelParameters.ModelPanelParams IModelPanelParameters.GetModelPanelParams() => ModelPanelParams();

        INetworkIdentity.NetworkIdentityParams INetworkIdentity.GetNetworkIdentityParams() => NetworkIdentityParams();

        ISetStateOnHurt.SetStateOnHurtParams ISetStateOnHurt.GetSetStateOnHurtParams() => SetStateOnHurtParams();

        float ICharacterDirection.GetCharacterDirectionTurnSpeed() => CharacterDirectionTurnSpeed;

        ISfxLocator.SfxLocatorParams ISfxLocator.GetSfxLocatorParams() => SfxLocatorParams();

        ItemDisplayRuleSet ICharacterModel.GetItemDisplayRuleSet() => ItemDisplayRuleSet();

        IAimAssist.AimAssistTargetParams IAimAssist.GetAimAssistTargetParams() => AimAssistTargetParams();

        ICrouchMecanim.CrouchMecanimParams ICrouchMecanim.GetCrouchMecanimParams() => CrouchMecanimParams();

        IRandomBlinkController.RandomBlinkParams IRandomBlinkController.GetRandomBlinkParams() => RandomBlinkParams();

        IExecuteSkillOnDamage.ExecuteSkillOnDamageParams IExecuteSkillOnDamage.GetExecuteSkillOnDamageParams() => ExecuteSkillOnDamage();

        IRigidBodyDirection.RigidbodyDirectionParams IRigidBodyDirection.GetRigidBodyDirectionParams() => RigidbodyDirectionParams();

        IRigidbodyMotor.RigidbodyMotorParams IRigidbodyMotor.GetRigidBodyMotorParams() => RigidbodyMotorParams();

        bool IAimAnimator.NeedToAddAimAnimator() => AddAimAnimator;
        bool IAnimationEvents.NeedToAddAnimationEvents() => AddAnimationEvents;
        bool ICameraTargetParams.NeedToAddCameraTargetParams() => AddCameraTargetParams;
        bool ICharacterBody.NeedToAddCharacterBody() => AddCharacterBody;
        bool ICharacterDeathBehavior.NeedToAddCharacterDeathBehavior() => AddCharacterDeathBehavior;
        bool ICharacterDirection.NeedToAddCharacterDirection() => AddCharacterDirection;
        bool ICharacterModel.NeedToAddCharacterModel() => AddCharacterModel;
        bool ICharacterMotor.NeedToAddCharacterMotor() => AddCharacterMotor;
        bool ICharacterNetworkTransform.NeedToAddCharacterNetworkTransform() => AddCharacterNetworkTransform;
        bool IChildLocator.NeedToAddChildLocator() => AddChildLocator;
        bool IDeathRewards.NeedToAddDeathRewards() => AddDeathRewards;
        bool IDestroyOnUnseen.NeedToAddDestroyOnUnseen() => AddDestroyOnUnseen;
        bool IEntityStateMachine.NeedToAddEntityStateMachines() => AddEntityStateMachines;
        bool IEquipmentSlot.NeedToAddEquipmentSlot() => AddEquipmentSlot;
        bool IFootStepHandler.NeedToAddFootstepHandler() => AddFootstepHandler;
        bool IGenericSkill.NeedToAddGenericSkills() => AddSkills;
        bool IHealthComponent.NeedToAddHealthComponent() => AddHealthComponent;
        bool IHitboxes.NeedToAddHitBoxes() => AddHitBoxes;
        bool IHitBoxGroup.NeedToAddHitBoxGroups() => AddHitBoxes;
        bool IHurtBoxGroup.NeedToAddHurtBoxGhoup() => AddHurtBoxes;
        bool IInputBankTest.NeedToAddInputBankTest() => AddInputBankTest;
        bool IInteractionDriver.NeedToAddInteractionDriver() => AddInteractionDriver;
        bool IInteractor.NeedToAddInteractor() => AddInteractor;
        bool IKinematicCharacterMotor.NeedToAddKinematicCharacterMotor() => AddCharacterMotor;
        bool IModelLocator.NeedToAddModelLocator() => AddModelLocator;
        bool IModelPanelParameters.NeedToAddModelPanelParameters() => AddModelPanelParameters;
        bool IModelSkinController.NeedToAddModelSkinController() => AddModelSkinController;
        bool INetworkIdentity.NeedToAddNetworkIdentity() => AddNetworkIdentity;
        bool INetworkStateMachine.NeedToAddNetworkStateMachine() => AddEntityStateMachines;
        bool ISetStateOnHurt.NeedToAddSetStateOnHurt() => AddSetStateOnHurt;
        bool ISkillLocator.NeedToAddSkillLocator() => AddSkills;
        bool ITeamComponent.NeedToAddTeamComponent() => AddTeamComponent;
        bool ISetupHurtboxes.NeedToSetupHurtboxes() => AddHurtBoxes;
        bool ISfxLocator.NeedToAddSfxLocator() => AddSfxLocator;
        bool IAimAssist.NeedToAddAimAssistTarget() => AddAimAssistScale;
        bool ICrouchMecanim.NeedToAddCrouchMecanim() => AddCrouchMecanim;
        bool IRandomBlinkController.NeedToAddRandomBlinkController() => AddRandomBlinks;
        bool IDeployable.NeedToAddDeployable() => AddDeployable;
        bool IExecuteSkillOnDamage.NeedToAddExecuteSkillOnDamage() => AddExecuteSkillOnDamage;
        bool IRemoveJitterBones.NeedToAddRemoveJitterBones() => AddRemoveJitterBones;
        bool IRigidbodyMotor.NeedToAddRigidbodyMotor() => AddRigidbodyMotor;
        bool IRigidBodyDirection.NeedToAddRigidBodyDirection() => AddRigidbodyMotor;


    }
}
