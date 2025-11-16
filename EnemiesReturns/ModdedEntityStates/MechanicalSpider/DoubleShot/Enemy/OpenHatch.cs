using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Enemy
{
    [RegisterEntityState]
    public class OpenHatch : BaseOpenHatch
    {

        public override string openHatchSound => "ER_Spider_Hatch_Open_Play";

        public override EntityState GetNextState()
        {
            return new ChargeFire();
        }
    }
}
