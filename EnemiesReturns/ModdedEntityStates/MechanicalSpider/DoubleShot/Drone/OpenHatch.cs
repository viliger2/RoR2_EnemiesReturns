using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Drone
{
    [RegisterEntityState]
    public class OpenHatch : BaseOpenHatch
    {
        public override string openHatchSound => "";

        public override EntityState GetNextState()
        {
            return new ChargeFire();
        }
    }
}
