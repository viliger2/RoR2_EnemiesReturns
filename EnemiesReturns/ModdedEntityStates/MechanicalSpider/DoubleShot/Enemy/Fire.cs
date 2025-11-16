using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Enemy
{
    [RegisterEntityState]
    internal class Fire : BaseFire
    {
        public override int baseNumberOfShots => Configuration.MechanicalSpider.DoubleShotShots.Value;

        public override string soundString => "ER_Spider_Fire_Play";

        public override float damageCoefficient => Configuration.MechanicalSpider.DoubleShotDamage.Value;

        public override float baseDelay => Configuration.MechanicalSpider.DoubleShotDelayBetween.Value;

        public override float baseDuration => 1f;

        public override EntityState GetNextCloseHatch()
        {
            return new CloseHatch();
        }

        public override EntityState GetNextFiringState()
        {
            return new ChargeFire();
        }
    }
}
