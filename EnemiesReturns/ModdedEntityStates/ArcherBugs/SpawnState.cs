using EntityStates;

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
