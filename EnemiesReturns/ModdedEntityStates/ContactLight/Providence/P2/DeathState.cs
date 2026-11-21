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
            var origin = FindModelChild("PortalClonesEffectOrigin");
            if (origin && origin.childCount > 0)
            {
                // a bit dirty but that's effects for you
                UnityEngine.Object.Destroy(origin.GetChild(0).gameObject);
            }
            base.OnEnter();
            ForceUpdateOverlays();
        }

        private void ForceUpdateOverlays()
        {
            if (cachedModelTransform)
            {
                var model = cachedModelTransform.gameObject.GetComponent<CharacterModel>();
                if (model)
                {
                    model.forceUpdate = true;
                }
            }
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
                if (isAuthority)
                {
                    var data = new EffectData()
                    {
                        origin = FindModelChild("TeleportEffectOrigin").position,
                        rotation = Quaternion.identity,
                    };
                    EffectManager.SpawnEffect(teleportEffect, data, true);
                }
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
