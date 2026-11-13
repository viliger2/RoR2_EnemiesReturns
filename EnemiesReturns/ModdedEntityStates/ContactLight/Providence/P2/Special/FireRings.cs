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

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.Special
{
    [RegisterEntityState]
    public class FireRings : BaseFireRings
    {
        public override int baseTimesToFire => ProvidencePhase2.P2SpecialTimesToFire.Value;

        public override int baseRingToFire => ProvidencePhase2.P2SpecialRingsToFire.Value;

        public override int additionalRingsMin => ProvidencePhase2.P2SpecialAdditionalRingsMin.Value;

        public override int additionalRingsMax => ProvidencePhase2.P2SpecialAdditionalRingsMax.Value;

        public override int additionalTimesMin => ProvidencePhase2.P2SpecialAdditionalTimesMin.Value;

        public override int additionalTimesMax => ProvidencePhase2.P2SpecialAdditionalTimesMax.Value;

        public override bool spawnShadowEffect => true;

        public override float baseDamage => ProvidencePhase2.P2SpecialDamage.Value;

        public override float forceVectorValue => ProvidencePhase2.P2SpecialForce.Value;

        public override float healthModifiersMax => ProvidencePhase2.P2SpecialHealthModifiersMax.Value;

        public override float healthModifiersMin => ProvidencePhase2.P2SpecialHealthModifiersMin.Value;

        public override void SetNextStateAuthority()
        {
            outer.SetNextState(new OutroFireRings());
        }
    }
}
