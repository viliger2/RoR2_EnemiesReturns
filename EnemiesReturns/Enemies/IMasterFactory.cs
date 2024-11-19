using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static EnemiesReturns.Utils;

namespace EnemiesReturns.Enemies
{
    public interface IMasterFactory
    {
        internal class CharacterMasterParams
        {
            public CharacterMasterParams(GameObject bodyPrefab)
            {
                this.bodyPrefab = bodyPrefab;
            }

            public GameObject bodyPrefab;
            public bool spawnOnStart = false; // basically never
            public TeamIndex teamIndex = TeamIndex.Monster;
            public bool destroyBodyOnDeath = true;
            public bool isBoss = false;
            public bool preventGameOver = true;
        }

        internal class BaseAIParams
        {
            public BaseAIParams(RoR2.Navigation.MapNodeGroup.GraphType graphType, EntityStateMachine stateMachine, EntityStates.SerializableEntityStateType scanState)
            {
                this.graphType = graphType;
                this.stateMachine = stateMachine;
                this.scanState = scanState;
            }

            public bool fullVision = false; // stationary and drones have it set to true
            public bool neverRetaliateFriendlies = true;
            public float enemyAttentionDuration = 5f;
            public RoR2.Navigation.MapNodeGroup.GraphType graphType;
            public EntityStateMachine stateMachine;
            public EntityStates.SerializableEntityStateType scanState;
            public bool isHealer = false;
            public float enemyAttention = 0f;
            public float aimVectorDampTime = 0.05f;
            public float aimVectorMaxSpeed = 180f;
        }

        internal class AISkillDriverParams
        {
            public AISkillDriverParams(string name)
            {
                this.customName = name;
            }

            public string customName = "";
            public SkillSlot skillSlot = SkillSlot.None;

            public SkillDef requiredSkill = null;
            public bool requireSkillReady = false;
            public bool requireEquipmentReady = false;
            public float minUserHealthFraction = float.NegativeInfinity;
            public float maxUserHealthFraction = float.PositiveInfinity;
            public float minTargetHealthFraction = float.NegativeInfinity;
            public float maxTargetHealthFraction = float.PositiveInfinity;
            public float minDistance = float.NegativeInfinity;
            public float maxDistance = float.PositiveInfinity;
            public bool selectionRequiresTargetLoS = false;
            public bool selectionRequiresOnGround = false;
            public bool selectionRequiresAimTarget = false;
            public int maxTimesSelected = -1;

            public AISkillDriver.TargetType moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            public bool activationRequiresTargetLoS = false;
            public bool activationRequiresAimTargetLoS = false;
            public bool activationRequiresAimConfirmation = false;
            public AISkillDriver.MovementType movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            public float moveInputScale = 1f;
            public AISkillDriver.AimType aimType = AISkillDriver.AimType.AtCurrentEnemy;
            public bool ignoreNodeGraph = false;
            public bool shouldSprint = false;
            public bool shouldFireEquipment = false;
            public AISkillDriver.ButtonPressType buttonPressType = AISkillDriver.ButtonPressType.Hold;

            public float driverUpdateTimerOverride = -1f;
            public bool resetCurrentEnemyOnNextDriverSelection = false;
            public bool noRepeat = false;
            public AISkillDriver nextHighPriorityOverride = null;
        }

        internal CharacterMaster AddCharacterMaster(GameObject masterPrefab, CharacterMasterParams masterParams)
        {
            var characterMaster = masterPrefab.GetOrAddComponent<CharacterMaster>();
            characterMaster.bodyPrefab = masterParams.bodyPrefab;
            characterMaster.spawnOnStart = masterParams.spawnOnStart;
            characterMaster.teamIndex = masterParams.teamIndex;
            characterMaster.destroyOnBodyDeath = masterParams.destroyBodyOnDeath;
            characterMaster.isBoss = masterParams.isBoss;
            characterMaster.preventGameOver = masterParams.preventGameOver;

            return characterMaster;
        }

        internal Inventory AddInventory(GameObject masterPrefab)
        {
            return masterPrefab.GetOrAddComponent<Inventory>();
        }

        internal BaseAI AddBaseAI(GameObject masterPrefab, BaseAIParams aiParams)
        {
            var baseAI = masterPrefab.GetOrAddComponent<BaseAI>();
            baseAI.fullVision = aiParams.fullVision;
            baseAI.neverRetaliateFriendlies = aiParams.neverRetaliateFriendlies;
            baseAI.enemyAttentionDuration = aiParams.enemyAttentionDuration;
            baseAI.desiredSpawnNodeGraphType = aiParams.graphType;
            baseAI.stateMachine = aiParams.stateMachine;
            baseAI.scanState = aiParams.scanState;
            baseAI.isHealer = aiParams.isHealer;
            baseAI.enemyAttention = aiParams.enemyAttention;
            baseAI.aimVectorDampTime = aiParams.aimVectorDampTime;
            baseAI.aimVectorMaxSpeed = aiParams.aimVectorMaxSpeed;

            return baseAI;
        }

        internal MinionOwnership AddMinionOwnership(GameObject masterPrefab)
        {
            return masterPrefab.GetOrAddComponent<MinionOwnership>();
        }

        internal AISkillDriver AddAISkillDriver(GameObject masterPrefab, AISkillDriverParams aiParams)
        {
            var skillDriver = masterPrefab.AddComponent<AISkillDriver>();
            skillDriver.customName = aiParams.customName;
            skillDriver.skillSlot = aiParams.skillSlot;

            skillDriver.requiredSkill = aiParams.requiredSkill;
            skillDriver.requireSkillReady = aiParams.requireSkillReady;
            skillDriver.requireEquipmentReady = aiParams.requireEquipmentReady;
            skillDriver.minUserHealthFraction = aiParams.minUserHealthFraction;
            skillDriver.maxUserHealthFraction = aiParams.maxUserHealthFraction;
            skillDriver.minTargetHealthFraction = aiParams.minTargetHealthFraction;
            skillDriver.maxTargetHealthFraction = aiParams.maxTargetHealthFraction;
            skillDriver.minDistance = aiParams.minDistance;
            skillDriver.maxDistance = aiParams.maxDistance;
            skillDriver.selectionRequiresTargetLoS = aiParams.selectionRequiresTargetLoS;
            skillDriver.selectionRequiresOnGround = aiParams.selectionRequiresOnGround;
            skillDriver.selectionRequiresAimTarget = aiParams.selectionRequiresAimTarget;
            skillDriver.maxTimesSelected = aiParams.maxTimesSelected;

            skillDriver.moveTargetType = aiParams.moveTargetType;
            skillDriver.activationRequiresAimTargetLoS = aiParams.activationRequiresAimTargetLoS;
            skillDriver.activationRequiresTargetLoS = aiParams.activationRequiresTargetLoS;
            skillDriver.activationRequiresAimConfirmation = aiParams.activationRequiresAimConfirmation;
            skillDriver.movementType = aiParams.movementType;
            skillDriver.moveInputScale = aiParams.moveInputScale;
            skillDriver.aimType = aiParams.aimType;
            skillDriver.ignoreNodeGraph = aiParams.ignoreNodeGraph;
            skillDriver.shouldSprint = aiParams.shouldSprint;
            skillDriver.shouldFireEquipment = aiParams.shouldFireEquipment;
            skillDriver.buttonPressType = aiParams.buttonPressType;

            skillDriver.driverUpdateTimerOverride = aiParams.driverUpdateTimerOverride;
            skillDriver.resetCurrentEnemyOnNextDriverSelection = aiParams.resetCurrentEnemyOnNextDriverSelection;
            skillDriver.noRepeat = aiParams.noRepeat;
            skillDriver.nextHighPriorityOverride = aiParams.nextHighPriorityOverride;

            return skillDriver;
        }

        public abstract GameObject CreateMaster(GameObject masterPrefab, GameObject bodyPrefab);

    }
}
