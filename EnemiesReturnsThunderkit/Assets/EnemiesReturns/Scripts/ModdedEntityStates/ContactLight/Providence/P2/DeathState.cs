using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine;
using EnemiesReturns.Reflection;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P2
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
        }

        public override void PlayDeathAnimation(float crossfadeDuration = 0.1F)
        {
            Animator modelAnimator = GetModelAnimator();
            if ((bool)modelAnimator)
            {
                modelAnimator.CrossFadeInFixedTime("DeathP2", crossfadeDuration);
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
