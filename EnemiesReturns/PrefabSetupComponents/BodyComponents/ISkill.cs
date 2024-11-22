using EnemiesReturns.Components.BodyComponents.Skills;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents
{
    public interface ISkills : IGenericSkill, ISkillLocator
    {
        public void AddSkills(GameObject body)
        {
            var skillDictionary = AddGenericSkills(body, GetGenericSkillParams());
            AddSkillLocator(body, skillDictionary);
        }
    }
}
