using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.Judgement.SkyLaser
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public static float effectDeathDuration = 2f;

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

                var pillarLarge = childLocator.FindChild("PillarLarge");
                if (pillarLarge)
                {
                    var components = pillarLarge.GetComponents<ObjectScaleCurve>();
                    foreach (var component in components)
                    {
                        component.enabled = !component.enabled;
                    }
                }
                var pillarSmall = childLocator.FindChild("PillarSmall");
                if (pillarSmall)
                {
                    var components = pillarSmall.GetComponents<ObjectScaleCurve>();
                    foreach (var component in components)
                    {
                        component.enabled = !component.enabled;
                    }
                }

                var light = childLocator.FindChild("Light");
                if (light)
                {
                    var components = light.GetComponents<LightIntensityCurve>();
                    foreach (var component in components)
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
