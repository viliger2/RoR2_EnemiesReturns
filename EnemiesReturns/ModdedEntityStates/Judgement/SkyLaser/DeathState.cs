using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using EnemiesReturns.Reflection;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public static float effectDeathDuration = 2f;

        private Transform pillarLarge;

        private Transform pillarSmall;

        private Vector3 initialPillarLargeScale;

        private Vector3 initialPillarSmallScale;

        private bool bodyMarkedForDestruction = false;

        public override void OnEnter()
        {
            bodyPreservationDuration = 3f;
            base.OnEnter();
            if (isVoidDeath)
            {
                return;
            }

            var childLocator = GetModelChildLocator();
            if (childLocator)
            {
                var particles = childLocator.FindChild("Particles");
                if (particles && particles.TryGetComponent<ParticleSystem>(out var particleSystem))
                {
                    particleSystem.Stop();
                }

                var dustClouds = childLocator.FindChild("DustClouds");
                if (dustClouds && dustClouds.TryGetComponent<ParticleSystem>(out var particleSystem2))
                {
                    particleSystem2.Stop();
                }

                pillarLarge = childLocator.FindChild("PillarLarge");
                if (pillarLarge)
                {
                    initialPillarLargeScale = pillarLarge.transform.localScale;
                    if(pillarLarge.TryGetComponent<ObjectScaleCurve>(out var scaleCurve))
                    {
                        scaleCurve.enabled = false;
                    }
                }
                pillarSmall = childLocator.FindChild("PillarSmall");
                if (pillarSmall)
                {
                    initialPillarSmallScale = pillarSmall.transform.localScale;
                    if (pillarSmall.TryGetComponent<ObjectScaleCurve>(out var scaleCurve))
                    {
                        scaleCurve.enabled = false;
                    }
                }

                var light = childLocator.FindChild("Light");
                if (light)
                {
                    var components = light.GetComponents<LightIntensityCurve>();
                    foreach(var component in components)
                    {
                        component.enabled = !component.enabled;
                    }
                }

                var postProccess = childLocator.FindChild("PostProccess");
                if (postProccess)
                {
                    var component = postProccess.GetComponent<PostProcessDuration>();
                    if (component)
                    {
                        component.enabled = true;
                        component.maxDuration = effectDeathDuration;
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
            if (isVoidDeath)
            {
                return;
            }

            if (!bodyMarkedForDestruction)
            {
                if (pillarLarge)
                {
                    pillarLarge.localScale = Vector3.Lerp(initialPillarLargeScale, Vector3.zero, age / effectDeathDuration);
                }
                if (pillarSmall)
                {
                    pillarSmall.localScale = Vector3.Lerp(initialPillarSmallScale, Vector3.zero, age / effectDeathDuration);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }
            if (!bodyMarkedForDestruction && fixedAge > effectDeathDuration)
            {
                bodyMarkedForDestruction = true;
                DestroyModel();
                if (NetworkServer.active)
                {
                    DestroyMaster();
                    DestroyBody();
                }
            }
        }

        private void DestroyBody()
        {
            if (base.gameObject)
            {
                NetworkServer.Destroy(base.gameObject);
            }
        }

        private void DestroyMaster()
        {
            if (base.characterBody && base.characterBody.master)
            {
                NetworkServer.Destroy(base.characterBody.masterObject);
            }
        }
    }
}
