using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using EntityStates;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Hunter
{
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject characterLandImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/CharacterLandImpact.prefab").WaitForCompletion();

        public static float deathEffectDuration = 0.33f;

        private Transform deathEffectOrigin;

        private bool spawnedDeathEffect;

        public override void OnEnter()
        {
            bodyPreservationDuration = 1f;
            base.OnEnter();
            if (isVoidDeath)
            {
                return;
            }

            deathEffectOrigin = FindModelChild("DeathImpactOrigin");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isVoidDeath)
            {
                return;
            }

            if (fixedAge > deathEffectDuration && !spawnedDeathEffect)
            {
                if (deathEffectOrigin && characterLandImpactEffect)
                {
                    EffectManager.SpawnEffect(characterLandImpactEffect, new EffectData
                    {
                        origin = deathEffectOrigin.position,
                        scale = 1.5f
                    }, false);
                }
                spawnedDeathEffect = true;
            }
        }

    }
}
