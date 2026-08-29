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

        public static GameObject teleportEffect = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Common_VFX.TeleportOutBoom_prefab).WaitForCompletion();

        private bool hasDied;

        public override void OnEnter()
        {
            bodyPreservationDuration = deathDelay;
            base.OnEnter();

            if (isVoidDeath)
            {
                return;
            }
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
                EffectManager.SimpleImpactEffect(teleportEffect, base.characterBody.corePosition, Vector3.up, false);
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
