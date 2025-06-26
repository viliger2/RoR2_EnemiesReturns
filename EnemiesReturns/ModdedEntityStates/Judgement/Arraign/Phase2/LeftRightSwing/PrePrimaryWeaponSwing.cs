using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.LeftRightSwings
{
    [RegisterEntityState]
    public class PrePrimaryWeaponSwing : BasePrePrimaryWeaponSwing
    {
        public override BasePrimaryWeaponSwing GetNextEntityState()
        {
            return new PrimaryWeaponSwing();
        }
    }
}
