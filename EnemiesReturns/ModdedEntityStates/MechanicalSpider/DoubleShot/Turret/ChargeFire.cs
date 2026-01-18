using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Turret
{
    [RegisterEntityState]
    public class ChargeFire : BaseChargeFire
    {
        public override string soundString => "ER_Spider_Fire_Charge_Drone_Play";

        public override EntityState GetNextState()
        {
            return new Fire();
        }
    }
}
