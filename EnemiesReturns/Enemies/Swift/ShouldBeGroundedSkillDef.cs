using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Bindings;

namespace EnemiesReturns.Enemies.Swift
{
    public class ShouldBeGroundedSkillDef : SkillDef
    {
        public bool shouldBeGrounded;

        protected class InstanceData : BaseSkillInstanceData
        {
            public CharacterMotor characterMotor;
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData
            {
                characterMotor = skillSlot.gameObject.GetComponent<CharacterMotor>()
            };
        }

        private bool ShouldBeGrounded(GenericSkill skill)
        {
            return ((InstanceData)skill.skillInstanceData).characterMotor.isGrounded == shouldBeGrounded;
        }

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            if (ShouldBeGrounded(skillSlot))
            {
                return base.IsReady(skillSlot);
            }

            return false;
        }
    }
}
