using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.Skills
{
    public interface IGenericSkill
    {
        protected class GenericSkillParams
        {
            public GenericSkillParams(SkillFamily family, string skillName, SkillSlot slot)
            {
                this.family = family;
                this.skillName = skillName;
                this.slot = slot;
            }

            public SkillFamily family;
            public string skillName;
            public bool hideInCharacterSelect = false;
            public SkillSlot slot;
        }

        protected GenericSkillParams[] GetGenericSkillParams();

        protected bool NeedToAddGenericSkills();

        protected Dictionary<SkillSlot, GenericSkill> AddGenericSkills(GameObject bodyPrefab, GenericSkillParams[] genericSkillParams)
        {
            Dictionary<SkillSlot, GenericSkill> skills = new Dictionary<SkillSlot, GenericSkill>();
            if (NeedToAddGenericSkills())
            {
                foreach (var skillParam in genericSkillParams)
                {
                    if (!skillParam.family)
                    {
                        Log.Warning($"SkillFamily for skillName {skillParam.skillName} is null! This WILL result in being stuck at 99%!");
                    }
                    var skill = bodyPrefab.AddComponent<GenericSkill>();
                    skill._skillFamily = skillParam.family;
                    skill.skillName = skillParam.skillName;
                    skill.hideInCharacterSelect = skillParam.hideInCharacterSelect;

                    skills.Add(skillParam.slot, skill);
                }
            }
            return skills;
        }
    }
}
