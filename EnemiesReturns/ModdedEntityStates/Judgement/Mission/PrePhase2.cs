using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class PrePhase2 : BaseState
    {
        public static float phaseDuration = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > phaseDuration && isAuthority)
            {
                outer.SetNextState(new Phase2());
            }
        }
    }
}
