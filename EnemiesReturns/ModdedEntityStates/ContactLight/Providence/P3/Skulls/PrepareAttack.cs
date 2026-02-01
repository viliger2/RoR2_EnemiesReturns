using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Secondary
{
    [RegisterEntityState]
    public class PrepareAttack : BasePrepareAttack
    {
        public override float baseDuration => 3f;

        public override string layerName => "Gesture";

        public override string animationStateName => "Thundercall";

        public override EntityState GetNextState()
        {
            return new SkullsAttack();
        }
    }
}
