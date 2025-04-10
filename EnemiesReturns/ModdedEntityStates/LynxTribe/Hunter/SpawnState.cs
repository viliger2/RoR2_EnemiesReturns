using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Hunter
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            spawnSoundString = "ER_Hunter_Spawn_Play";
            duration = 1.2f;
            EffectManager.SimpleEffect(spawnEffect, transform.position, Quaternion.identity, false);
            base.OnEnter();
        }

    }
}
