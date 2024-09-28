using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject burrowPrefab;

        public override void OnEnter()
        {
            duration = 5f;
            spawnSoundString = "ER_Colossus_Spawn_Play";

            base.OnEnter();
            if (burrowPrefab)
            {
                EffectManager.SimpleMuzzleFlash(burrowPrefab, base.gameObject, "BurrowCenter", false);
            }
        }
    }
}
