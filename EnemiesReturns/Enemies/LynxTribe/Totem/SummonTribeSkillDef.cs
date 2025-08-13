using JetBrains.Annotations;
using RoR2;
using UnityEngine.Networking;

namespace EnemiesReturns.Enemies.LynxTribe.Totem
{
    public class SummonTribeSkillDef : LynxTotemSkillDef
    {
        public override bool IsReady([NotNull] GenericSkill skillSlot)
        {
            if (!IsCappedOnMinions(skillSlot))
            {
                return base.IsReady(skillSlot);
            }

            return false;
        }

        private bool IsCappedOnMinions(GenericSkill skillSlot)
        {
            var body = ((InstanceData)skillSlot.skillInstanceData).characterBody;
            if (body.hasEffectiveAuthority && NetworkServer.active) // this won't work on clients, so if someone wants to play as totem his minions won't be capped via this
            {
                return body.master.IsDeployableLimited(Enemies.LynxTribe.Totem.TotemStuff.SummonLynxTribeDeployable);
            }

            return false; // players are never capped
        }
    }
}
