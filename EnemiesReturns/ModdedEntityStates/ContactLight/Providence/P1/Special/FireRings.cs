using EnemiesReturns.Configuration.ContactLight.Providence;
using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseRings;
using EnemiesReturns.Projectiles;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.Special
{
    [RegisterEntityState]
    public class FireRings : BaseFireRings
    {
        public override int baseTimesToFire => ProvidencePhase1.P1SpecialTimesToFire.Value;

        public override int baseRingToFire => ProvidencePhase1.P1SpecialRingsToFire.Value;

        public override int additionalRingsMin => ProvidencePhase1.P1SpecialAdditionalRingsMin.Value;

        public override int additionalRingsMax => ProvidencePhase1.P1SpecialAdditionalRingsMax.Value;

        public override int additionalTimesMin => ProvidencePhase1.P1SpecialAdditionalTimesMin.Value;

        public override int additionalTimesMax => ProvidencePhase1.P1SpecialAdditionalTimesMax.Value;

        public override bool spawnShadowEffect => false;

        public override float baseDamage => ProvidencePhase1.P1SpecialDamage.Value;

        public override float forceVectorValue => ProvidencePhase1.P1SpecialForce.Value;

        public override float healthModifiersMax => ProvidencePhase1.P1SpecialHealthModifiersMax.Value;

        public override float healthModifiersMin => ProvidencePhase1.P1SpecialHealthModifiersMin.Value;

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new OutroFireRings());
        }
    }
}
