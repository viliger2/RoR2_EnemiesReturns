﻿using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    [RegisterEntityState]
    public class SpawnStateFromShaman : BaseState
    {
        public static float duration = 3.8f;

        public static GameObject shakeEffect;

        public static GameObject poundEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LemurianBruiser/LemurianBruiserSpawnEffect.prefab").WaitForCompletion();

        public static float debrisSpawnDuration = 1.5f;

        public static float poundSpawnDuration = 2.916f;

        private bool debrisSpawned;

        private bool poundSpawned;

        private Transform effectOrigin;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("ER_Totem_ShamanSpawn_Play", base.gameObject);
            PlayAnimation("Body", "SpawnFromShaman");
            effectOrigin = FindModelChild("SpawnFromShamanEffectOrigin");
            if (!effectOrigin)
            {
                effectOrigin = transform;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge > debrisSpawnDuration && !debrisSpawned)
            {
                EffectManager.SimpleEffect(shakeEffect, effectOrigin.position, Quaternion.identity, false);
                debrisSpawned = true;
            }
            if (fixedAge > poundSpawnDuration && !poundSpawned)
            {
                EffectManager.SimpleEffect(poundEffect, effectOrigin.position, Quaternion.identity, false);
                poundSpawned = true;
            }
            if (fixedAge > duration && isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
