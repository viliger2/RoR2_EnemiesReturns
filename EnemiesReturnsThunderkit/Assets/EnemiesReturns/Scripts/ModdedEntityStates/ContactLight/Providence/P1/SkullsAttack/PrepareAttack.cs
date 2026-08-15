using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack
{
    [RegisterEntityState]
    public class PrepareAttack : BasePrepareAttack
    {
        public override float baseDuration => 3.75f;

        public override string layerName => "Gesture, Override";

        public override string animationStateName => "SummonSkulls";

        public override EntityState GetNextState()
        {
            return new SkullsAttack();
        }
    }
}
