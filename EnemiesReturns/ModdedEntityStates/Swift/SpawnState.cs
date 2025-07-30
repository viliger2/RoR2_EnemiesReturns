using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 2f;
            spawnSoundString = "ER_Swift_Spawn_Play";

            base.OnEnter();
        }
    }
}
