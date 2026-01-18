using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.SandCrab
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 1.25f;
            spawnSoundString = "ER_SandCrab_Spawn_Play";
            base.OnEnter();
        }
    }
}
