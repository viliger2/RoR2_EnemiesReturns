using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    [RegisterEntityState]
    public class StompL : StompBase
    {
        internal override string animationStateName => "StompL";

        internal override string targetMuzzle => "StompSpawnL";

        internal override string hitBoxGroupName => "LeftStomp";
    }
}
