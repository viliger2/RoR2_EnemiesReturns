using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            spawnSoundString = "";
            duration = 4f;
            base.OnEnter();
            EffectManager.SimpleEffect(spawnEffect, base.transform.position, Quaternion.identity, false);
        }
    }
}
