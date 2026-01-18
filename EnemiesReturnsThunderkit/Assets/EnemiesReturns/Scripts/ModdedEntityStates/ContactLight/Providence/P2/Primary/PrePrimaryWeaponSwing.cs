using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseProjectilePrimary;
using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Primary
{
    [RegisterEntityState]
    public class PrePrimaryWeaponSwing : BasePrePrimaryWeaponSwing
    {
        public override BasePrimaryWeaponSwing GetNextEntityState()
        {
            return new ProjectileSwingsWithClones();
        }
    }
}
