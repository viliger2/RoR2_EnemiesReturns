using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

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
