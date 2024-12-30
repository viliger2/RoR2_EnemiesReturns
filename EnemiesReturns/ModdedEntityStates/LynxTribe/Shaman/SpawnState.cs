using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            spawnSoundString = EnemiesReturns.Configuration.General.LynxVoices.Value ? "ER_Shaman_Spawn_Play" : "ER_Shaman_Spawn_No_Voice_Play"; // TODO
            duration = 1.2f;
            EffectManager.SimpleEffect(spawnEffect, transform.position, Quaternion.identity, false);
            base.OnEnter();
        }
    }
}
