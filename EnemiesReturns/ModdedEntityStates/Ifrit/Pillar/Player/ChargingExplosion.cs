namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Player
{
    public class ChargingExplosion : BaseChargingExplosion
    {
        public override float duration => EnemiesReturnsConfiguration.Ifrit.SpawnPillarOnChampionKillChargeTime.Value;

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
