using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 1.667f;
            spawnSoundString = "ER_Spider_Spawn_Play";

            base.OnEnter();
        }
    }
}
