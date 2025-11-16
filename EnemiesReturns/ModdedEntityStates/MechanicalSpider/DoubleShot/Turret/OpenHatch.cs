using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Turret
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
