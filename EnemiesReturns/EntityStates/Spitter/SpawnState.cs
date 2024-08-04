using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Spitter
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 2f;
            spawnSoundString = "ER_Spitter_Spawn_Play";

            base.OnEnter();
        }
    }
}
