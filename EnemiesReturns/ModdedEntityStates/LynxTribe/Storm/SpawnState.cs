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
            // TODO: write something with particles to indicate that storm is starting
            duration = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormCastTime.Value;
            base.OnEnter();
        }

    }
}
