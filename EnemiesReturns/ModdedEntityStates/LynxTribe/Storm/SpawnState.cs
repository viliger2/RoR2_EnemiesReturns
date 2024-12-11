using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormCastTime.Value;
            base.OnEnter();
        }

    }
}
