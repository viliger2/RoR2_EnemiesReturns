using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    [RegisterEntityState]
    public class InitialDeathState : BaseState
    {
        public static float spawnChancePerLoop => EnemiesReturns.Configuration.LynxTribe.LynxShaman.PostLoopTotemSummon.Value;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isAuthority)
            {
                if (Configuration.General.EnableLynxTotem.Value)
                {
                    var totalChance = spawnChancePerLoop * Run.instance.loopClearCount;
                    var roll = RoR2Application.rng.RangeFloat(0f, 100f);
                    if (roll < totalChance)
                    {
                        outer.SetNextState(new SummonTotemDeath());
                        return;
                    }
                }
                outer.SetNextState(new DeathState());
            }
        }
    }
}
