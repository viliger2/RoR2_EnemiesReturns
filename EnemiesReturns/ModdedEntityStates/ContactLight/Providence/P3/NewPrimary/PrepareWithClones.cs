using EnemiesReturns.ModdedEntityStates.ContactLight.Providence.BaseStates.BaseNewPrimary;
using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3.NewPrimary
{
    [RegisterEntityState]
    public class PrepareWithClones : BasePrepareAndFireSpawnClones
    {
        public override Pattern[] patterns => new Pattern[]
        {
            new Circle()
            {
                count = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneCircleCount.Value,
                radius = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneCircleRadius.Value,
            },
            new Cross()
            {
                pointsPerArm = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneCrossPointPerArm.Value,
                distance = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneCrossPointDistanceBetween.Value,
                rotationInternal = 0f
            },
            new Cross()
            {
                pointsPerArm = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneCrossPointPerArm.Value,
                distance = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneCrossPointDistanceBetween.Value,
                rotationInternal = 45f
            },
            new Triangle()
            {
                rowCount = Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneTriangleRowCount.Value,
                distanceBetweenPoints =  Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneTriangleDistanceBetweenPoints.Value,
                distanceBetweenRows =  Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneTriangleDistanceBetweenRows.Value
            }
        };

        public override float baseDuration => 3f;

        public override float baseStartProjectileDuration => 0.5f;

        public override float baseProjectileAnimationDuration => 2.2f;

        public override float damageCoefficient => Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneDamage.Value;

        public override float projectileLifetime => Configuration.ContactLight.Providence.ProvidencePhase3.P3LaserLifetime.Value;

        public override float searchDistance => Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneSearchDistance.Value;

        public override float searchAngle => Configuration.ContactLight.Providence.ProvidencePhase3.P3PrimaryCloneMaxSearchAngle.Value;

        public override EntityState GetNextState()
        {
            return new Swing1();
        }
    }
}
