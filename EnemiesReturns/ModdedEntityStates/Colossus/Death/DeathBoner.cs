using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    [RegisterEntityState]
    public class DeathBoner : DeathFallBase
    {
        public override string fallAnimation => "DeathBoner";
    }
}
