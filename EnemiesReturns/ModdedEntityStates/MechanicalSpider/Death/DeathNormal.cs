using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    public class DeathNormal : GenericCharacterDeath
    {
        public static GameObject smokeBombPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();

        public static float effectTime = 0.54f;

        private bool effectSpawned = false;

        public override void OnEnter()
        {
            bodyPreservationDuration = 1f;
            base.OnEnter();
            Util.PlaySound("ER_Spider_Death_Normal_Play", base.gameObject);
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
