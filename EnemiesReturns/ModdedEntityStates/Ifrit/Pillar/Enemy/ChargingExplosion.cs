using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Enemy
{
    public class ChargingExplosion : BaseChargingExplosion
    {
        public override float duration => EnemiesReturnsConfiguration.Ifrit.PillarExplosionChargeDuration.Value;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextState(new Enemy.FireExplosion());
            }
        }
    }
}
