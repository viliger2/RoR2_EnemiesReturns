using EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow;
using EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class LynxTotemSkillDef : SkillDef
    {
        protected class InstanceData : BaseSkillInstanceData
        {
            public EntityStateMachine bodyStateMachine;
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData
            {
                bodyStateMachine = EntityStateMachine.FindByCustomName(skillSlot.gameObject, "Body")
            };
        }

        private bool IsBurrowed(GenericSkill skillSlot)
        {
            return (((InstanceData)skillSlot.skillInstanceData).bodyStateMachine.state is Burrowed);
        }

        //public override bool CanExecute([NotNull] GenericSkill skillSlot)
        //{
        //    if (IsBurrowed(skillSlot))
        //    {
        //        return base.CanExecute(skillSlot);
        //    }

        //    return false;
        //}

        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            if (IsBurrowed(skillSlot))
            {
                return base.IsReady(skillSlot);
            }

            return false;
        }
    }
}
