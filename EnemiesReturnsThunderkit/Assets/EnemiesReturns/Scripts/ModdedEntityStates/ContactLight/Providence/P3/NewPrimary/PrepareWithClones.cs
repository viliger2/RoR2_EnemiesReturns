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
                count = 8,
                radius = 11.5f,
            },
            new Cross()
            {
                pointsPerArm = 2,
                distance = 8.5f,
                rotationInternal = 0f
            },
            new Cross()
            {
                pointsPerArm = 2,
                distance = 8.5f,
                rotationInternal = 45f
            },
            new Triangle()
            {
                rowCount = 3,
                distanceBetweenPoints = 8f,
                distanceBetweenRows = 6.5f
            }
        };

        public override float baseDuration => 3f;

        public override float baseStartProjectileDuration => 0.5f;

        public override float baseProjectileAnimationDuration => 2.2f;

        public override float damageCoefficient => 5f;

        public override float projectileLifetime => 2f;

        public override EntityState GetNextState()
        {
            //return new Swing1();
            return null;
        }
    }
}
