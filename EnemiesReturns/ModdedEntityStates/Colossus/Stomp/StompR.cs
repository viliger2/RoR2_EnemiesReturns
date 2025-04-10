using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    [RegisterEntityState]
    public class StompR : StompBase
    {
        internal override string animationStateName => "StompR";

        internal override string targetMuzzle => "StompSpawnR";

        internal override string hitBoxGroupName => "RightStomp";
    }
}
