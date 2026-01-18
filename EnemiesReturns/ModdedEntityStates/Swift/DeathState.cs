using EnemiesReturns.Reflection;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EnemiesReturns.ModdedEntityStates.Swift
{
    [RegisterEntityState]
    public class DeathState : EntityStates.Vulture.FallingDeath
    {
        public static GameObject impactEffect;

        private bool wasGrounded = false;

        public override void OnEnter()
        {
            base.OnEnter();
            wasGrounded = base.characterMotor.isGrounded;
            if (!impactEffect)
            {
                var handler = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_BeetleGuard.BeetleGuardDeathImpact_prefab);
                if (handler.IsValid())
                {
                    handler.Completed += (operationResult) =>
                    {
                        if (handler.IsDone && handler.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        {
                            impactEffect = handler.Result;
                            if (wasGrounded)
                            {
                                EffectManager.SimpleEffect(impactEffect, transform.position, Quaternion.identity, false);
                            }
                        }
                        Addressables.Release(handler);
                    };
                }
            }
            else
            {
                if (wasGrounded)
                {
                    EffectManager.SimpleEffect(impactEffect, transform.position, Quaternion.identity, false);
                }
            }

            var stones1 = FindModelChild("Stones1");
            if (stones1)
            {
                stones1.gameObject.SetActive(false);
            }

            var stones2 = FindModelChild("Stones2");
            if (stones2)
            {
                stones2.gameObject.SetActive(false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!wasGrounded && characterMotor.isGrounded)
            {
                if (impactEffect)
                {
                    EffectManager.SimpleEffect(impactEffect, transform.position, Quaternion.identity, false);
                }
                wasGrounded = true;
            }
        }
    }
}
