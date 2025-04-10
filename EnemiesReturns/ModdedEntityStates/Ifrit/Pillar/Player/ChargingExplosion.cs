using EnemiesReturns.Reflection;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar.Player
{
    [RegisterEntityState]
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
