using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Dash
{
    public class DashStart : BaseState
    {
        public static float duration = 0.3f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayCrossfade("Body", "DashStart", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(isAuthority && fixedAge >= duration)
            {
                outer.SetNextState(new Dash());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
