using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            spawnSoundString = "ER_Lynx_Storm_Spawn_Play";
            duration = 4f;
            base.OnEnter();
        }
    }
}
