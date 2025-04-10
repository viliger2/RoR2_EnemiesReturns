using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2.SkyLeap
{
    [RegisterEntityState]
    public class HoldSkyLeap : BaseHoldSkyLeap
    {
        public override float baseDuration => 1.5f;

        public override float baseTargetMarked => 0.75f;

        public override void SetNextStateAuthority(Vector3 dropPosition)
        {
            outer.SetNextState(new ExitSkyLeap
            {
                dropPosition = dropPosition
            });
        }
    }
}
