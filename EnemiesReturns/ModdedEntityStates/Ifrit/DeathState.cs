using EnemiesReturns.Helpers;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Ifrit
{
    public class DeathState : GenericCharacterDeath
    {
        public static float effectDuration = 1.25f;

        public static float fallEffectTime = 1.75f;

        public static GameObject deathEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBruiserDeathImpact.prefab").WaitForCompletion();

        private Renderer maneRenderer;
        private Renderer bodyRenderer;

        private float initialEmission;

        private MaterialPropertyBlock manePropertyBlock;
        private MaterialPropertyBlock bodyPropertyBlock;

        private bool effectSpawned;

        private Transform modelTransform;

        public override void OnEnter()
        {
            bodyPreservationDuration = 2f;
            base.OnEnter();

            modelTransform = GetModelTransform();
            if (modelTransform && modelTransform.gameObject.TryGetComponent<TransformScaler>(out var transformScaler))
            {
                transformScaler.SetScaling(Vector3.zero, effectDuration);
            }

            var childLocator = GetModelChildLocator();

            var maneTransform = childLocator.FindChild("Mane");
            maneRenderer = maneTransform.GetComponent<Renderer>();
            manePropertyBlock = SetupPropertyBlock(maneRenderer, out initialEmission);

            var bodyTransform = childLocator.FindChild("MainBody");
            bodyRenderer = bodyTransform.GetComponent<Renderer>();
            bodyPropertyBlock = SetupPropertyBlock(bodyRenderer, out initialEmission);
        }

        public override void Update()
        {
            base.Update();
            if (age <= effectDuration)
            {
                SetPropertyBlock(maneRenderer, manePropertyBlock, initialEmission, age);
                SetPropertyBlock(bodyRenderer, bodyPropertyBlock, initialEmission, age);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(fixedAge >= fallEffectTime && !effectSpawned)
            {
                EffectManager.SpawnEffect(deathEffect, new EffectData { origin = modelTransform.position }, false);
                effectSpawned = true;
            }
        }

        private void SetPropertyBlock(Renderer renderer, MaterialPropertyBlock block, float initialEmission, float age)
        {
            block.SetFloat("_EmPower", Mathf.Lerp(initialEmission, 0f, age / effectDuration));
            renderer.SetPropertyBlock(block);
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
