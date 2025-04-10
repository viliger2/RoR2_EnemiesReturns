using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Totem
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject lemurianBruiserDeathEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBruiserDeathImpact.prefab").WaitForCompletion();

        public static GameObject characterLandImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/CharacterLandImpact.prefab").WaitForCompletion();

        public static GameObject shakeEffect;

        public static float shamanDeath = 1.875f;

        public static float topPartDeath = 2.208f;

        public static float middlePartDeath = 2.5f;

        private Transform shamanPosition;

        private Transform topPartPosition;

        private Transform middlePartPosition;

        private bool shamanDeathSpawned;

        private bool topDeathSpawned;

        private bool middleDeathSpawned;

        public override void OnEnter()
        {
            bodyPreservationDuration = 3f;
            base.OnEnter();
            if (isVoidDeath)
            {
                return;
            }

            var childLocator = GetModelChildLocator();
            shamanPosition = childLocator.FindChild("ShamanDeathSpot");
            topPartPosition = childLocator.FindChild("TopParthDeathSpot");
            middlePartPosition = childLocator.FindChild("MiddleParthDeathSpot");

            var shakeEffectTransform = childLocator.FindChild("ShakeEffect");
            if (shakeEffectTransform && shakeEffect)
            {
                EffectManager.SimpleEffect(shakeEffect, shakeEffectTransform.position, shakeEffectTransform.rotation, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }
            if (fixedAge > shamanDeath && !shamanDeathSpawned)
            {
                if (characterLandImpactEffect && shamanPosition)
                {
                    EffectManager.SpawnEffect(characterLandImpactEffect, new EffectData
                    {
                        origin = shamanPosition.position,
                        scale = 2f
                    }, false);
                }
                shamanDeathSpawned = true;
            }
            if (fixedAge > topPartDeath && !topDeathSpawned)
            {
                if (lemurianBruiserDeathEffect && topPartPosition)
                {
                    EffectManager.SimpleEffect(lemurianBruiserDeathEffect, topPartPosition.position, Quaternion.identity, false);
                }
                topDeathSpawned = true;
            }
            if (fixedAge > middlePartDeath && !middleDeathSpawned)
            {
                if (lemurianBruiserDeathEffect && middlePartPosition)
                {
                    EffectManager.SimpleEffect(lemurianBruiserDeathEffect, middlePartPosition.position, Quaternion.identity, false);
                }
                middleDeathSpawned = true;
            }
        }

    }
}
