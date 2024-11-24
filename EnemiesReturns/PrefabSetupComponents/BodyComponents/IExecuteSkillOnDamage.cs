using EnemiesReturns.Behaviors;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns.PrefabSetupComponents.BodyComponents
{
    public interface IExecuteSkillOnDamage
    {

        protected class ExecuteSkillOnDamageParams
        {
            public string mainStateMachineName;
            public SkillSlot skillToExecute;
        }

        protected ExecuteSkillOnDamageParams GetExecuteSkillOnDamageParams();

        protected bool NeedToAddExecuteSkillOnDamage();

        protected ExecuteSkillOnDamage AddExecuteSkillOnDamage(GameObject bodyPrefab, CharacterBody characterBody, EntityStateMachine[] esms, Dictionary<SkillSlot, GenericSkill> skills, ExecuteSkillOnDamageParams skillOnDamageParams)
        {
            ExecuteSkillOnDamage skillOnDamage = null;
            if (NeedToAddExecuteSkillOnDamage())
            {
                var mainStateMachine = esms.First(item => item.customName == skillOnDamageParams.mainStateMachineName);
                if (!mainStateMachine)
                {
#if DEBUG || NOWEAVER
                    Log.Warning($"Couldn't find ESM {skillOnDamageParams.mainStateMachineName} on body {bodyPrefab}");
#endif
                    return null;
                }
                skills.TryGetValue(skillOnDamageParams.skillToExecute, out var skill);
                if (!skill)
                {
#if DEBUG || NOWEAVER
                    Log.Warning($"Couldn't find GenericSkill {skillOnDamageParams.skillToExecute} on body {bodyPrefab}");
#endif
                    return null;
                }

                skillOnDamage = bodyPrefab.GetOrAddComponent<ExecuteSkillOnDamage>();
                skillOnDamage.characterBody = characterBody;
                skillOnDamage.mainStateMachine = mainStateMachine;
                skillOnDamage.skillToExecute = skill;
            }
            return skillOnDamage;
        }

    }
}
