using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    [RegisterEntityState]
    public class DeathInitial : BaseState
    {
        public static float spawnChance => EnemiesReturns.Configuration.MechanicalSpider.DroneSpawnChance.Value;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                var isVoidDeath = base.healthComponent && (base.healthComponent.killingDamageType & DamageType.VoidDeath) != 0;
                if (isVoidDeath)
                {
                    outer.SetNextState(new DeathNormal());
                    return;
                }
#if NOWEAVER || DEBUG
                outer.SetNextState(new DeathDrone());
                return;
#endif
                var chance = RoR2Application.rng.RangeFloat(0f, 100f);
                if (chance < spawnChance)
                {
                    outer.SetNextState(new DeathDrone());
                }
                else
                {
                    outer.SetNextState(new DeathNormal());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
