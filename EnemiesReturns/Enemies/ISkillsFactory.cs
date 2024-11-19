using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies
{
    public interface ISkillsFactory
    {
        internal class SkillParams
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

        internal SkillDef CreateSkill(SkillParams skillParams)
        {
            var skill = ScriptableObject.CreateInstance<SkillDef>();
            (skill as ScriptableObject).name = skillParams.name;
            skill.skillName = skillParams.name;

            skill.skillNameToken = skillParams.nameToken;
            skill.skillDescriptionToken = skillParams.descriptionToken;
            skill.icon = skillParams.icon;

            skill.activationStateMachineName = skillParams.activationStateMachine;
            skill.activationState = skillParams.activationState;
            skill.interruptPriority= skillParams.interruptPriority;

            skill.baseRechargeInterval = skillParams.baseRechargeInterval;
            skill.baseMaxStock= skillParams.baseMaxStock;
            skill.rechargeStock= skillParams.rechargeStock;
            skill.requiredStock= skillParams.requiredStock;
            skill.stockToConsume= skillParams.stockToConsume;

            skill.resetCooldownTimerOnUse = skillParams.resetCooldownTimerOnUse;
            skill.fullRestockOnAssign= skillParams.fullRestockOnAssign;
            skill.dontAllowPastMaxStocks = skillParams.dontAllowPAstMaxStocks;
            skill.beginSkillCooldownOnSkillEnd = skillParams.beginSkillCooldownOnSkillEnd;

            skill.canceledFromSprinting= skillParams.canceledFromSprinting;
            skill.forceSprintDuringState = skillParams.forceSprintDuringState;
            skill.canceledFromSprinting = skillParams.canceledFromSprinting;

            skill.isCombatSkill= skillParams.isCombatSkill;
            skill.mustKeyPress = skillParams.mustKeyPress;

            return skill;
        }

        void CreateSkills();
    }
}
