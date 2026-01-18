using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseSkulls;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.SkullsAttack
{
    public class PrepareAttack : BasePrepareAttack
    {
        public override float baseDuration => 3f;

        public override string layerName => "Gesture";

        public override string animationStateName => "Thundercall";

        public override string nextStateESMName => "Skulls";

        public override EntityState GetNextState()
        {
            return new SkullsAttack();
        }
    }
}
