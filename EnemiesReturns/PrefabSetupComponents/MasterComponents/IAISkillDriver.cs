using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using UnityEngine;

namespace EnemiesReturns.Components.MasterComponents
{
    public interface IAISkillDriver
    {
        protected class AISkillDriverParams
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
            public float minDistance = 0;
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

        protected bool NeedToAddAISkillDriver();

        protected AISkillDriverParams[] GetAISkillDriverParams();

        protected void AddAISkillDrivers(GameObject masterPrefab, AISkillDriverParams[] aiParams)
        {
            if (NeedToAddAISkillDriver())
            {
                foreach (var aiParam in aiParams)
                {
                    var skillDriver = masterPrefab.AddComponent<AISkillDriver>();
                    skillDriver.customName = aiParam.customName;
                    skillDriver.skillSlot = aiParam.skillSlot;

                    skillDriver.requiredSkill = aiParam.requiredSkill;
                    skillDriver.requireSkillReady = aiParam.requireSkillReady;
                    skillDriver.requireEquipmentReady = aiParam.requireEquipmentReady;
                    skillDriver.minUserHealthFraction = aiParam.minUserHealthFraction;
                    skillDriver.maxUserHealthFraction = aiParam.maxUserHealthFraction;
                    skillDriver.minTargetHealthFraction = aiParam.minTargetHealthFraction;
                    skillDriver.maxTargetHealthFraction = aiParam.maxTargetHealthFraction;
                    skillDriver.minDistance = aiParam.minDistance;
                    skillDriver.maxDistance = aiParam.maxDistance;
                    skillDriver.selectionRequiresTargetLoS = aiParam.selectionRequiresTargetLoS;
                    skillDriver.selectionRequiresOnGround = aiParam.selectionRequiresOnGround;
                    skillDriver.selectionRequiresAimTarget = aiParam.selectionRequiresAimTarget;
                    skillDriver.maxTimesSelected = aiParam.maxTimesSelected;

                    skillDriver.moveTargetType = aiParam.moveTargetType;
                    skillDriver.activationRequiresAimTargetLoS = aiParam.activationRequiresAimTargetLoS;
                    skillDriver.activationRequiresTargetLoS = aiParam.activationRequiresTargetLoS;
                    skillDriver.activationRequiresAimConfirmation = aiParam.activationRequiresAimConfirmation;
                    skillDriver.movementType = aiParam.movementType;
                    skillDriver.moveInputScale = aiParam.moveInputScale;
                    skillDriver.aimType = aiParam.aimType;
                    skillDriver.ignoreNodeGraph = aiParam.ignoreNodeGraph;
                    skillDriver.shouldSprint = aiParam.shouldSprint;
                    skillDriver.shouldFireEquipment = aiParam.shouldFireEquipment;
                    skillDriver.buttonPressType = aiParam.buttonPressType;

                    skillDriver.driverUpdateTimerOverride = aiParam.driverUpdateTimerOverride;
                    skillDriver.resetCurrentEnemyOnNextDriverSelection = aiParam.resetCurrentEnemyOnNextDriverSelection;
                    skillDriver.noRepeat = aiParam.noRepeat;
                    skillDriver.nextHighPriorityOverride = aiParam.nextHighPriorityOverride;
                }
            }
        }

    }
}
