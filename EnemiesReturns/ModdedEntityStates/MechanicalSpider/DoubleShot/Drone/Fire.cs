using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Drone
{
    [RegisterEntityState]
    public class Fire : BaseFire
    {
        public override int baseNumberOfShots => Configuration.MechanicalSpider.DoubleShotShots.Value;

        public override string soundString => "ER_Spider_Fire_Drone_Play";

        public override float damageCoefficient => Configuration.MechanicalSpider.DoubleShotDamage.Value;

        public override float baseDelay => Configuration.MechanicalSpider.DoubleShotDelayBetween.Value;

        public override float baseDuration => 1f;

        public override EntityState GetNextCloseHatch()
        {
            return new CloseHatch();
        }

        public override EntityState GetNextFiringState()
        {
            return new ChargeFire();
        }
    }
}
