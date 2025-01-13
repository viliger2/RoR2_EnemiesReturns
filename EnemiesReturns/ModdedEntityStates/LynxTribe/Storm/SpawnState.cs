using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Storm
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public override void OnEnter()
        {
            duration = EnemiesReturns.Configuration.LynxTribe.LynxShaman.SummonStormCastTime.Value;
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

    }
}
