using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Mission
{
    [RegisterEntityState]
    public class PrePhase15 : BaseState
    {
        public static float phaseDuration = 10f;

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fixedAge > phaseDuration && isAuthority)
            {
                outer.SetNextState(new Phase15());
            }
        }
    }
}
