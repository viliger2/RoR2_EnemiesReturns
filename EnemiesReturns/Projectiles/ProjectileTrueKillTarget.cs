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

        private EquipmentDef cachedEquipment;

        private void Awake()
        {
            targetComponent = GetComponent<ProjectileTargetComponent>();
            controller = GetComponent<ProjectileController>();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (!dropEliteEquipment)
            {
                return;
            }

            if (!cachedEquipment)
            {
                cachedEquipment = GetTargetEquipment(GetTargetBody());
            }
        }

        public void SlayTarget()
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var body = GetTargetBody();
            if (body)
            {
                if (dropEliteEquipment)
                {
                    var equipmentDef = GetTargetEquipment(body);
                    if (equipmentDef)
                    {
                        var hurtBoxPosition = body.corePosition;
                        var rotation = Vector3.zero;
                        if (controller)
                        {
                            rotation = (hurtBoxPosition - controller.owner.transform.position).normalized;
                        }

                        PickupDropletController.CreatePickupDroplet(new UniquePickup(PickupCatalog.FindPickupIndex(equipmentDef.equipmentIndex)), hurtBoxPosition, rotation * 15f, false);
                    }
                }

                if (spawnEffect)
                {
                    GameObject effect = null;
                    if (addressableEffectPrefab.RuntimeKeyIsValid())
                    {
                        effect = addressableEffectPrefab.LoadAssetAsync().WaitForCompletion();
                    }
                    else if (effectPrefab)
                    {
                        effect = effectPrefab;
                    }

                    if (effect)
                    {
                        EffectManager.SpawnEffect(effect, new EffectData
                        {
                            origin = body.corePosition,
                            rotation = Quaternion.identity
                        }, transmit: true);
                    }
                }

                body.master.TrueKill(controller.owner);
            } else
            {
                if (cachedEquipment && dropEliteEquipment)
                {
                    PickupDropletController.CreatePickupDroplet(new UniquePickup(PickupCatalog.FindPickupIndex(cachedEquipment.equipmentIndex)), this.transform.position, Vector3.zero, false);
                }
            }
        }

        private EquipmentDef GetTargetEquipment(CharacterBody body)
        {
            if (!body.master)
            {
                return null;
            }

            if (body.isElite && body.inventory)
            {
                var equipment = body.inventory.GetActiveEquipment();
                if (equipment.equipmentDef && equipment.equipmentDef.passiveBuffDef)
                {
                    return equipment.equipmentDef;
                }
            }

            return null;
        }

        private CharacterBody GetTargetBody()
        {
            if (!targetComponent || !targetComponent.target)
            {
                return null;
            }

            var hurtBox = targetComponent.target.GetComponent<HurtBox>();
            if (!hurtBox || !hurtBox.healthComponent || !hurtBox.healthComponent.body)
            {
                return null;
            }

            return hurtBox.healthComponent.body;
        }

    }
}
