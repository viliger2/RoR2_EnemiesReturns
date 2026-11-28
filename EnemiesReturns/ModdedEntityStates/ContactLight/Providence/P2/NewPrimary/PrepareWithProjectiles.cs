using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2.NewPrimary
{
    [RegisterEntityState]
    public class PrepareWithProjectiles : BasePrepareAndFireProjectiles
    {
        public override float baseDuration => 3f;

        public override float baseStartProjectileDuration => 0.5f;

        public override float baseProjectileAnimationDuration => 2.2f;

        public override float baseProjectileDelay => Configuration.ContactLight.Providence.ProvidencePhase2.P2OrbsDelayBetween.Value;

        public override float damageCoefficient => Configuration.ContactLight.Providence.ProvidencePhase2.P2OrbsDamage.Value;

        public override string[] originChildNames => new string[]
        {
            "FirstProjectile",
            "SecondProjectile",
            "ThirdProjectile",
            "FourthProjectile",
            "FifthProjectile"
        };

        public override int projectileCount => Configuration.ContactLight.Providence.ProvidencePhase2.P2OrbsProjectileCount.Value;

        public override EntityState GetNextState()
        {
            return new Swing1();
        }
    }
}
