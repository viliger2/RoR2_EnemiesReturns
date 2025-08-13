using EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow;

using RoR2;
using RoR2.Skills;
using UnityEngine.Bindings;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class LynxTotemSkillDef : SkillDef
    {
        protected class InstanceData : BaseSkillInstanceData
        {
            public EntityStateMachine bodyStateMachine;
            public CharacterBody characterBody;
        }

        public override BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
        {
            return new InstanceData
            {
                bodyStateMachine = EntityStateMachine.FindByCustomName(skillSlot.gameObject, "Body"),
                characterBody = skillSlot.gameObject.GetComponent<CharacterBody>()
            };
        }

        private bool IsBurrowed(GenericSkill skillSlot)
        {
            return (((InstanceData)skillSlot.skillInstanceData).bodyStateMachine.state is Burrowed);
        }

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
