﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Shaman
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            spawnSoundString = "ER_Shaman_Spawn_No_Voice_Play";
            duration = 1.2f;
            EffectManager.SimpleEffect(spawnEffect, transform.position, Quaternion.identity, false);
            base.OnEnter();
        }
    }
}
