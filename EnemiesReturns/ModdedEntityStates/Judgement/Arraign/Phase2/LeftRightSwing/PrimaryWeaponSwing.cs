using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.LeftRightSwings
{
    [RegisterEntityState]
    public class PrimaryWeaponSwing : BasePrimaryWeaponSwing
    {
        public override string swingSoundEffect => "ER_Arraign_LeftRightSwingP2_Play";
    }
}
