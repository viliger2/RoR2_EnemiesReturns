using EnemiesReturns.PrefabAPICompat;
using HG;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.ItemDisplayRuleSet;
using static EnemiesReturns.Utils;
using RoR2.Networking;
using KinematicCharacterController;
using System.Linq;
using EnemiesReturns.EditorHelpers;
using ThreeEyedGames;
using EnemiesReturns.Configuration;
using EnemiesReturns.Helpers;
using EnemiesReturns.ModdedEntityStates.MechanicalSpider.Dash;
using RoR2.Hologram;
using RoR2.Audio;

namespace EnemiesReturns.Enemies.MechanicalSpider
{
    public class MechanicalSpiderFactory
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

        public struct SkinDefs
        {
            public static SkinDef Default;
            //public static SkinDef Lakes;
            //public static SkinDef Sulfur;
            //public static SkinDef Depths;
        }

        public struct SpawnCards
        {
            public static CharacterSpawnCard cscMechanicalSpiderDefault;
            public static InteractableSpawnCard iscMechanicalSpiderBroken;
            //public static CharacterSpawnCard cscSpitterLakes;
            //public static CharacterSpawnCard cscSpitterSulfur;
            //public static CharacterSpawnCard cscSpitterDepths;
        }

        public static GameObject MechanicalSpiderBody;

        public static GameObject MechanicalSpiderMaster;

        public static GameObject MechanicalSpiderDroneBody;

        public static GameObject MechanicalSpiderDroneMaster;

        public static LoopSoundDef ProjectileFlightSoundLoop;

        public GameObject CreateBody(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            AddMainBodyComponents(bodyPrefab, sprite, log);

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public GameObject CreateDroneBody(GameObject bodyPrefab, Sprite sprite)
        {
            AddMainBodyComponents(bodyPrefab, sprite, null);

            var esms = bodyPrefab.GetComponents<EntityStateMachine>();
            foreach(var esm in esms)
            {
                if(esm.customName == "Body")
                {
                    esm.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.SpawnStateDrone));
                    break;
                }
            }

            var body = bodyPrefab.GetComponent<CharacterBody>();
            body.baseRegen = EnemiesReturns.Configuration.MechanicalSpider.DroneBaseRegen.Value;
            body.levelRegen = EnemiesReturns.Configuration.MechanicalSpider.DroneLevelRegen.Value;

            var sfxLocator = bodyPrefab.GetComponent<SfxLocator>();
            sfxLocator.aliveLoopStart = "";
            sfxLocator.aliveLoopStop = "";

            bodyPrefab.RegisterNetworkPrefab();

            return bodyPrefab;
        }

        public GameObject CreateMaster(GameObject masterPrefab, GameObject bodyPrefab)
        {
            AddMainMasterComponents(masterPrefab, bodyPrefab);
           
            #region AISkillDriver_StrafeAndShoot
            var asdStrafeAndShoot = masterPrefab.AddComponent<AISkillDriver>();
            asdStrafeAndShoot.customName = "StrafeAndShoot";
            asdStrafeAndShoot.skillSlot = SkillSlot.Primary;

            asdStrafeAndShoot.requiredSkill = null;
            asdStrafeAndShoot.requireSkillReady = false;
            asdStrafeAndShoot.requireEquipmentReady = false;
            asdStrafeAndShoot.minUserHealthFraction = float.NegativeInfinity;
            asdStrafeAndShoot.maxUserHealthFraction = float.PositiveInfinity;
            asdStrafeAndShoot.minTargetHealthFraction = float.NegativeInfinity;
            asdStrafeAndShoot.maxTargetHealthFraction = float.PositiveInfinity;
            asdStrafeAndShoot.minDistance = 0f;
            asdStrafeAndShoot.maxDistance = 60f;
            asdStrafeAndShoot.selectionRequiresTargetLoS = true;
            asdStrafeAndShoot.selectionRequiresOnGround = false;
            asdStrafeAndShoot.selectionRequiresAimTarget = false;
            asdStrafeAndShoot.maxTimesSelected = -1;

            asdStrafeAndShoot.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdStrafeAndShoot.activationRequiresTargetLoS = true;
            asdStrafeAndShoot.activationRequiresAimTargetLoS = false;
            asdStrafeAndShoot.activationRequiresAimConfirmation = true;
            asdStrafeAndShoot.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            asdStrafeAndShoot.moveInputScale = 1;
            asdStrafeAndShoot.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdStrafeAndShoot.ignoreNodeGraph = false;
            asdStrafeAndShoot.shouldSprint = false;
            asdStrafeAndShoot.shouldFireEquipment = false;
            asdStrafeAndShoot.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdStrafeAndShoot.driverUpdateTimerOverride = -1;
            asdStrafeAndShoot.resetCurrentEnemyOnNextDriverSelection = false;
            asdStrafeAndShoot.noRepeat = false;
            asdStrafeAndShoot.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_ChaseOffNodeGraph
            var asdChaseOffNodeGraph = masterPrefab.AddComponent<AISkillDriver>();
            asdChaseOffNodeGraph.customName = "ChaseOffNodegraph";
            asdChaseOffNodeGraph.skillSlot = SkillSlot.None;

            asdChaseOffNodeGraph.requiredSkill = null;
            asdChaseOffNodeGraph.requireSkillReady = false;
            asdChaseOffNodeGraph.requireEquipmentReady = false;
            asdChaseOffNodeGraph.minUserHealthFraction = float.NegativeInfinity;
            asdChaseOffNodeGraph.maxUserHealthFraction = float.PositiveInfinity;
            asdChaseOffNodeGraph.minTargetHealthFraction = float.NegativeInfinity;
            asdChaseOffNodeGraph.maxTargetHealthFraction = float.PositiveInfinity;
            asdChaseOffNodeGraph.minDistance = 0f;
            asdChaseOffNodeGraph.maxDistance = 7f;
            asdChaseOffNodeGraph.selectionRequiresTargetLoS = true;
            asdChaseOffNodeGraph.selectionRequiresOnGround = false;
            asdChaseOffNodeGraph.selectionRequiresAimTarget = false;
            asdChaseOffNodeGraph.maxTimesSelected = -1;

            asdChaseOffNodeGraph.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdChaseOffNodeGraph.activationRequiresTargetLoS = false;
            asdChaseOffNodeGraph.activationRequiresAimTargetLoS = false;
            asdChaseOffNodeGraph.activationRequiresAimConfirmation = false;
            asdChaseOffNodeGraph.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdChaseOffNodeGraph.moveInputScale = 1;
            asdChaseOffNodeGraph.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdChaseOffNodeGraph.ignoreNodeGraph = true;
            asdChaseOffNodeGraph.shouldSprint = false;
            asdChaseOffNodeGraph.shouldFireEquipment = false;
            asdChaseOffNodeGraph.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdChaseOffNodeGraph.driverUpdateTimerOverride = -1;
            asdChaseOffNodeGraph.resetCurrentEnemyOnNextDriverSelection = false;
            asdChaseOffNodeGraph.noRepeat = false;
            asdChaseOffNodeGraph.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_DashToTraverse
            var asdDashToTraverse = masterPrefab.AddComponent<AISkillDriver>();
            asdDashToTraverse.customName = "DashToTraverse";
            asdDashToTraverse.skillSlot = SkillSlot.Utility;

            asdDashToTraverse.requiredSkill = null;
            asdDashToTraverse.requireSkillReady = true;
            asdDashToTraverse.requireEquipmentReady = false;
            asdDashToTraverse.minUserHealthFraction = float.NegativeInfinity;
            asdDashToTraverse.maxUserHealthFraction = float.PositiveInfinity;
            asdDashToTraverse.minTargetHealthFraction = float.NegativeInfinity;
            asdDashToTraverse.maxTargetHealthFraction = float.PositiveInfinity;
            asdDashToTraverse.minDistance = 50f;
            asdDashToTraverse.maxDistance = float.PositiveInfinity;
            asdDashToTraverse.selectionRequiresTargetLoS = false;
            asdDashToTraverse.selectionRequiresOnGround = false;
            asdDashToTraverse.selectionRequiresAimTarget = false;
            asdDashToTraverse.maxTimesSelected = -1;

            asdDashToTraverse.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdDashToTraverse.activationRequiresTargetLoS = true;
            asdDashToTraverse.activationRequiresAimTargetLoS = false;
            asdDashToTraverse.activationRequiresAimConfirmation = false;
            asdDashToTraverse.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdDashToTraverse.moveInputScale = 1;
            asdDashToTraverse.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdDashToTraverse.ignoreNodeGraph = false;
            asdDashToTraverse.shouldSprint = false;
            asdDashToTraverse.shouldFireEquipment = false;
            asdDashToTraverse.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdDashToTraverse.driverUpdateTimerOverride = -1;
            asdDashToTraverse.resetCurrentEnemyOnNextDriverSelection = false;
            asdDashToTraverse.noRepeat = false;
            asdDashToTraverse.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_PathFromAfar
            var asdPathFromAfar = masterPrefab.AddComponent<AISkillDriver>();
            asdPathFromAfar.customName = "PathFromAfar";
            asdPathFromAfar.skillSlot = SkillSlot.None;

            asdPathFromAfar.requiredSkill = null;
            asdPathFromAfar.requireSkillReady = false;
            asdPathFromAfar.requireEquipmentReady = false;
            asdPathFromAfar.minUserHealthFraction = float.NegativeInfinity;
            asdPathFromAfar.maxUserHealthFraction = float.PositiveInfinity;
            asdPathFromAfar.minTargetHealthFraction = float.NegativeInfinity;
            asdPathFromAfar.maxTargetHealthFraction = float.PositiveInfinity;
            asdPathFromAfar.minDistance = 0f;
            asdPathFromAfar.maxDistance = float.PositiveInfinity;
            asdPathFromAfar.selectionRequiresTargetLoS = false;
            asdPathFromAfar.selectionRequiresOnGround = false;
            asdPathFromAfar.selectionRequiresAimTarget = false;
            asdPathFromAfar.maxTimesSelected = -1;

            asdPathFromAfar.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdPathFromAfar.activationRequiresTargetLoS = false;
            asdPathFromAfar.activationRequiresAimTargetLoS = false;
            asdPathFromAfar.activationRequiresAimConfirmation = false;
            asdPathFromAfar.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdPathFromAfar.moveInputScale = 1;
            asdPathFromAfar.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdPathFromAfar.ignoreNodeGraph = false;
            asdPathFromAfar.shouldSprint = false;
            asdPathFromAfar.shouldFireEquipment = false;
            asdPathFromAfar.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdPathFromAfar.driverUpdateTimerOverride = -1;
            asdPathFromAfar.resetCurrentEnemyOnNextDriverSelection = false;
            asdPathFromAfar.noRepeat = false;
            asdPathFromAfar.nextHighPriorityOverride = null;
            #endregion

            masterPrefab.RegisterNetworkPrefab();

            return masterPrefab;
        }

        public GameObject CreateDroneMaster(GameObject masterPrefab, GameObject bodyPrefab)
        {
            AddMainMasterComponents(masterPrefab, bodyPrefab);

            #region AIOwnership
            masterPrefab.AddComponent<AIOwnership>();
            #endregion

            #region AISkillDriver_HardLeashToLeader
            var asdHardLeashToLeader = masterPrefab.AddComponent<AISkillDriver>();
            asdHardLeashToLeader.customName = "HardLeashToLeader";
            asdHardLeashToLeader.skillSlot = SkillSlot.None;

            asdHardLeashToLeader.requiredSkill = null;
            asdHardLeashToLeader.requireSkillReady = false;
            asdHardLeashToLeader.requireEquipmentReady = false;
            asdHardLeashToLeader.minUserHealthFraction = float.NegativeInfinity;
            asdHardLeashToLeader.maxUserHealthFraction = float.PositiveInfinity;
            asdHardLeashToLeader.minTargetHealthFraction = float.NegativeInfinity;
            asdHardLeashToLeader.maxTargetHealthFraction = float.PositiveInfinity;
            asdHardLeashToLeader.minDistance = 60f;
            asdHardLeashToLeader.maxDistance = float.PositiveInfinity;
            asdHardLeashToLeader.selectionRequiresTargetLoS = false;
            asdHardLeashToLeader.selectionRequiresOnGround = false;
            asdHardLeashToLeader.selectionRequiresAimTarget = false;
            asdHardLeashToLeader.maxTimesSelected = -1;

            asdHardLeashToLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdHardLeashToLeader.activationRequiresTargetLoS = false;
            asdHardLeashToLeader.activationRequiresAimTargetLoS = false;
            asdHardLeashToLeader.activationRequiresAimConfirmation = false;
            asdHardLeashToLeader.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdHardLeashToLeader.moveInputScale = 1;
            asdHardLeashToLeader.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdHardLeashToLeader.ignoreNodeGraph = false;
            asdHardLeashToLeader.shouldSprint = false;
            asdHardLeashToLeader.shouldFireEquipment = false;
            asdHardLeashToLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdHardLeashToLeader.driverUpdateTimerOverride = 3;
            asdHardLeashToLeader.resetCurrentEnemyOnNextDriverSelection = true;
            asdHardLeashToLeader.noRepeat = false;
            asdHardLeashToLeader.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_SoftLeashToLeader
            var asdSoftLeashToLeader = masterPrefab.AddComponent<AISkillDriver>();
            asdSoftLeashToLeader.customName = "SoftLeashToLeader";
            asdSoftLeashToLeader.skillSlot = SkillSlot.None;

            asdSoftLeashToLeader.requiredSkill = null;
            asdSoftLeashToLeader.requireSkillReady = false;
            asdSoftLeashToLeader.requireEquipmentReady = false;
            asdSoftLeashToLeader.minUserHealthFraction = float.NegativeInfinity;
            asdSoftLeashToLeader.maxUserHealthFraction = float.PositiveInfinity;
            asdSoftLeashToLeader.minTargetHealthFraction = float.NegativeInfinity;
            asdSoftLeashToLeader.maxTargetHealthFraction = float.PositiveInfinity;
            asdSoftLeashToLeader.minDistance = 20f;
            asdSoftLeashToLeader.maxDistance = float.PositiveInfinity;
            asdSoftLeashToLeader.selectionRequiresTargetLoS = false;
            asdSoftLeashToLeader.selectionRequiresOnGround = false;
            asdSoftLeashToLeader.selectionRequiresAimTarget = false;
            asdSoftLeashToLeader.maxTimesSelected = -1;

            asdSoftLeashToLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdSoftLeashToLeader.activationRequiresTargetLoS = false;
            asdSoftLeashToLeader.activationRequiresAimTargetLoS = false;
            asdSoftLeashToLeader.activationRequiresAimConfirmation = false;
            asdSoftLeashToLeader.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            asdSoftLeashToLeader.moveInputScale = 1;
            asdSoftLeashToLeader.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            asdSoftLeashToLeader.ignoreNodeGraph = false;
            asdSoftLeashToLeader.shouldSprint = false;
            asdSoftLeashToLeader.shouldFireEquipment = false;
            asdSoftLeashToLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdSoftLeashToLeader.driverUpdateTimerOverride = 0.05f;
            asdSoftLeashToLeader.resetCurrentEnemyOnNextDriverSelection = true;
            asdSoftLeashToLeader.noRepeat = false;
            asdSoftLeashToLeader.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_StrafeAndShoot
            var asdStrafeAndShoot = masterPrefab.AddComponent<AISkillDriver>();
            asdStrafeAndShoot.customName = "StrafeAndShoot";
            asdStrafeAndShoot.skillSlot = SkillSlot.Primary;

            asdStrafeAndShoot.requiredSkill = null;
            asdStrafeAndShoot.requireSkillReady = true;
            asdStrafeAndShoot.requireEquipmentReady = false;
            asdStrafeAndShoot.minUserHealthFraction = float.NegativeInfinity;
            asdStrafeAndShoot.maxUserHealthFraction = float.PositiveInfinity;
            asdStrafeAndShoot.minTargetHealthFraction = float.NegativeInfinity;
            asdStrafeAndShoot.maxTargetHealthFraction = float.PositiveInfinity;
            asdStrafeAndShoot.minDistance = 0f;
            asdStrafeAndShoot.maxDistance = 60f;
            asdStrafeAndShoot.selectionRequiresTargetLoS = true;
            asdStrafeAndShoot.selectionRequiresOnGround = false;
            asdStrafeAndShoot.selectionRequiresAimTarget = false;
            asdStrafeAndShoot.maxTimesSelected = -1;

            asdStrafeAndShoot.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            asdStrafeAndShoot.activationRequiresTargetLoS = true;
            asdStrafeAndShoot.activationRequiresAimTargetLoS = false;
            asdStrafeAndShoot.activationRequiresAimConfirmation = true;
            asdStrafeAndShoot.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            asdStrafeAndShoot.moveInputScale = 1;
            asdStrafeAndShoot.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            asdStrafeAndShoot.ignoreNodeGraph = false;
            asdStrafeAndShoot.shouldSprint = false;
            asdStrafeAndShoot.shouldFireEquipment = false;
            asdStrafeAndShoot.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdStrafeAndShoot.driverUpdateTimerOverride = -1;
            asdStrafeAndShoot.resetCurrentEnemyOnNextDriverSelection = false;
            asdStrafeAndShoot.noRepeat = false;
            asdStrafeAndShoot.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_IdleNearLeader
            var asdIdleNearLeader = masterPrefab.AddComponent<AISkillDriver>();
            asdIdleNearLeader.customName = "IdleNearLeader";
            asdIdleNearLeader.skillSlot = SkillSlot.None;

            asdIdleNearLeader.requiredSkill = null;
            asdIdleNearLeader.requireSkillReady = false;
            asdIdleNearLeader.requireEquipmentReady = false;
            asdIdleNearLeader.minUserHealthFraction = float.NegativeInfinity;
            asdIdleNearLeader.maxUserHealthFraction = float.PositiveInfinity;
            asdIdleNearLeader.minTargetHealthFraction = float.NegativeInfinity;
            asdIdleNearLeader.maxTargetHealthFraction = float.PositiveInfinity;
            asdIdleNearLeader.minDistance = 0f;
            asdIdleNearLeader.maxDistance = 20f;
            asdIdleNearLeader.selectionRequiresTargetLoS = false;
            asdIdleNearLeader.selectionRequiresOnGround = false;
            asdIdleNearLeader.selectionRequiresAimTarget = false;
            asdIdleNearLeader.maxTimesSelected = -1;

            asdIdleNearLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdIdleNearLeader.activationRequiresTargetLoS = false;
            asdIdleNearLeader.activationRequiresAimTargetLoS = false;
            asdIdleNearLeader.activationRequiresAimConfirmation = false;
            asdIdleNearLeader.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            asdIdleNearLeader.moveInputScale = 1;
            asdIdleNearLeader.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdIdleNearLeader.ignoreNodeGraph = false;
            asdIdleNearLeader.shouldSprint = false;
            asdIdleNearLeader.shouldFireEquipment = false;
            asdIdleNearLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdIdleNearLeader.driverUpdateTimerOverride = -1f;
            asdIdleNearLeader.resetCurrentEnemyOnNextDriverSelection = false;
            asdIdleNearLeader.noRepeat = false;
            asdIdleNearLeader.nextHighPriorityOverride = null;
            #endregion

            #region AISkillDriver_ReturnToLeader
            var asdReturnToLeader = masterPrefab.AddComponent<AISkillDriver>();
            asdReturnToLeader.customName = "ReturnToLeaderWhenNoEnemies";
            asdReturnToLeader.skillSlot = SkillSlot.None;

            asdReturnToLeader.requiredSkill = null;
            asdReturnToLeader.requireSkillReady = false;
            asdReturnToLeader.requireEquipmentReady = false;
            asdReturnToLeader.minUserHealthFraction = float.NegativeInfinity;
            asdReturnToLeader.maxUserHealthFraction = float.PositiveInfinity;
            asdReturnToLeader.minTargetHealthFraction = float.NegativeInfinity;
            asdReturnToLeader.maxTargetHealthFraction = float.PositiveInfinity;
            asdReturnToLeader.minDistance = 10f;
            asdReturnToLeader.maxDistance = float.PositiveInfinity;
            asdReturnToLeader.selectionRequiresTargetLoS = false;
            asdReturnToLeader.selectionRequiresOnGround = false;
            asdReturnToLeader.selectionRequiresAimTarget = false;
            asdReturnToLeader.maxTimesSelected = -1;

            asdReturnToLeader.moveTargetType = AISkillDriver.TargetType.CurrentLeader;
            asdReturnToLeader.activationRequiresTargetLoS = false;
            asdReturnToLeader.activationRequiresAimTargetLoS = false;
            asdReturnToLeader.activationRequiresAimConfirmation = false;
            asdReturnToLeader.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            asdReturnToLeader.moveInputScale = 1;
            asdReturnToLeader.aimType = AISkillDriver.AimType.AtMoveTarget;
            asdReturnToLeader.ignoreNodeGraph = false;
            asdReturnToLeader.shouldSprint = false;
            asdReturnToLeader.shouldFireEquipment = false;
            asdReturnToLeader.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            asdReturnToLeader.driverUpdateTimerOverride = 0.05f;
            asdReturnToLeader.resetCurrentEnemyOnNextDriverSelection = true;
            asdReturnToLeader.noRepeat = false;
            asdReturnToLeader.nextHighPriorityOverride = null;
            #endregion

            masterPrefab.RegisterNetworkPrefab();

            return masterPrefab;
        }

        private void AddMainBodyComponents(GameObject bodyPrefab, Sprite sprite, UnlockableDef log)
        {
            var aimOrigin = bodyPrefab.transform.Find("AimOrigin");
            var cameraPivot = bodyPrefab.transform.Find("CameraPivot");
            var modelTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider");
            var modelBase = bodyPrefab.transform.Find("ModelBase");

            var focusPoint = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/LogBookTarget");
            var cameraPosition = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/LogBookTarget/LogBookCamera");

            var modelRenderer = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/MechanicalSpider").gameObject.GetComponent<SkinnedMeshRenderer>();

            var headTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Gun");
            var rootTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root");
            if (!headTransform)
            {
                Log.Warning("headTransform is null! This WILL result in stuck camera on " + nameof(MechanicalSpider) + " spawn.");
            }
            if (!rootTransform)
            {
                Log.Warning("rootTransform is null! This WILL result in stuck camera on " + nameof(MechanicalSpider) + " spawn.");
            }

            var animator = modelTransform.gameObject.GetComponent<Animator>();

            #region MechanicalSpiderBody

            #region NetworkIdentity
            bodyPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterDirection
            var characterDirection = bodyPrefab.AddComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBase;
            characterDirection.turnSpeed = 720f;
            characterDirection.modelAnimator = animator;
            #endregion

            #region CharacterMotor
            var characterMotor = bodyPrefab.AddComponent<CharacterMotor>();
            characterMotor.characterDirection = characterDirection;
            characterMotor.muteWalkMotion = false;
            characterMotor.mass = 100f;
            characterMotor.airControl = 0.25f;
            characterMotor.disableAirControlUntilCollision = false;
            characterMotor.generateParametersOnAwake = true;
            #endregion

            #region InputBankTest
            var inputBank = bodyPrefab.AddComponent<InputBankTest>();
            #endregion

            #region CharacterBody
            CharacterBody characterBody = null;
            if (!bodyPrefab.TryGetComponent(out characterBody))
            {
                characterBody = bodyPrefab.AddComponent<CharacterBody>();
            }
            characterBody.baseNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_BODY_NAME";
            characterBody.bodyFlags = CharacterBody.BodyFlags.None | CharacterBody.BodyFlags.Mechanical;
            characterBody.rootMotionInMainState = false;
            characterBody.mainRootSpeed = 33f;

            characterBody.baseMaxHealth = EnemiesReturns.Configuration.MechanicalSpider.BaseMaxHealth.Value;
            characterBody.baseRegen = 0f;
            characterBody.baseMaxShield = 0f;
            characterBody.baseMoveSpeed = EnemiesReturns.Configuration.MechanicalSpider.BaseMoveSpeed.Value;
            characterBody.baseAcceleration = 40f;
            characterBody.baseJumpPower = EnemiesReturns.Configuration.MechanicalSpider.BaseJumpPower.Value;
            characterBody.baseDamage = EnemiesReturns.Configuration.MechanicalSpider.BaseDamage.Value;
            characterBody.baseAttackSpeed = 1f;
            characterBody.baseCrit = 0f;
            characterBody.baseArmor = EnemiesReturns.Configuration.MechanicalSpider.BaseArmor.Value;
            characterBody.baseVisionDistance = float.PositiveInfinity;
            characterBody.baseJumpCount = 1;
            characterBody.sprintingSpeedMultiplier = 1.45f;

            characterBody.autoCalculateLevelStats = true;
            characterBody.levelMaxHealth = EnemiesReturns.Configuration.MechanicalSpider.LevelMaxHealth.Value;
            characterBody.levelDamage = EnemiesReturns.Configuration.MechanicalSpider.LevelDamage.Value;
            characterBody.levelArmor = EnemiesReturns.Configuration.MechanicalSpider.LevelArmor.Value;

            characterBody.wasLucky = false;
            characterBody.spreadBloomDecayTime = 0.45f;
            characterBody._defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();
            characterBody.aimOriginTransform = aimOrigin;
            characterBody.hullClassification = HullClassification.Human;
            if (sprite)
            {
                characterBody.portraitIcon = sprite.texture;
            }
            characterBody.bodyColor = new Color(0.5568628f, 0.627451f, 0.6745098f);
            characterBody.isChampion = false;
            characterBody.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Uninitialized));
            #endregion

            #region CameraTargetParams
            var cameraTargetParams = bodyPrefab.AddComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Addressables.LoadAssetAsync<CharacterCameraParams>("RoR2/Base/Common/ccpStandardTall.asset").WaitForCompletion();
            cameraTargetParams.cameraPivotTransform = cameraPivot;
            #endregion

            #region ModelLocator
            var modelLocator = bodyPrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBase;

            modelLocator.autoUpdateModelTransform = true;
            modelLocator.dontDetatchFromParent = false;

            modelLocator.noCorpse = false;
            modelLocator.dontDetatchFromParent = false;
            modelLocator.preserveModel = false;

            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.3f;
            modelLocator.normalMaxAngleDelta = 55f;
            #endregion

            #region EntityStateMachineBody
            var esmBody = bodyPrefab.AddComponent<EntityStateMachine>();
            esmBody.customName = "Body";
            esmBody.initialStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.SpawnState));
            esmBody.mainStateType = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.MainState));
            #endregion

            #region EntityStateMachineWeapon
            var esmWeapon = bodyPrefab.AddComponent<EntityStateMachine>();
            esmWeapon.customName = "Weapon";
            esmWeapon.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            esmWeapon.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            #endregion

            #region EntityStateSlide
            var esmSlide = bodyPrefab.AddComponent<EntityStateMachine>();
            esmSlide.customName = "Slide";
            esmSlide.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            esmSlide.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            #endregion

            #region GenericSkills

            #region Primary
            var gsPrimary = bodyPrefab.AddComponent<GenericSkill>();
            gsPrimary._skillFamily = SkillFamilies.Primary;
            gsPrimary.skillName = "DoubleShot";
            gsPrimary.hideInCharacterSelect = false;
            #endregion

            #region Utility
            var gsUtility = bodyPrefab.AddComponent<GenericSkill>();
            gsUtility._skillFamily = SkillFamilies.Utility;
            gsUtility.skillName = "Dash";
            gsUtility.hideInCharacterSelect = false;
            #endregion

            #endregion

            #region SkillLocator
            SkillLocator skillLocator = null;
            if (!bodyPrefab.TryGetComponent(out skillLocator))
            {
                skillLocator = bodyPrefab.AddComponent<SkillLocator>();
            }
            skillLocator.primary = gsPrimary;
            skillLocator.utility = gsUtility;
            //skillLocator.secondary = gsSecondary;
            //skillLocator.special = gsSpecial;
            #endregion

            #region TeamComponent
            TeamComponent teamComponent = null;
            if (!bodyPrefab.TryGetComponent(out teamComponent))
            {
                teamComponent = bodyPrefab.AddComponent<TeamComponent>();
            }
            teamComponent.teamIndex = TeamIndex.None;
            #endregion

            #region HealthComponent
            var healthComponent = bodyPrefab.AddComponent<HealthComponent>();
            healthComponent.dontShowHealthbar = false;
            healthComponent.globalDeathEventChanceCoefficient = 1f;
            #endregion

            #region Interactor
            bodyPrefab.AddComponent<Interactor>().maxInteractionDistance = 3f;
            #endregion

            #region InteractionDriver
            bodyPrefab.AddComponent<InteractionDriver>();
            #endregion

            #region CharacterDeathBehavior
            var characterDeathBehavior = bodyPrefab.AddComponent<CharacterDeathBehavior>();
            characterDeathBehavior.deathStateMachine = esmBody;
            characterDeathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.Death.DeathInitial));
            characterDeathBehavior.idleStateMachine = new EntityStateMachine[] { esmWeapon, esmSlide };
            #endregion

            #region CharacterNetworkTransform
            var characterNetworkTransform = bodyPrefab.AddComponent<CharacterNetworkTransform>();
            characterNetworkTransform.positionTransmitInterval = 0.1f;
            characterNetworkTransform.interpolationFactor = 2f;
            #endregion

            #region NetworkStateMachine
            var networkStateMachine = bodyPrefab.AddComponent<NetworkStateMachine>();
            networkStateMachine.stateMachines = new EntityStateMachine[] { esmBody, esmWeapon, esmSlide };
            #endregion

            #region DeathRewards
            bodyPrefab.AddComponent<DeathRewards>().logUnlockableDef = log;
            #endregion

            #region EquipmentSlot
            bodyPrefab.AddComponent<EquipmentSlot>();
            #endregion

            #region SfxLocator
            var sfxLocator = bodyPrefab.AddComponent<SfxLocator>();
            sfxLocator.deathSound = ""; // each death state has its own sound
            sfxLocator.aliveLoopStart = "ER_Spider_Alive_Loop_Play";
            sfxLocator.aliveLoopStop = "ER_Spider_Alive_Loop_Stop";
            #endregion

            #region KinematicCharacterMotor
            var capsuleCollider = bodyPrefab.GetComponent<CapsuleCollider>();

            var kinematicCharacterMotor = bodyPrefab.AddComponent<KinematicCharacterMotor>();
            kinematicCharacterMotor.CharacterController = characterMotor;
            kinematicCharacterMotor.Capsule = capsuleCollider;
            kinematicCharacterMotor._attachedRigidbody = bodyPrefab.GetComponent<Rigidbody>();

            // new shit
            kinematicCharacterMotor.StableGroundLayers = LayerIndex.world.mask;
            kinematicCharacterMotor.AllowSteppingWithoutStableGrounding = false;
            kinematicCharacterMotor.LedgeAndDenivelationHandling = true;
            kinematicCharacterMotor.SimulatedCharacterMass = 1f;
            kinematicCharacterMotor.CheckMovementInitialOverlaps = true;
            kinematicCharacterMotor.KillVelocityWhenExceedMaxMovementIterations = true;
            kinematicCharacterMotor.KillRemainingMovementWhenExceedMaxMovementIterations = true;
            kinematicCharacterMotor.DiscreteCollisionEvents = false;
            // end new shit

            kinematicCharacterMotor.CapsuleRadius = capsuleCollider.radius;
            kinematicCharacterMotor.CapsuleHeight = capsuleCollider.height;
            if (capsuleCollider.center != Vector3.zero)
            {
                Log.Error("CapsuleCollider for " + bodyPrefab + " has non-zero center. This WILL result in pathing issues for AI.");
            }
            kinematicCharacterMotor.CapsuleYOffset = 0f;

            kinematicCharacterMotor.GroundDetectionExtraDistance = 0f;
            kinematicCharacterMotor.MaxStepHeight = 0.2f;
            kinematicCharacterMotor.MinRequiredStepDepth = 0.1f;
            kinematicCharacterMotor.MaxStableSlopeAngle = 55f;
            kinematicCharacterMotor.MaxStableDistanceFromLedge = 0.5f;
            kinematicCharacterMotor.MaxStableDenivelationAngle = 55f;

            kinematicCharacterMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
            kinematicCharacterMotor.PreserveAttachedRigidbodyMomentum = true;

            kinematicCharacterMotor.HasPlanarConstraint = false;
            kinematicCharacterMotor.PlanarConstraintAxis = new Vector3(0f, 0f, 1f);

            kinematicCharacterMotor.StepHandling = StepHandlingMethod.Standard;
            kinematicCharacterMotor.InteractiveRigidbodyHandling = true;
            kinematicCharacterMotor.playerCharacter = true; // thanks Randy
            #endregion

            #region SetStateOnHurt
            var setStateOnHurt = bodyPrefab.AddComponent<SetStateOnHurt>();
            setStateOnHurt.targetStateMachine = esmBody;
            setStateOnHurt.idleStateMachine = new EntityStateMachine[] { esmWeapon, esmSlide };
            setStateOnHurt.hurtState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.HurtState));
            setStateOnHurt.hitThreshold = 0.3f;
            setStateOnHurt.canBeHitStunned = true;
            setStateOnHurt.canBeStunned = true;
            setStateOnHurt.canBeFrozen = true;
            #endregion

            #region ExecuteSkillOnDamage
            var skillOnDamage = bodyPrefab.AddComponent<ExecuteSkillOnDamage>();
            skillOnDamage.characterBody = characterBody;
            skillOnDamage.skillToExecute = gsUtility;
            skillOnDamage.mainStateMachine = esmBody;
            #endregion

            #endregion

            #region SetupBoxes

            var surfaceDef = Addressables.LoadAssetAsync<SurfaceDef>("RoR2/Base/RoboBallBoss/sdRoboBall.asset").WaitForCompletion(); // TODO: maybe make my own

            var hurtBoxesTransform = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "Hurtbox").ToArray();
            List<HurtBox> hurtBoxes = new List<HurtBox>();
            foreach (Transform t in hurtBoxesTransform)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            var sniperHurtBoxes = bodyPrefab.GetComponentsInChildren<Transform>().Where(t => t.name == "SniperHurtbox").ToArray();
            foreach (Transform t in sniperHurtBoxes)
            {
                var hurtBox = t.gameObject.AddComponent<HurtBox>();
                hurtBox.healthComponent = healthComponent;
                hurtBox.damageModifier = HurtBox.DamageModifier.Normal;
                hurtBox.isSniperTarget = true;
                hurtBoxes.Add(hurtBox);

                t.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;
            }

            var mainHurtboxTransform = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/MainHurtbox");
            var mainHurtBox = mainHurtboxTransform.gameObject.AddComponent<HurtBox>();
            mainHurtBox.healthComponent = healthComponent;
            mainHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtBox.isBullseye = true;
            hurtBoxes.Add(mainHurtBox);

            mainHurtboxTransform.gameObject.AddComponent<SurfaceDefProvider>().surfaceDef = surfaceDef;

            //var hitBox = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/Armature/Root/Root_Pelvis_Control/Bone.001/Bone.002/Bone.003/Head/Hitbox").gameObject.AddComponent<HitBox>();
            #endregion

            #region mdlMechanicalSpider
            var mdlMechanicalSpider = modelTransform.gameObject;

            // FIXING IMPACT\LIGHTIMPACT\LANDING ISSUES:
            // first of all, animation should not have "landing" by itself in it, only the impact of landing
            // second, add a reference pose to animation at the time of where animation reaches its resting position
            // third, if your animation doesn't do the first rule then just clip it so it has minimal body movement
            // and only has the impact of landing 

            #region AimAnimator
            // if you are having issues with AimAnimator,
            // * just add Additive Reference Pose for your pitch and yaw animations in the middle of the animation
            // * make both animations loop
            // * set them both to zero speed in your animation controller
            // * I haven't found how to add poses to "separate" animation files, so those have to be in fbx
            var aimAnimator = mdlMechanicalSpider.AddComponent<AimAnimator>();
            aimAnimator.inputBank = inputBank;
            aimAnimator.directionComponent = characterDirection;

            aimAnimator.pitchRangeMin = -65f; // its looking up, not down, for fuck sake
            aimAnimator.pitchRangeMax = 65f;

            aimAnimator.yawRangeMin = -180f;
            aimAnimator.yawRangeMax = 180f;
            aimAnimator.fullYaw = true;

            aimAnimator.pitchGiveupRange = 40f;
            aimAnimator.yawGiveupRange = 20f;

            aimAnimator.giveupDuration = 3f;

            aimAnimator.raisedApproachSpeed = 720f;
            aimAnimator.loweredApproachSpeed = 360f;
            aimAnimator.smoothTime = 0.1f;

            aimAnimator.aimType = AimAnimator.AimType.Direct;

            aimAnimator.enableAimWeight = false;
            aimAnimator.UseTransformedAimVector = false;
            #endregion

            #region ChildLocator
            var childLocator = mdlMechanicalSpider.AddComponent<ChildLocator>();
            var ourChildLocator = mdlMechanicalSpider.GetComponent<OurChildLocator>();
            childLocator.transformPairs = Array.ConvertAll(ourChildLocator.transformPairs, item =>
            {
                return new ChildLocator.NameTransformPair
                {
                    name = item.name,
                    transform = item.transform,
                };
            });
            UnityEngine.Object.Destroy(ourChildLocator);
            #endregion

            #region HurtBoxGroup
            var hurtboxGroup = mdlMechanicalSpider.AddComponent<HurtBoxGroup>();
            hurtboxGroup.hurtBoxes = hurtBoxes.ToArray();
            for (short i = 0; i < hurtboxGroup.hurtBoxes.Length; i++)
            {
                hurtboxGroup.hurtBoxes[i].hurtBoxGroup = hurtboxGroup;
                hurtboxGroup.hurtBoxes[i].indexInGroup = i;
                if (hurtboxGroup.hurtBoxes[i].isBullseye)
                {
                    hurtboxGroup.bullseyeCount++;
                }
            }
            hurtboxGroup.mainHurtBox = mainHurtBox;
            #endregion

            #region AnimationEvents
            if (!mdlMechanicalSpider.TryGetComponent<AnimationEvents>(out _))
            {
                mdlMechanicalSpider.AddComponent<AnimationEvents>();
            }
            #endregion

            #region DestroyOnUnseen
            mdlMechanicalSpider.AddComponent<DestroyOnUnseen>().cull = false;
            #endregion

            #region CharacterModel
            var characterModel = mdlMechanicalSpider.AddComponent<CharacterModel>();
            characterModel.body = characterBody;
            characterModel.itemDisplayRuleSet = CreateItemDisplayRuleSet();
            characterModel.autoPopulateLightInfos = true;
            characterModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
            new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = modelRenderer.material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    hideOnDeath = false
                }
            };
            #endregion

            #region FootstepHandler
            FootstepHandler footstepHandler = null;
            if (!mdlMechanicalSpider.TryGetComponent(out footstepHandler))
            {
                footstepHandler = mdlMechanicalSpider.AddComponent<FootstepHandler>();
            }
            footstepHandler.enableFootstepDust = true;
            footstepHandler.baseFootstepString = "Play_treeBot_step";
            footstepHandler.footstepDustPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/GenericFootstepDust.prefab").WaitForCompletion();
            #endregion

            #region ModelPanelParameters
            var modelPanelParameters = mdlMechanicalSpider.AddComponent<ModelPanelParameters>();
            modelPanelParameters.focusPointTransform = focusPoint;
            modelPanelParameters.cameraPositionTransform = cameraPosition;
            modelPanelParameters.modelRotation = new Quaternion(0, 0, 0, 1);
            modelPanelParameters.minDistance = 1.5f;
            modelPanelParameters.maxDistance = 6f;
            #endregion

            #region SkinDefs
            SkinDefs.Default = CreateSkinDef("skinMechanicalSpiderDefault", mdlMechanicalSpider, characterModel.baseRendererInfos);

            CharacterModel.RendererInfo[] lakesRender = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    renderer = modelRenderer,
                    defaultMaterial = ContentProvider.MaterialCache["matMechanicalSpider"],
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    hideOnDeath = false
                },
            };

            var modelSkinController = mdlMechanicalSpider.AddComponent<ModelSkinController>();
            modelSkinController.skins = new SkinDef[]
            {
                SkinDefs.Default
            };
            #endregion

            //var helper = mdlMechanicalSpider.AddComponent<AnimationParameterHelper>();
            //helper.animator = modelTransform.gameObject.GetComponent<Animator>();
            //helper.animationParameters = new string[] { "walkSpeedDebug" };

            #region ParticleEffects
            var rightFrontLeg = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg1.1/SparkRightFrontLeg");
            var backLeftLeg = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg3.1/SparkBackLeftLeg");

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

            var smoke = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Smoke");
            var smokeGameObject = brokenMissileDrone.transform.Find("ModelBase/BrokenDroneVFX/Damage Point/Smoke, Point").gameObject;
            var smokeCenter = UnityEngine.GameObject.Instantiate(smokeGameObject);
            smokeCenter.transform.parent = smoke;
            smokeCenter.transform.localPosition = Vector3.zero;
            smokeCenter.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            #endregion

            #endregion

            #region AimAssist
            var aimAssistTarget = bodyPrefab.transform.Find("ModelBase/mdlMechanicalSpider/AimAssist").gameObject.AddComponent<AimAssistTarget>();
            aimAssistTarget.point0 = headTransform;
            aimAssistTarget.point1 = rootTransform;
            aimAssistTarget.assistScale = 1f;
            aimAssistTarget.healthComponent = healthComponent;
            aimAssistTarget.teamComponent = teamComponent;
            #endregion
        }

        private void AddMainMasterComponents(GameObject masterPrefab, GameObject bodyPrefab)
        {
            #region NetworkIdentity
            masterPrefab.AddComponent<NetworkIdentity>().localPlayerAuthority = true;
            #endregion

            #region CharacterMaster
            var characterMaster = masterPrefab.AddComponent<CharacterMaster>();
            characterMaster.bodyPrefab = bodyPrefab;
            characterMaster.spawnOnStart = false;
            characterMaster.teamIndex = TeamIndex.Monster;
            characterMaster.destroyOnBodyDeath = true;
            characterMaster.isBoss = false;
            characterMaster.preventGameOver = true;
            #endregion

            #region Inventory
            masterPrefab.AddComponent<Inventory>();
            #endregion

            #region EntityStateMachineAI
            var esmAI = masterPrefab.AddComponent<EntityStateMachine>();
            esmAI.customName = "AI";
            esmAI.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            esmAI.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            #endregion

            #region BaseAI
            var baseAI = masterPrefab.AddComponent<BaseAI>();
            baseAI.fullVision = false;
            baseAI.neverRetaliateFriendlies = true;
            baseAI.enemyAttentionDuration = 5f;
            baseAI.desiredSpawnNodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            baseAI.stateMachine = esmAI;
            baseAI.scanState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.AI.Walker.Wander));
            baseAI.isHealer = false;
            baseAI.enemyAttention = 0f;
            baseAI.aimVectorDampTime = 0.05f;
            baseAI.aimVectorMaxSpeed = 180f;
            #endregion

            #region MinionOwnership
            if (!masterPrefab.TryGetComponent<MinionOwnership>(out _))
            {
                masterPrefab.AddComponent<MinionOwnership>();
            }
            #endregion
        }

        public GameObject CreateInteractable(GameObject interactablePrefab, GameObject masterPrefab)
        {
            var meshRendererTransform = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/MechanicalSpider");
            var meshRenderer = meshRendererTransform.GetComponent<SkinnedMeshRenderer>();
            var hologramPivot = interactablePrefab.transform.Find("HologramPivot");
            var modelTransform = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider");
            var modelBase = interactablePrefab.transform.Find("ModelBase");

            #region MechanicalSpiderBroken

            #region NetworkIdentity
            interactablePrefab.AddComponent<NetworkIdentity>();
            #endregion

            #region Highlight
            var highlight = interactablePrefab.AddComponent<Highlight>();
            highlight.targetRenderer = meshRenderer;
            highlight.strength = 1f;
            highlight.highlightColor = Highlight.HighlightColor.interactive;
            #endregion

            #region SummonMasterBehavior
            var summonMaster = interactablePrefab.AddComponent<SummonMasterBehavior>();
            summonMaster.masterPrefab = masterPrefab;
            summonMaster.callOnEquipmentSpentOnPurchase = false;
            summonMaster.destroyAfterSummoning = false;
            #endregion

            #region PurchaseInteraction
            var purchaseInteraction = interactablePrefab.AddComponent<PurchaseInteraction>();
            purchaseInteraction.displayNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_INTERACTABLE_NAME";
            purchaseInteraction.contextToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_CONTEXT";
            purchaseInteraction.available = true;
            purchaseInteraction.costType = CostTypeIndex.Money;
            purchaseInteraction.cost = EnemiesReturns.Configuration.MechanicalSpider.DroneCost.Value;
            purchaseInteraction.solitudeCost = 0;
            purchaseInteraction.automaticallyScaleCostWithDifficulty = false; // TODO: maybe?
            purchaseInteraction.requiredUnlockable = "";
            purchaseInteraction.ignoreSpherecastForInteractability = false;
            purchaseInteraction.purchaseStatNames = new string[] { "totalDronesPurchased" };
            purchaseInteraction.setUnavailableOnTeleporterActivated = false;
            purchaseInteraction.isShrine = false;
            purchaseInteraction.isGoldShrine = false;
            purchaseInteraction.shouldProximityHighlight = true;
            purchaseInteraction.saleStarCompatible = false;
            purchaseInteraction.requiredExpansion = null;
            purchaseInteraction.requiredUnlockable = null;
            #endregion

            #region EventFunctions
            var eventFunctions = interactablePrefab.AddComponent<EventFunctions>();
            #endregion

            #region HologramProjector
            var projector = interactablePrefab.AddComponent<HologramProjector>();
            projector.displayDistance = 15f;
            projector.hologramPivot = hologramPivot;
            projector.disableHologramRotation = false;
            projector.hologramContentInstance = null;
            #endregion

            #region GenericDisplayNameProvider
            interactablePrefab.AddComponent<GenericDisplayNameProvider>().displayToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_BODY_NAME"; // its empty for drones so who knows
            #endregion

            #region ModelLocator
            var modelLocator = interactablePrefab.AddComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelVisibility = null;
            modelLocator.modelBaseTransform = modelBase;
            modelLocator.modelScaleCompensation = 1f;

            modelLocator.autoUpdateModelTransform = false;
            modelLocator.dontDetatchFromParent = true;

            modelLocator.noCorpse = false;
            modelLocator.dontReleaseModelOnDeath = false;
            modelLocator.preserveModel = false;
            modelLocator.forceCulled = false;

            modelLocator.normalizeToFloor = true;
            modelLocator.normalSmoothdampTime = 0.1f;
            modelLocator.normalMaxAngleDelta = 90f;
            #endregion

            #region GenericInspectInfoProvider
            interactablePrefab.AddComponent<GenericInspectInfoProvider>().InspectInfo = null; // TODO
            #endregion

            #region Inventory
            interactablePrefab.AddComponent<Inventory>();
            #endregion

            #region SetEliteRampFromInventory
            var setEliteRamp = interactablePrefab.AddComponent<SetEliteRampOnShader>();
            setEliteRamp.renderers = interactablePrefab.GetComponentsInChildren<Renderer>();
            #endregion

            #region SpiderDroneOnPurchaseEvents
            var purchaseEvetns = interactablePrefab.AddComponent<SpiderDroneOnPurchaseEvents>();
            purchaseEvetns.purchaseInteraction = purchaseInteraction;
            purchaseEvetns.eventFunctions = eventFunctions;
            purchaseEvetns.summonMasterBehavior = summonMaster;
            #endregion

            #region ParticleEffects
            var rightFrontLeg = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg1.1/SparkRightFrontLeg");
            var backLeftLeg = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/Leg3.1");

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

            var smoke = interactablePrefab.transform.Find("ModelBase/mdlMechanicalSpider/SpiderArmature/Root/UpperBody");
            var smokeGameObject = brokenMissileDrone.transform.Find("ModelBase/BrokenDroneVFX/Damage Point/Smoke, Point").gameObject;
            var smokeCenter = UnityEngine.GameObject.Instantiate(smokeGameObject);
            smokeCenter.transform.parent = smoke;
            smokeCenter.transform.localPosition = Vector3.zero;
            smokeCenter.transform.localScale = new Vector3(2f, 2f, 2f);
            #endregion

            #endregion

            meshRendererTransform.gameObject.AddComponent<EntityLocator>().entity = interactablePrefab;

            interactablePrefab.RegisterNetworkPrefab();

            return interactablePrefab;
        }

        #region SkillDefs

        internal SkillDef CreateDoubleShotSkill()
        {
            var bite = ScriptableObject.CreateInstance<SkillDef>();
            (bite as ScriptableObject).name = "MechanicalSpiderWeaponDoubleShot";
            bite.skillName = "DoubleShot";

            bite.skillNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DOUBLE_SHOT_NAME";
            bite.skillDescriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DOUBLE_SHOT_DESCRIPTION";
            //var acridBite = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoBite.asset").WaitForCompletion();
            //if (acridBite)
            //{
            //    bite.icon = acridBite.icon;
            //}

            bite.activationStateMachineName = "Weapon";
            bite.activationState = new EntityStates.SerializableEntityStateType(typeof(ModdedEntityStates.MechanicalSpider.DoubleShot.OpenHatch));
            bite.interruptPriority = EntityStates.InterruptPriority.Skill;

            bite.baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.DoubleShotCooldown.Value;
            bite.baseMaxStock = 1;
            bite.rechargeStock = 1;
            bite.requiredStock = 1;
            bite.stockToConsume = 1;

            bite.resetCooldownTimerOnUse = false;
            bite.fullRestockOnAssign = true;
            bite.dontAllowPastMaxStocks = false;
            bite.beginSkillCooldownOnSkillEnd = false;

            bite.cancelSprintingOnActivation = true;
            bite.forceSprintDuringState = false;
            bite.canceledFromSprinting = false;

            bite.isCombatSkill = true;
            bite.mustKeyPress = false;

            return bite;
        }

        internal SkillDef CreateDashSkill()
        {
            var spit = ScriptableObject.CreateInstance<SkillDef>();
            (spit as ScriptableObject).name = "MechanicalSpiderBodyDash";
            spit.skillName = "Dash";

            spit.skillNameToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DASH_NAME";
            spit.skillDescriptionToken = "ENEMIES_RETURNS_MECHANICAL_SPIDER_DASH_DESCRIPTION";
            //var crocoSpit = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoSpit.asset").WaitForCompletion();
            //if (crocoSpit)
            //{
            //    spit.icon = crocoSpit.icon;
            //}

            spit.activationStateMachineName = "Slide";
            spit.activationState = new EntityStates.SerializableEntityStateType(typeof(DashStart));
            spit.interruptPriority = EntityStates.InterruptPriority.Any;

            spit.baseRechargeInterval = EnemiesReturns.Configuration.MechanicalSpider.DashCooldown.Value;
            spit.baseMaxStock = 1;
            spit.rechargeStock = 1;
            spit.requiredStock = 1;
            spit.stockToConsume = 1;

            spit.resetCooldownTimerOnUse = false;
            spit.fullRestockOnAssign = true;
            spit.dontAllowPastMaxStocks = false;
            spit.beginSkillCooldownOnSkillEnd = false;

            spit.cancelSprintingOnActivation = true;
            spit.forceSprintDuringState = false;
            spit.canceledFromSprinting = false;

            spit.isCombatSkill = true;
            spit.mustKeyPress = false;

            return spit;
        }

        #endregion

        public InteractableSpawnCard CreateInteractableSpawnCard(string name, GameObject prefab)
        {
            var card = ScriptableObject.CreateInstance<InteractableSpawnCard>();
            (card as ScriptableObject).name = name;
            card.sendOverNetwork = true;
            card.prefab = prefab;
            card.hullSize = HullClassification.Human;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoChestSpawn;
            card.directorCreditCost = 0; // TODO: does it even matter?
            card.occupyPosition = true;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.orientToFloor = true;
            card.slightlyRandomizeOrientation = false;
            card.skipSpawnWhenSacrificeArtifactEnabled = false;
            card.weightScalarWhenSacrificeArtifactEnabled = 1f;
            card.skipSpawnWhenDevotionArtifactEnabled = true;
            card.maxSpawnsPerStage = -1;
            card.prismaticTrialSpawnChance = 1f;

            return card;
        }

        public CharacterSpawnCard CreateCharacterSpawnCard(string name, GameObject master, SkinDef skin = null, GameObject bodyGameObject = null)
        {
            var card = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            (card as ScriptableObject).name = name;
            card.prefab = master;
            card.sendOverNetwork = true;
            card.hullSize = HullClassification.Golem;
            card.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            card.requiredFlags = RoR2.Navigation.NodeFlags.None;
            card.forbiddenFlags = RoR2.Navigation.NodeFlags.NoCharacterSpawn;
            card.directorCreditCost = EnemiesReturns.Configuration.MechanicalSpider.DirectorCost.Value;
            card.occupyPosition = false;
            card.eliteRules = SpawnCard.EliteRules.Default;
            card.noElites = false;
            card.forbiddenAsBoss = false;
            if (skin && bodyGameObject && bodyGameObject.TryGetComponent<CharacterBody>(out var body))
            {
                card.loadout = new SerializableLoadout
                {
                    bodyLoadouts = new SerializableLoadout.BodyLoadout[]
                    {
                    new SerializableLoadout.BodyLoadout()
                    {
                        body = body,
                        skinChoice = skin,
                        skillChoices = Array.Empty<SerializableLoadout.BodyLoadout.SkillChoice>() // yes, we need it
                    }
                    }
                };
            };

            return card;
        }

        public GameObject CreateDoubleShotChargeEffect()
        {
            var chargeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ChargeGolem.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderCharge", false);

            var particleDuration = chargeEffect.GetComponent<ScaleParticleSystemDuration>();
            particleDuration.initialDuration = 3f; // same as charge state duration
            particleDuration._newDuration = 0.5f; // same as charge state duration

            var light = chargeEffect.transform.Find("Point light").gameObject.GetComponent<Light>();
            light.color = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            var glow = chargeEffect.transform.Find("Particles/Glow").gameObject.GetComponent<ParticleSystem>();

            var glowMain = glow.main;
            glowMain.simulationSpeed = 2f;

            var colorOverLifetime = glow.colorOverLifetime;

            var gradient = new Gradient();
            gradient.mode = GradientMode.Blend;
            gradient.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 0f, time = 0f },
                new GradientAlphaKey{alpha = 181f, time = 0.962f},
                new GradientAlphaKey{alpha = 255f, time = 1f}
            };
            gradient.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(0.8490566f, 0.6350543f, 0.1321645f), time = 0f},
                new GradientColorKey{color = new Color(0.5849056f, 0.4381365f, 0.09656459f), time = 0.942f},
                new GradientColorKey{color = new Color(1f, 1f, 0f), time = 1f},
            };

            colorOverLifetime.color = new ParticleSystem.MinMaxGradient(gradient);

            chargeEffect.transform.Find("Particles/Glow").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderChargeGlow", CreateChargeGlowMaterial);

            var sparks = chargeEffect.transform.Find("Particles/Sparks").gameObject.GetComponent<ParticleSystem>();
            var colorOverLifetime2 = sparks.colorOverLifetime;

            var gradient2 = new Gradient();
            gradient2.mode = GradientMode.Blend;
            gradient2.alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey{alpha = 255f, time = 0f },
                new GradientAlphaKey{alpha = 0f, time = 1f}
            };
            gradient2.colorKeys = new GradientColorKey[]
            {
                new GradientColorKey{color = new Color(0.8923398f, 1f, 0.1372549f), time = 0f},
                new GradientColorKey{color = new Color(1f, 0.6179392f, 0.03137255f), time = 0.05f},
                new GradientColorKey{color = new Color(1f, 0.6179392f, 0.03137255f), time = 1f},
            };

            colorOverLifetime2.color = new ParticleSystem.MinMaxGradient(gradient2);

            particleDuration.particleSystems = new ParticleSystem[] { glow };

            // adding stuff so it becomes an effect
            var vfxAttributes = chargeEffect.AddComponent<VFXAttributes>();
            vfxAttributes.DoNotPool = false;
            vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Medium;
            vfxAttributes.vfxIntensity = VFXAttributes.VFXIntensity.High;
            vfxAttributes.optionalLights = new Light[] { light };

            //var effectComponent = chargeEffect.AddComponent<EffectComponent>();
            //effectComponent.positionAtReferencedTransform = true;
            //effectComponent.parentToReferencedTransform = true;

            return chargeEffect;
        }

        public Material CreateChargeGlowMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matArcaneCircleProvi.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderChargeGlow";
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/UI/texCrosshairCircle.png").WaitForCompletion());

            return material;
        }

        public GameObject CreateDoubleShotProjectilePrefab(GameObject impactEffect)
        {
            var projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitProjectile.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotProjectile", true);

            var projectileController = projectilePrefab.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = CreateDoubleShotGhostPrefab();
            projectileController.flightSoundLoop = CreateProjectileLoopSoundDef();

            var projectileSingleTargetImpact = projectilePrefab.GetComponent<ProjectileSingleTargetImpact>();
            projectileSingleTargetImpact.impactEffect = impactEffect;

            return projectilePrefab;
        }

        private LoopSoundDef CreateProjectileLoopSoundDef()
        {
            var projectileFlightSoundLoop = ScriptableObject.CreateInstance<LoopSoundDef>();
            (projectileFlightSoundLoop as ScriptableObject).name = "lsdMechanicalSpiderProjectileFlight";
            projectileFlightSoundLoop.startSoundName = "ER_Spider_Projectile_Loop_Play";
            projectileFlightSoundLoop.startSoundName = "ER_Spider_Projectile_Loop_Stop";

            ProjectileFlightSoundLoop = projectileFlightSoundLoop;

            return projectileFlightSoundLoop;
        }

        public GameObject CreateDoubleShotImpactEffect()
        {
            var projectileEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitImpactEffect.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotImpactEffect", false);

            var effectComponent = projectileEffect.GetComponent<EffectComponent>();
            effectComponent.soundName = "ER_Spider_Projectile_Hit_Play";

            UnityEngine.GameObject.DestroyImmediate(projectileEffect.transform.Find("Goo").gameObject);

            var flashParticleSystem = projectileEffect.transform.Find("Flash").gameObject.GetComponent<ParticleSystem>();
            var main = flashParticleSystem.main;
            main.startColor = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            projectileEffect.transform.Find("Point Light").gameObject.GetComponent<Light>().color = new Color(0.8490566f, 0.6350543f, 0.1321645f);
            projectileEffect.transform.Find("Ring, Mesh").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectile", CreateDoubleShotProjectileMaterial);

            return projectileEffect;
        }

        public GameObject CreateDoubleShotGhostPrefab()
        {
            var projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/VerminSpitGhost.prefab").WaitForCompletion().InstantiateClone("MechanicalSpiderDoubleShotProjectileGhost", false);
            projectileGhost.transform.Find("Goo, WS").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectile", CreateDoubleShotProjectileMaterial);
            projectileGhost.transform.Find("Goo, Directional").gameObject.GetComponent<ParticleSystemRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotProjectile", CreateDoubleShotProjectileMaterial);
            projectileGhost.transform.Find("Trail").gameObject.GetComponent<TrailRenderer>().material = ContentProvider.GetOrCreateMaterial("matMechanicalSpiderDoubleShotTrail", CreateDoubleShotTrailMaterial);

            projectileGhost.transform.Find("Point light").gameObject.GetComponent<Light>().color = new Color(0.8490566f, 0.6350543f, 0.1321645f);

            UnityEngine.GameObject.DestroyImmediate(projectileGhost.GetComponent<DetachParticleOnDestroyAndEndEmission>()); // fixes log spam on projectile destroy

            return projectileGhost;
        }

        public Material CreateDoubleShotProjectileMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/FlyingVermin/matVerminGooSmall.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotProjectile";
            material.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Junk/Mage/texMageLaserMask.png").WaitForCompletion());
            material.SetTexture("_Cloud1Tex", Addressables.LoadAssetAsync<Texture2D>("RoR2/Base/Common/texCloudPixel2.png").WaitForCompletion());
            material.SetTextureScale("_Cloud1Tex", new Vector2(0.5f, 0.5f));
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());
            material.SetFloat("_InvFade", 7f);
            material.SetFloat("_AlphaBoost", 7.32f);
            material.SetFloat("_Cutoff", 0.542f);
            material.SetFloat("_SpecularStrength", 0.701f);
            material.SetFloat("_SpecularExponent", 2.86f);
            material.SetInt("_RampInfo", 0);
            return material;
        }

        public Material CreateDoubleShotTrailMaterial()
        {
            var material = UnityEngine.Object.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/FlyingVermin/matVerminGooTrail.mat").WaitForCompletion());
            material.name = "matMechanicalSpiderDoubleShotTrail";
            material.SetTexture("_RemapTex", Addressables.LoadAssetAsync<Texture2D>("RoR2/DLC1/Common/ColorRamps/texRampConstructLaser.png").WaitForCompletion());

            return material;
        }

        private ItemDisplayRuleSet CreateItemDisplayRuleSet()
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
                childName = "JawR",
                localPos = new Vector3(-0.32686F, 2.51006F, -0.21041F),
                localAngles = new Vector3(354.7525F, 340F, 7.12234F),
                localScale = new Vector3(0.6F, 0.6F, 0.6F),
                limbMask = LimbFlags.None
            });

            displayRuleGroupFire.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = fireEquipDisplay,
                childName = "JawL",
                localPos = new Vector3(-0.12522F, 2.55699F, -0.28728F),
                localAngles = new Vector3(354.7525F, 20.00001F, 351.6044F),
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
                childName = "Head",
                localPos = new Vector3(0F, -0.29125F, -0.36034F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.43975F, 0.43975F, 0.43975F),
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
                localPos = new Vector3(-0.36417F, 4.08597F, -0.81975F),
                localAngles = new Vector3(88.15041F, 342.9204F, 152.0255F),
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
                childName = "TailEnd",
                localPos = new Vector3(-0.00302F, 0.77073F, 0.00143F),
                localAngles = new Vector3(284.2227F, 198.9412F, 159.205F),
                localScale = new Vector3(1.15579F, 1.15579F, 1.15579F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "JawL",
                localPos = new Vector3(-0.25677F, 2.49244F, -0.13195F),
                localAngles = new Vector3(323.8193F, 261.7038F, 7.48606F),
                localScale = new Vector3(1.45f, 1.45f, 1.45f),
                limbMask = LimbFlags.None
            });
            displayRuleGroupLightning.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/DisplayEliteRhinoHorn.prefab").WaitForCompletion(),
                childName = "JawR",
                localPos = new Vector3(-0.00804F, 2.49258F, -0.13194F),
                localAngles = new Vector3(322.8305F, 89.99672F, 7F),
                localScale = new Vector3(1.45f, 1.45f, 1.45f),
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
                childName = "Head",
                localPos = new Vector3(0F, 0F, 0F),
                localAngles = new Vector3(270F, 0F, 0F),
                localScale = new Vector3(0.8F, 0.8F, 0.8F),
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
                childName = "JawL",
                localPos = new Vector3(0F, 0.38638F, -0.00001F),
                localAngles = new Vector3(0F, 270F, 0F),
                localScale = new Vector3(0.5F, 0.5F, 0.5F),
                limbMask = LimbFlags.None
            });
            displayRuleGroupPoison.AddDisplayRule(new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/DisplayEliteUrchinCrown.prefab").WaitForCompletion(),
                childName = "JawR",
                localPos = new Vector3(0F, 0.38638F, -0.00001F),
                localAngles = new Vector3(0F, 90F, 0F),
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
                localPos = new Vector3(-0.12323F, 2.48183F, -0.47279F),
                localAngles = new Vector3(7.00848F, 2.28661F, 1.77239F),
                localScale = new Vector3(4.42437F, 4.42437F, 4.42437F),
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
                localPos = new Vector3(0F, 1.1304F, 0.00001F),
                localAngles = new Vector3(0F, 0F, 0F),
                localScale = new Vector3(0.84412F, 0.84412F, 0.84412F),
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
                localPos = new Vector3(0.03071F, 0.76725F, -0.77548F),
                localAngles = new Vector3(285.3486F, 351.2853F, 11.4788F),
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
                localPos = new Vector3(-0.08902F, 0.78706F, 1.13371F),
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

        private void Renamer(GameObject object1)
        {

        }
    }
}
