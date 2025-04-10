using EnemiesReturns.Enemies.Colossus;
using EnemiesReturns.Reflection;
using EntityStates;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    [RegisterEntityState]
    public class InitialDeathState : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            var rockController = GetModelTransform().gameObject.GetComponent<FloatingRocksController>();

            if (rockController)
            {
                rockController.enabled = false;
            }

            if (NetworkServer.active)
            {
                var awooga = characterBody.gameObject.GetComponent<ColossusAwooga>();
                if (awooga && awooga.boing)
                {
                    outer.SetNextState(new DeathBoner());
                }
                else
                {
                    int value = UnityEngine.Random.Range(0, 100);
                    if (value > 50)
                    {
                        outer.SetNextState(new Death1());
                    }
                    else
                    {
                        outer.SetNextState(new Death2());
                    }
                }
            }
        }
    }
}
