using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EnemiesReturns.Behaviors.ContactLight.GaussCannon
{
    [RequireComponent(typeof(PurchaseInteraction))]
    public class FireBullet : NetworkBehaviour
    {
        public float damageCoefficient;

        public float bulletRadius;

        public AssetReferenceT<GameObject> tracerPrefabReference;

        public AssetReferenceT<GameObject> hitEffectPrefabReference;

        public Transform aimOrigin;

        public int maxPurchaseCount;

        public float refreshDuration;

        private int purchaseCount;

        private float refreshTimer;

        private bool waitingForRefresh;

        private GameObject tracerPrefab;

        private GameObject hitEffectPrefab;

        private PurchaseInteraction purchaseInteraction;

        private void Awake()
        {
            purchaseInteraction = GetComponent<PurchaseInteraction>();

            var handle = tracerPrefabReference.LoadAssetAsync();
            handle.Completed += (result) =>
            {
                if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    tracerPrefab = result.Result;
                    Addressables.Release(handle);
                }
            };

            var handl2 = hitEffectPrefabReference.LoadAssetAsync();
            handl2.Completed += (result) =>
            {
                if (result.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    hitEffectPrefab = result.Result;
                    Addressables.Release(handle);
                }
            };
        }

        private void FixedUpdate()
        {
            if (waitingForRefresh)
            {
                refreshTimer -= Time.fixedDeltaTime;
                if (refreshTimer <= 0f && purchaseCount < maxPurchaseCount)
                {
                    purchaseInteraction.SetAvailable(newAvailable: true);
                    waitingForRefresh = false;
                }
            }
        }

        public void AddStack(Interactor interactor)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!interactor)
            {
                return;
            }

            if (!interactor.TryGetComponent<CharacterBody>(out var characterBody))
            {
                return;
            }

            BulletAttack bulletAttack = new BulletAttack();
            bulletAttack.owner = characterBody.gameObject;
            bulletAttack.weapon = characterBody.gameObject;
            bulletAttack.procCoefficient = 0f;
            bulletAttack.origin = aimOrigin.position;
            bulletAttack.aimVector = aimOrigin.forward;
            bulletAttack.minSpread = 0;
            bulletAttack.maxSpread = 0;
            bulletAttack.bulletCount = 1u;
            bulletAttack.damage = damageCoefficient * characterBody.damage;
            bulletAttack.force = 0;
            bulletAttack.tracerEffectPrefab = tracerPrefab;
            bulletAttack.hitEffectPrefab = hitEffectPrefab;
            bulletAttack.isCrit = Util.CheckRoll(characterBody.crit, characterBody.master);
            bulletAttack.radius = bulletRadius;
            bulletAttack.smartCollision = true;
            bulletAttack.stopperMask = LayerIndex.world.mask;
            bulletAttack.hitMask = LayerIndex.entityPrecise.mask;
            bulletAttack.damageType.AddModdedDamageType(Content.DamageTypes.EndGameBossWeapon);
            bulletAttack.Fire();

            purchaseCount++;
            if (purchaseCount >= maxPurchaseCount)
            {
                // do something, I dunoo
            }
            else
            {
                refreshTimer += refreshDuration;
                waitingForRefresh = true;
            }

        }

    }
}
