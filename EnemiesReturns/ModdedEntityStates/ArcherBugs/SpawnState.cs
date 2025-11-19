using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ArcherBugs
{
    [RegisterEntityState]
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
