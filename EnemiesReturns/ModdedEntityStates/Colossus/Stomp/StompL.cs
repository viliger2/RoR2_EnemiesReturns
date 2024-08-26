using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Stomp
{
    public class StompL : StompBase
    {
        internal override string animationStateName => "StompL";

        internal override string targetMuzzle => "StompSpawnL";

        internal override string hitBoxGroupName => "LeftStomp";
    }
}
