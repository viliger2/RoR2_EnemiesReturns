using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Scout
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            spawnSoundString = "ER_Scout_Spawn_Play";
            duration = 1.2f;
            EffectManager.SimpleEffect(spawnEffect, transform.position, Quaternion.identity, false);
            base.OnEnter();
        }

    }
}
