using EnemiesReturns.Configuration;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Player
{
    public class ChargingExplosion : BaseChargingExplosion
    {
        public override float duration => EnemiesReturns.Configuration.Ifrit.SpawnPillarOnChampionKillChargeTime.Value;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && base.isAuthority)
            {
                outer.SetNextState(new Player.FireExplosion());
            }
        }
    }
}
