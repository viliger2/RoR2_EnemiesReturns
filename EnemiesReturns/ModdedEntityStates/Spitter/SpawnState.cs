using EntityStates;

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
