using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    public class SpawnState : GenericCharacterSpawnState
    {
        public static GameObject leavesSpawnEffect;

        public static GameObject debrisSpawnEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/LemurianBruiserSpawnEffect.prefab").WaitForCompletion();

        public static GameObject poundEffect;

        public static float debrisSpawnDuration = 0.208f;

        public static float poundSpawnDuration = 1.125f;

        private bool debrisSpawned;

        private bool poundSpawned;

        public override void OnEnter()
        {
            spawnSoundString = "ER_Totem_Spawn_Play";
            duration = 2f;
            EffectManager.SimpleEffect(leavesSpawnEffect, transform.position, Quaternion.identity, false);
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge > debrisSpawnDuration && !debrisSpawned)
            {
                EffectManager.SimpleEffect(debrisSpawnEffect, transform.position, Quaternion.identity, false);
                debrisSpawned = true;
            }
            if(fixedAge > poundSpawnDuration && !poundSpawned)
            {
                EffectManager.SimpleEffect(poundEffect, transform.position, Quaternion.identity, false);
                poundSpawned = true;
            }
        }

    }
}
