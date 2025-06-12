using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.LeftRightSwings
{
    [RegisterEntityState]
    public class PrimaryWeaponSwing : BasePrimaryWeaponSwing
    {
        public override string swingSoundEffect => "ER_Arraign_LeftRightSwingP1_Play";
    }
}
