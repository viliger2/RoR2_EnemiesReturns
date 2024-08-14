using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 5f;
            spawnSoundString = "ER_Colossus_Spawn_Play";

            base.OnEnter();
        }
    }
}
