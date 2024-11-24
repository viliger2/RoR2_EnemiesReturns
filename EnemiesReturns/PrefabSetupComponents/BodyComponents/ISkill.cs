using EnemiesReturns.Components.BodyComponents.Skills;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ISkills : IGenericSkill, ISkillLocator
    {
        public Dictionary<SkillSlot, GenericSkill> AddSkills(GameObject body)
        {
            var skillDictionary = AddGenericSkills(body, GetGenericSkillParams());
            AddSkillLocator(body, skillDictionary);
            return skillDictionary;
        }
    }
}
