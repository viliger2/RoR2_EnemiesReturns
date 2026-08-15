using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseRings;
using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Special
{
    [RegisterEntityState]
    public class FireRings : BaseFireRings
    {
        public override int baseTimesToFire => 3;

        public override int baseRingToFire => 2;

        public override int additionalRingsMin => 1;

        public override int additionalRingsMax => 2;

        public override int additionalTimesMin => 1;

        public override int additionalTimesMax => 3;

        public override bool spawnShadowEffect => true;

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new OutroFireRings());
        }
    }
}
