using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 1.3f;
            spawnSoundString = "ER_ArcherBug_Spawn_Play";
            base.OnEnter();
        }
    }
}
