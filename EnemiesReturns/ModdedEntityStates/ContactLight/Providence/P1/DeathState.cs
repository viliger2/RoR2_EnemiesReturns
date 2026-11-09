using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P1
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public static float deathDelay = 3f;

        public static GameObject teleportEffect;

        public static GameObject brotherDeathEffect;

        private bool hasDied;

        public override void OnEnter()
        {
            bodyPreservationDuration = deathDelay;
            if (brotherDeathEffect)
            {
                EffectManager.SimpleMuzzleFlash(brotherDeathEffect, base.gameObject, "Chest", transmit: false);
            }
            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }

            if (base.fixedAge > deathDelay && !hasDied)
            {
                hasDied = true;
                var data = new EffectData()
                {
                    origin = FindModelChild("TeleportEffectOrigin").position,
                    rotation = Quaternion.identity,
                };
                EffectManager.SpawnEffect(teleportEffect, data, false);
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyBodyAsapServer();
                }
            }
        }

        public override void OnExit()
        {
            base.DestroyModel();
            base.OnExit();
        }
    }
}
