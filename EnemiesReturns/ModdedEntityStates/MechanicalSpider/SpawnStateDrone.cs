using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider
{
    public class SpawnStateDrone : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = 0.1f;
            spawnSoundString = "Play_drone_repair";

            base.OnEnter();
        }
    }
}
