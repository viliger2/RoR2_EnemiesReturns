using EnemiesReturns.Behaviors;
using EntityStates;
using RoR2;
using UnityEngine;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class DeathState : GenericCharacterDeath
    {
        public static float effectDuration = 1.25f;

        public static float fallEffectTime = 1.75f;

        public static GameObject deathEffect;

        private Renderer maneRenderer;
        private Renderer bodyRenderer;

        private float initialEmission;

        private MaterialPropertyBlock manePropertyBlock;
        private MaterialPropertyBlock bodyPropertyBlock;

        private bool effectSpawned;

        private Transform effectSpawnTransform;

        private Transform modelTransform;

        public override void OnEnter()
        {
            bodyPreservationDuration = 2f;
            base.OnEnter();

            if (isVoidDeath)
            {
                return;
            }

            modelTransform = GetModelTransform();
            if (modelTransform && modelTransform.gameObject.TryGetComponent<TransformScaler>(out var transformScaler))
            {
                transformScaler.SetScaling(Vector3.zero, effectDuration);
            }

            var childLocator = GetModelChildLocator();

            var maneTransform = childLocator.FindChild("Mane");
            maneRenderer = maneTransform.GetComponent<Renderer>();
            if (maneRenderer)
            {
                manePropertyBlock = SetupPropertyBlock(maneRenderer, out initialEmission);
            }

            effectSpawnTransform = childLocator.FindChild("Chest");

            var bodyTransform = childLocator.FindChild("MainBody");
            bodyRenderer = bodyTransform.GetComponent<Renderer>();
            if (bodyRenderer)
            {
                bodyPropertyBlock = SetupPropertyBlock(bodyRenderer, out initialEmission);
            }
        }

        public override void Update()
        {
            base.Update();
            if (isVoidDeath)
            {
                return;
            }

            if (age <= effectDuration)
            {
                SetPropertyBlock(maneRenderer, manePropertyBlock, initialEmission, age);
                SetPropertyBlock(bodyRenderer, bodyPropertyBlock, initialEmission, age);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }
            if (fixedAge >= fallEffectTime && !effectSpawned && effectSpawnTransform && deathEffect)
            {
                EffectManager.SpawnEffect(deathEffect, new EffectData { origin = modelTransform.position, scale = 3.0f }, false);
                effectSpawned = true;
            }
        }

        private void SetPropertyBlock(Renderer renderer, MaterialPropertyBlock block, float initialEmission, float age)
        {
            if (renderer && block != null)
            {
                block.SetFloat("_EmPower", Mathf.Lerp(initialEmission, 0f, age / effectDuration));
                renderer.SetPropertyBlock(block);
            }
        }

        private MaterialPropertyBlock SetupPropertyBlock(Renderer renderer, out float initialEmission)
        {
            var propertyBlock = new MaterialPropertyBlock();
            initialEmission = renderer.material.GetFloat("_EmPower");
            propertyBlock.SetFloat("_EmPower", initialEmission);
            renderer.SetPropertyBlock(propertyBlock);

            return propertyBlock;
        }
    }
}
