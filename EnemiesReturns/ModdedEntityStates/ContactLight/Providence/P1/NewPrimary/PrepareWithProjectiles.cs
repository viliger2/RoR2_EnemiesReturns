using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1.NewPrimary
{
    [RegisterEntityState]
    public class PrepareWithProjectiles : BasePrepareAndFireProjectiles
    {
        public override float baseDuration => 3f;

        public override float baseStartProjectileDuration => 0.5f;

        public override float baseProjectileAnimationDuration => 2.2f;

        public override float baseProjectileDelay => Configuration.ContactLight.Providence.ProvidencePhase1.P1OrbsDelayBetween.Value;

        public override float damageCoefficient => Configuration.ContactLight.Providence.ProvidencePhase1.P1OrbsDamage.Value;

        public override string[] originChildNames => new string[]
        {
            "FirstProjectile",
            "SecondProjectile",
            "ThirdProjectile"
        };

        public override int projectileCount => Configuration.ContactLight.Providence.ProvidencePhase1.P1OrbsProjectileCount.Value;

        public override EntityState GetNextState()
        {
            return new Swing1();
        }
    }
}
