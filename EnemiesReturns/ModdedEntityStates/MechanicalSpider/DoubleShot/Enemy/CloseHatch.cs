using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Enemy
{
    [RegisterEntityState]
    public class CloseHatch : BaseCloseHatch
    {
        public override string closeHatchSound => "ER_Spider_Hatch_Close_Play";
    }
}
