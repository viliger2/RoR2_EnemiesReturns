using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class InitialDeathState : BaseState
    {
        public static float spawnChancePerLoop => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PostLoopTotemSummon.Value;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                var totalChance = spawnChancePerLoop * Run.instance.loopClearCount;
                var roll = RoR2Application.rng.RangeFloat(0f, 100f);
                if(roll < totalChance)
                {
                    outer.SetNextState(new SummonTotemDeath());
                } else
                {
                    outer.SetNextState(new DeathState());
                }
            }
        }
    }
}
