using EnemiesReturns.Reflection;
using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.DoubleShot.Turret
{
    [RegisterEntityState]
    public class Fire : BaseFire
    {
        public override int baseNumberOfShots => GetNumberOfShots();

        public override string soundString => "ER_Spider_Fire_Drone_Play";

        public override float damageCoefficient => Configuration.MechanicalSpider.TurretDoubleShotDamage.Value;

        public override float baseDelay => GetBaseDelay();

        public override float baseDuration => GetBaseDuration();

        private int GetNumberOfShots()
        {
            int baseNumber = Configuration.MechanicalSpider.DoubleShotShots.Value;
            if (characterBody.inventory.GetItemCountPermanent(Content.Items.MechanicalSpiderTurretScepterHelper) > 0)
            {
                baseNumber *= Configuration.MechanicalSpider.ScepterTurretProjectileMultiplier.Value;
            }

            return baseNumber;
        }

        private float GetBaseDelay()
        {
            float baseDelay = Configuration.MechanicalSpider.DoubleShotDelayBetween.Value;
            if (characterBody.inventory.GetItemCountPermanent(Content.Items.MechanicalSpiderTurretScepterHelper) > 0)
            {
                baseDelay /= Configuration.MechanicalSpider.ScepterTurretAttackSpeedMultiplier.Value;
            }

            return baseDelay;
        }
        
        private float GetBaseDuration()
        {
            float baseDuration = 1f;
            if (characterBody.inventory.GetItemCountPermanent(Content.Items.MechanicalSpiderTurretScepterHelper) > 0)
            {
                baseDuration /= Configuration.MechanicalSpider.ScepterTurretAttackSpeedMultiplier.Value;
            }

            return baseDuration;
        }


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
