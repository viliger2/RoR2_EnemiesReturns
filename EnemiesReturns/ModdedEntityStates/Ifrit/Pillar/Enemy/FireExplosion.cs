using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Enemy
{
    public class FireExplosion : BaseFireExplosion
    {
        public override float damage => EnemiesReturnsConfiguration.Ifrit.PillarExplosionDamage.Value;

        public override float radius => EnemiesReturnsConfiguration.Ifrit.PillarExplosionRadius.Value;

        public override float force => EnemiesReturnsConfiguration.Ifrit.PillarExplosionForce.Value;

        public override bool ignoresLoS => EnemiesReturnsConfiguration.Ifrit.PillarExplosionIgnoesLoS.Value;

        public override float damagePerStack => 0f;
    }
}
