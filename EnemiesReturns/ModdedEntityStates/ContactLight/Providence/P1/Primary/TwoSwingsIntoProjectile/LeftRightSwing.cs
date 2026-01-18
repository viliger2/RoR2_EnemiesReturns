using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseTwoSwingsIntoProjectile;
using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary.TwoSwingsIntoProjectile
{
    [RegisterEntityState]
    public class LeftRightSwing : BaseLeftRightSwing
    {
        public override float baseFirstSwing => 1.5f;

        public override float baseSecondSwing => 1.5f;

        public override float damageCoefficient => 2f;

        public override string layerName => "UpperBodyOnly";

        public override string firstSwingStateName => "Slash1";

        public override string secondSwingStateName => "Slash2";

        public override string playbackParam => "combo.playbackRate";

        public override string hitboxName => "Sword";

        public override string firstAttackParam => "Slash1.attack";

        public override string secondAttackParam => "Slash2.attack";

        public override EntityState GetNextState()
        {
            return new FireProjectiles();
        }
    }
}
