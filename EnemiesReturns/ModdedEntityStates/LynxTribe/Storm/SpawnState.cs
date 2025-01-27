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
            spawnSoundString = "ER_Lynx_Storm_Spawn_Play";
            duration = 4f;
            base.OnEnter();
            if (NetworkServer.active)
            {
                characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }
        }

    }
}
