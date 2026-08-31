using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.Secondary
{
    [RegisterEntityState]
    public class PrepareAttack : BasePrepareAttack
    {
        public override float baseDuration => 1.5f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SummonSkullsInit";

        public override EntityState GetNextState()
        {
            return new SkullsAttack();
        }
    }
}
