using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseProjectilePrimary;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Primary
{
    [RegisterEntityState]
    public class PrePrimaryWeaponSwing : BasePrePrimaryWeaponSwing
    {
        public override BasePrimaryWeaponSwing GetNextEntityState()
        {
            return new ProjectileSwings();
        }
    }
}
