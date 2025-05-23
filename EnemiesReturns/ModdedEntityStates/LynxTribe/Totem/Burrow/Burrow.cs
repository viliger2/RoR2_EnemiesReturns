﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem.Burrow
{
    [RegisterEntityState]
    public class Burrow : BaseState
    {
        public static float duration = 1.4f;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Body", "Burrow");
            Util.PlaySound("ER_Totem_Burrow_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextState(new Burrowed());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
