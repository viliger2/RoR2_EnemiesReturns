using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    // TODO: disable smoke and sparks on death, people think they are interactable
    public class DeathNormal : GenericCharacterDeath
    {
        public static GameObject smokeBombPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();

        public static float effectTime = 0.54f;

        private bool effectSpawned = false;

        private Renderer modelRenderer;

        private MaterialPropertyBlock propertyBlock;

        private float initialEmission;

        public override void OnEnter()
        {
            bodyPreservationDuration = 1f;
            base.OnEnter();
            Util.PlaySound("ER_Spider_Death_Normal_Play", base.gameObject);

            var childLocator = GetModelChildLocator();
            modelRenderer = childLocator.FindChildComponent<Renderer>("Model");
            if (modelRenderer)
            {
                propertyBlock = new MaterialPropertyBlock();
                initialEmission = modelRenderer.material.GetFloat("_EmPower");
                propertyBlock.SetFloat("_EmPower", initialEmission);
                modelRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        public override void Update()
        {
            base.Update();
            if (modelRenderer)
            {
                propertyBlock.SetFloat("_EmPower", Mathf.Lerp(initialEmission, 0, age / (bodyPreservationDuration - 0.1f)));
                modelRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }
            if (fixedAge >= effectTime && !effectSpawned)
            {
                EffectManager.SimpleMuzzleFlash(smokeBombPrefab, characterBody.gameObject, "Body", false);
                effectSpawned = true;
            }
        }
    }
}
