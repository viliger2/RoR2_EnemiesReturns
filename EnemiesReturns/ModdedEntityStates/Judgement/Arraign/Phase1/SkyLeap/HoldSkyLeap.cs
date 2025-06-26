using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    [RegisterEntityState]
    public class HoldSkyLeap : BaseHoldSkyLeap
    {
        public override float baseDuration => 2.5f;

        public override float baseTargetMarked => 0.5f;

        public override float baseTargetDropped => 1.5f;

        public override void SetNextStateAuthority(Vector3 dropPosition)
        {
            outer.SetNextState(new ExitSkyLeap
            {
                dropPosition = dropPosition
            });
        }
    }
}
