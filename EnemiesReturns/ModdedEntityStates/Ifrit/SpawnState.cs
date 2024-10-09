using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject spawnEffect;

        public override void OnEnter()
        {
            duration = 3f;
            spawnSoundString = "ER_Ifrit_Spawn_Play";
            //EffectManager.SimpleMuzzleFlash(spawnEffect, base.gameObject, "Center", transmit: false);
            EffectManager.SpawnEffect(spawnEffect, new EffectData {origin = base.transform.position}, false);
            base.OnEnter();
        }
    }
}
