using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class Spawn : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 10f;
            base.OnEnter();
        }
    }
}
