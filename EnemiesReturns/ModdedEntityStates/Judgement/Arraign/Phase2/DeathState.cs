using EnemiesReturns.Reflection;
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace EnemiesReturns.ModdedEntityStates.Judgement.Arraign.Phase2
{
    [RegisterEntityState]
    public class DeathState : GenericCharacterDeath
    {
        private AsyncOperationHandle<GameObject> handle;

        public override void OnEnter()
        {
            bodyPreservationDuration = 20f;
            base.OnEnter();
            var handle = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Brother.BrotherDeathEffect_prefab);
            if (handle.IsValid())
            {
                handle.Completed += (operationResult) =>
                {
                    if (operationResult.Status == AsyncOperationStatus.Succeeded)
                    {
                        EffectManager.SimpleMuzzleFlash(operationResult.Result, base.gameObject, "MuzzleCenter", transmit: false);
                    }
                    Addressables.Release(handle);
                };
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
