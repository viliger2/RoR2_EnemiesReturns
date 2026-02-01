using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.Enemies.ContactLight.Providence
{
    [CreateAssetMenu(menuName = "EnemiesReturns/SkillDefs/StartOnCooldownSkillDef")]
    public class StartOnCooldownSkillDef : SkillDef
    {
        public bool startOnCooldown = true;

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            if (startOnCooldown)
            {
                skillSlot.stock = 0;
                skillSlot.rechargeStopwatch = 0;
            }
            return base.OnAssigned(skillSlot);
        }
    }
}
