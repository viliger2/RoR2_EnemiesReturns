using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow
{
    public class Burrowed : GenericCharacterMain
    {
        public static float duration = 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Body", "Burrowed", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority)
            {
                if (inputBank.moveVector.sqrMagnitude > 0.1f)
                //if (inputBank.moveVector.sqrMagnitude > 0.1f || (base.fixedAge >= duration && inputBank.skill3.down))
                {
                    outer.SetNextState(new Unburrow());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
