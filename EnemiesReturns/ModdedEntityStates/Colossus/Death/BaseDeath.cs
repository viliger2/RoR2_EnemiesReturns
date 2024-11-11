using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Colossus.Death
{
    public abstract class BaseDeath : GenericCharacterDeath
    {
        public abstract float duration { get; }

        public abstract float fallEffectSpawnTime { get; }

        public abstract string fallEffectChild { get; }

        public static GameObject fallEffect;

        public Transform fallTransform;

        private Renderer eyeRenderer;

        private MaterialPropertyBlock eyePropertyBlock;

        private float initialEmmision;

        private Light headLight;

        private float initialRange;

        private bool fallEffectSpawned;

        public override void OnEnter()
        {
            bodyPreservationDuration = 5f; // just to be sure
            base.OnEnter();

            if (isVoidDeath)
            {
                return;
            }

            var childLocator = GetModelChildLocator();

            eyeRenderer = childLocator.FindChildComponent<Renderer>("EyeModel");
            eyePropertyBlock = new MaterialPropertyBlock();
            initialEmmision = eyeRenderer.material.GetFloat("_EmPower");
            eyePropertyBlock.SetFloat("_EmPower", initialEmmision);
            eyeRenderer.SetPropertyBlock(eyePropertyBlock);

            fallTransform = childLocator.FindChild(fallEffectChild);

            headLight = childLocator.FindChildComponent<Light>("HeadLight");
            initialRange = headLight.range;

            var stoneParticles = childLocator.FindChild("StoneParticles");
            if (stoneParticles)
            {
                stoneParticles.gameObject.SetActive(false);
            }

            var spotlight = childLocator.FindChild("LaserChargeSpotlight");
            if (spotlight)
            {
                spotlight.gameObject.SetActive(false);
            }
        }

        public override void Update()
        {
            base.Update();
            if (isVoidDeath)
            {
                return;
            }

            if (age <= duration)
            {
                eyePropertyBlock.SetFloat("_EmPower", Mathf.Lerp(initialEmmision, 0f, age / duration));
                eyeRenderer.SetPropertyBlock(eyePropertyBlock);
                headLight.range = Mathf.Lerp(initialRange, 0f, age / duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }

            if (fixedAge >= fallEffectSpawnTime && !fallEffectSpawned)
            {
                EffectManager.SpawnEffect(fallEffect, new EffectData { origin = fallTransform.position }, true);
                fallEffectSpawned = true;
            }
        }
    }
}
