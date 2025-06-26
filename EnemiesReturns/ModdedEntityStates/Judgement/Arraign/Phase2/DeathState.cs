using EnemiesReturns.Reflection;
using EntityStates;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            bodyPreservationDuration = 20f;
            base.OnEnter();
        }
    }
}
