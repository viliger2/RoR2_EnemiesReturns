using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit.Pillar
{
    [RegisterEntityState]
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject burrowPrefab;

        public override void OnEnter()
        {
            duration = 2f;
            spawnSoundString = "ER_Ifrit_Pillar_Spawn_Play";

            base.OnEnter();
            if (burrowPrefab)
            {
                EffectManager.SimpleMuzzleFlash(burrowPrefab, base.gameObject, "SpawnEffectOrigin", false);
            }
        }
    }
}
