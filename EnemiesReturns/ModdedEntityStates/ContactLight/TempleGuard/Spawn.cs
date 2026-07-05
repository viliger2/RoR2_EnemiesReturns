using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.TempleGuard
{
    [RegisterEntityState]
    public class Spawn : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 2.5f;
            spawnSoundString = "ER_TempleGuard_Spawn_Play";
            base.OnEnter();
        }
    }
}
