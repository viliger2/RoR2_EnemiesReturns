using EnemiesReturns.Reflection;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.ContactLight.Providence.P3
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject swordProp;

        public static float durationBeforeDissolving = 5f;

        public static float dissolveDuration = 15f;

        public static GameObject brotherDeathEffect;

        private bool dissolving;

        public override bool shouldAutoDestroy => base.fixedAge > durationBeforeDissolving + dissolveDuration + 1f;

        public override void OnEnter()
        {
            base.OnEnter();
            if (brotherDeathEffect)
            {
                EffectManager.SimpleMuzzleFlash(brotherDeathEffect, base.gameObject, "Chest", transmit: false);
            }
        }

        public override void PlayDeathAnimation(float crossfadeDuration = 0.1F)
        {
            //base.PlayDeathAnimation(crossfadeDuration);
            PlayAnimation("Body", "DeathP2");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= durationBeforeDissolving)
            {
                Dissolve();
            }
        }

        private void Dissolve()
        {
            if (!dissolving)
            {
                dissolving = true;
                Transform modelTransform = GetModelTransform();
                if ((bool)modelTransform)
                {
                    modelTransform.gameObject.GetComponent<CharacterModel>();
                    PrintController component = base.modelLocator.modelTransform.gameObject.GetComponent<PrintController>();
                    component.enabled = false;
                    component.printTime = dissolveDuration;
                    component.enabled = true;
                }

                var crown = FindModelChild("Crown");
                if (crown)
                {
                    var shaderAlpha = crown.gameObject.GetComponent<AnimateShaderAlpha>();
                    if (shaderAlpha)
                    {
                        shaderAlpha.enabled = true;
                    }
                }

                var swordRenderer = FindModelChild("SwordRenderer");
                if (swordRenderer)
                {
                    swordRenderer.gameObject.SetActive(false);
                }

                var swordLocation = FindModelChild("SwordSpawnMuzzle");
                if(swordLocation && swordProp)
                {
                    UnityEngine.Object.Instantiate(swordProp, swordLocation.position, swordLocation.rotation);
                }

                Transform transform = FindModelChild("TrueDeathEffect");
                if ((bool)transform)
                {
                    transform.gameObject.SetActive(value: true);
                    transform.GetComponent<ScaleParticleSystemDuration>().newDuration = dissolveDuration;
                }
            }
        }
    }
}
