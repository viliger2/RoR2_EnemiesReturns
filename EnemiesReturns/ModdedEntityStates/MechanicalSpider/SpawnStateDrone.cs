using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    public class SpawnStateDrone : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 0.1f;

            base.OnEnter();
        }
    }
}
