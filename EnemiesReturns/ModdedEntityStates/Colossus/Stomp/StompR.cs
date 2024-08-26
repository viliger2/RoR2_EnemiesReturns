using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public class StompR : StompBase
    {
        internal override string animationStateName => "StompR";

        internal override string targetMuzzle => "StompSpawnR";

        internal override string hitBoxGroupName => "RightStomp";
    }
}
