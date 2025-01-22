using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
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
            Util.PlaySound("", base.gameObject); // TODO
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
