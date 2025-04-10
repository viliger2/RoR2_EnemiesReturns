using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            spawnSoundString = "";
            duration = 4f;
            base.OnEnter();
        }
    }
}
