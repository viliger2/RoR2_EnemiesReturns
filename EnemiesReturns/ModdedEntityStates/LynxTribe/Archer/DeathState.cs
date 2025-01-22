using EntityStates;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace EnemiesReturns.ModdedEntityStates.LynxTribe.Archer
{
    public class DeathState : GenericCharacterDeath
    {
        public static GameObject characterLandImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/CharacterLandImpact.prefab").WaitForCompletion();

        public static float deathEffectDuration = 0.625f;

        private Transform deathEffectOrigin;

        private bool spawnedDeathEffect;

        public override void OnEnter()
        {
            bodyPreservationDuration = 1.2f;
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

            if(fixedAge > deathEffectDuration && !spawnedDeathEffect)
            {
                if(deathEffectOrigin && characterLandImpactEffect)
                {
                    EffectManager.SpawnEffect(characterLandImpactEffect, new EffectData
                    {
                        origin = deathEffectOrigin.position,
                        scale = 2f
                    }, false);
                }
                spawnedDeathEffect = true;
            }
        }

    }
}
