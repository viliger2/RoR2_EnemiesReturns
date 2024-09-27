using System;
using System.Collections.Generic;
using System.Text;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Player
{
    public class FireExplosion : BaseFireExplosion
    {
        public override float damage => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillDamage.Value;

        public override float radius => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillRadius.Value;

        public override float force => EnemiesReturnsConfiguration.Ifrit.PillarExplosionForce.Value;

        public override bool ignoresLoS => EnemiesReturnsConfiguration.Ifrit.PillarExplosionIgnoesLoS.Value;

        public override float damagePerStack => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillDamagePerStack.Value;
    }
}
