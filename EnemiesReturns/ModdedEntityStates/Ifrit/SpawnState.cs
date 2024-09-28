using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 3f;
            spawnSoundString = "ER_Ifrit_Spawn_Play";

            base.OnEnter();
        }
    }
}
