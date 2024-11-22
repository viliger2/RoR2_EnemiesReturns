using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesReturns.Components.BodyComponents.Skills
{
    public interface ISkillLocator
    {
        protected bool NeedToAddSkillLocator();

        internal SkillLocator AddSkillLocator(GameObject bodyPrefab, Dictionary<SkillSlot, GenericSkill> skillDictionary)
        {
            SkillLocator skillLocator = null;
            if (NeedToAddSkillLocator())
            {
                skillLocator = bodyPrefab.GetOrAddComponent<SkillLocator>();
                skillDictionary.TryGetValue(SkillSlot.Primary, out skillLocator.primary);
                skillDictionary.TryGetValue(SkillSlot.Secondary, out skillLocator.secondary);
                skillDictionary.TryGetValue(SkillSlot.Special, out skillLocator.special);
                skillDictionary.TryGetValue(SkillSlot.Utility, out skillLocator.utility);
            }
            return skillLocator;
        }
    }
}
