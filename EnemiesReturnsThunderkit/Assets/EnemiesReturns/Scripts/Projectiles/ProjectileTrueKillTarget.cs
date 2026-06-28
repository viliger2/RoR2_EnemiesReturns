using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EnemiesReturns.Projectiles
{
    [RequireComponent(typeof(ProjectileTargetComponent))]
    public class ProjectileTrueKillTarget : MonoBehaviour
    {
        public bool dropEliteEquipment;

        public bool spawnEffect;

        public AssetReferenceT<GameObject> addressableEffectPrefab;

        public GameObject effectPrefab;

        private ProjectileTargetComponent targetComponent;

        private ProjectileController controller;

        private void Awake()
        {
            targetComponent = GetComponent<ProjectileTargetComponent>();
            controller = GetComponent<ProjectileController>();
        }

        public void SlayTarget()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if(!targetComponent || !targetComponent.target)
            {
                return;
            }

            var hurtBox = targetComponent.target.GetComponent<HurtBox>();
            if (!hurtBox || !hurtBox.healthComponent || !hurtBox.healthComponent.body)
            {
                return;
            }

            var body = hurtBox.healthComponent.body;

            if (!body.master)
            {
                return;
            }

            var hurtBoxPosition = hurtBox.transform.position;
            var rotation = Vector3.zero;
            if (controller)
            {
                rotation = (hurtBoxPosition - controller.owner.transform.position).normalized;
            }

            if (dropEliteEquipment && body.isElite && body.inventory)
            {
                var equipment = body.inventory.GetActiveEquipment();
                if (equipment.equipmentDef && equipment.equipmentDef.passiveBuffDef)
                {
                    PickupDropletController.CreatePickupDroplet(new UniquePickup(PickupCatalog.FindPickupIndex(equipment.equipmentDef.equipmentIndex)), hurtBoxPosition, rotation * 15f, false);
                }
            }

            if (spawnEffect)
            {
                GameObject effect = null;
                if (addressableEffectPrefab.RuntimeKeyIsValid())
                {
                    effect = addressableEffectPrefab.LoadAssetAsync().WaitForCompletion();
                } else if (effectPrefab)
                {
                    effect = effectPrefab;
                }

                //if (hurtBox.hurtBoxGroup.TryGetComponent<CharacterModel>(out var charModel))
                //{
                //    TemporaryOverlayInstance temporaryOverlayInstance = TemporaryOverlayManager.AddOverlay(charModel.gameObject);
                //    temporaryOverlayInstance.duration = 0.1f;
                //    temporaryOverlayInstance.animateShaderAlpha = true;
                //    temporaryOverlayInstance.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                //    temporaryOverlayInstance.destroyComponentOnEnd = true;
                //    temporaryOverlayInstance.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                //    temporaryOverlayInstance.AddToCharacterModel(charModel);
                //}

                if (effect)
                {
                    //GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BossHunterKillEffect");
                    EffectManager.SpawnEffect(effect, new EffectData
                    {
                        origin = hurtBoxPosition,
                        rotation = Util.QuaternionSafeLookRotation(rotation, Vector3.up)
                    }, transmit: true);
                }
            }

            body.master.TrueKill(controller.owner);
        }
    }
}
