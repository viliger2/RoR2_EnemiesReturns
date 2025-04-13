using EnemiesReturns.ModdedEntityStates.Judgement.Arraign.BaseSkyLeap;
using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase1.SkyLeap
{
    [RegisterEntityState]
    public class HoldSkyLeap : BaseHoldSkyLeap
    {
        public override float baseDuration => 2.5f;

        public override float baseTargetMarked => 0.5f;

        public override float baseTargetDropped => 1f;

        public override void SetNextStateAuthority(Vector3 dropPosition)
        {
            outer.SetNextState(new ExitSkyLeap
            {
                dropPosition = dropPosition
            });
        }
    }
}
