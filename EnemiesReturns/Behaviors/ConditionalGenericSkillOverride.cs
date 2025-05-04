using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Behaviors
{
    public class ConditionalGenericSkillOverride : MonoBehaviour
    {
        [Serializable]
        public struct ConditionalSkillInfo
        {
            public GenericSkill replacementSkill;

            public SkillSlot skillSlot;

            public GenericSkill originalSkill;
        }

        public SkillLocator skillLocator;

        public CharacterBody characterBody;

        public ConditionalSkillInfo[] conditionalSkillInfos;

        private bool wasSprinting;

        private void Start()
        {
            if (!skillLocator)
            {
                skillLocator = GetComponent<SkillLocator>();
            }
            if (!characterBody)
            {
                characterBody = GetComponent<CharacterBody>();
            }

            for(int i = 0; i < conditionalSkillInfos.Length; i++)
            {
                if (!conditionalSkillInfos[i].originalSkill)
                {
                    conditionalSkillInfos[i].originalSkill = GetGenericSkillBySlot(skillLocator, conditionalSkillInfos[i].skillSlot);
                }
            }
        }

        private void FixedUpdate()
        {
            bool wasSprinting = false;
            if (characterBody)
            {
                wasSprinting = characterBody.isSprinting;
            }

            for(int i = 0; i < conditionalSkillInfos.Length; i++)
            {
                if(wasSprinting && !this.wasSprinting)
                {
                    SetGenericSkillBySlot(skillLocator, conditionalSkillInfos[i].replacementSkill, conditionalSkillInfos[i].skillSlot);
                } else if(!wasSprinting && this.wasSprinting)
                {
                    SetGenericSkillBySlot(skillLocator, conditionalSkillInfos[i].originalSkill, conditionalSkillInfos[i].skillSlot);
                }
            }
            this.wasSprinting = wasSprinting;
        }

        private GenericSkill GetGenericSkillBySlot(SkillLocator skillLocator, SkillSlot slot)
        {
            if (!skillLocator)
            {
                return null;
            }

            if(slot == SkillSlot.Primary)
            {
                return skillLocator.primary;
            }

            if (slot == SkillSlot.Secondary)
            {
                return skillLocator.secondary;
            }

            if (slot == SkillSlot.Utility)
            {
                return skillLocator.utility;
            }

            if (slot == SkillSlot.Special)
            {
                return skillLocator.special;
            }

            return null;
        }

        private void SetGenericSkillBySlot(SkillLocator skillLocator, GenericSkill skill, SkillSlot slot) 
        {
            if (!skillLocator || !skill || slot == SkillSlot.None)
            {
                return;
            }

            if(slot == SkillSlot.Primary)
            {
                skillLocator.primary = skill;
            }

            if (slot == SkillSlot.Secondary)
            {
                skillLocator.secondary = skill;
            }

            if (slot == SkillSlot.Utility)
            {
                skillLocator.utility = skill;
            }

            if (slot == SkillSlot.Special)
            {
                skillLocator.special = skill;
            }
        }
    }
}
