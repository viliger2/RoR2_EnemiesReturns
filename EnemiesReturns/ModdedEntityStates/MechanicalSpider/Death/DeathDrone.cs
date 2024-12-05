using EnemiesReturns.Enemies.MechanicalSpider;
using EnemiesReturns.Behaviors;
using EntityStates;
using RoR2;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EnemiesReturns.ModdedEntityStates.MechanicalSpider.Death
{
    internal class DeathDrone : GenericCharacterDeath
    {
        public static float deathDuration = 4f;

        private bool startedGrounded = true;

        private bool droneSpawned = false;

        public override void OnEnter()
        {
            base.OnEnter();
            if (isVoidDeath)
            {
                return;
            }
            if (characterMotor)
            {
                if (characterMotor.isGrounded)
                {
                    if (NetworkServer.active)
                    {
                        SpawnDrone(base.characterBody.footPosition);
                        Explode();
                    }
                    DestroyModel();
                }
                else
                {
                    startedGrounded = false;
                }
            }

            Util.PlaySound("ER_Spider_Death_Drone_Play", gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!startedGrounded && characterMotor && characterMotor.isGrounded)
            {
                if (NetworkServer.active && !droneSpawned)
                {
                    SpawnDrone(base.characterBody.footPosition);
                    Explode();
                    droneSpawned = true;
                }
                DestroyModel();
            }
            if (fixedAge >= deathDuration && NetworkServer.active)
            {
                Explode();
            }
        }

        public void Explode()
        {
            EntityState.Destroy(base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            Explode();
            DestroyModel();
        }

        private void SpawnDrone(Vector3 spawnPosition)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            var placementRule = new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.Direct,
                position = spawnPosition
            };

            var result = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(MechanicalSpiderStuff.SpawnCards.iscMechanicalSpiderBroken, placementRule, RoR2Application.rng));
            if (result)
            {
                var inventory = result.GetComponent<Inventory>();
                if (inventory && characterBody.inventory)
                {
                    inventory.CopyEquipmentFrom(characterBody.inventory);
                    inventory.CopyItemsFrom(characterBody.inventory);
                    DeleteMinionItems(inventory);
                }
                var eliteDef = inventory.GetEquipment(0).equipmentDef?.passiveBuffDef?.eliteDef ?? null;
                var setEliteRamp = result.GetComponent<SetEliteRampOnShader>();
                if (setEliteRamp && inventory)
                {
                    setEliteRamp.SetEliteRampIndex(eliteDef?.shaderEliteRampIndex ?? -1, eliteDef?.eliteIndex ?? EliteIndex.None); // surely, SURELY it won't break
                }
                var purchaseInteraction = result.GetComponent<PurchaseInteraction>();
                if (purchaseInteraction && purchaseInteraction.costType == CostTypeIndex.Money && inventory)
                {
                    float eliteModifier = 1f;
                    if (eliteDef)
                    {
                        var eliteTier = CombatDirector.eliteTiers.First(tier => tier.eliteTypes.Contains(eliteDef));
                        eliteModifier = eliteTier.costMultiplier * EnemiesReturns.Configuration.MechanicalSpider.DroneEliteConstMultiplier.Value;
                    }
                    purchaseInteraction.Networkcost = Run.instance.GetDifficultyScaledCost((int)(purchaseInteraction.cost * eliteModifier));
                }
            }
        }

        private void DeleteMinionItems(Inventory inventory)
        {
            // this entire thing so we don't remove elite bonus stats from the body
            int bonusHpToRemove = EnemiesReturns.Configuration.MechanicalSpider.DroneBonusHP.Value;
            int bonusDamageToRemove = EnemiesReturns.Configuration.MechanicalSpider.DroneBonusDamage.Value;
            var equipment = inventory.GetEquipment(0).equipmentDef;
            if (equipment && equipment.passiveBuffDef && equipment.passiveBuffDef.eliteDef)
            {
                bonusHpToRemove = Mathf.Min(inventory.GetItemCount(RoR2Content.Items.BoostHp) - ConvertCoefficientToItemCount(equipment.passiveBuffDef.eliteDef.healthBoostCoefficient), 0);
                bonusDamageToRemove = Mathf.Min(inventory.GetItemCount(RoR2Content.Items.BoostDamage) - ConvertCoefficientToItemCount(equipment.passiveBuffDef.eliteDef.damageBoostCoefficient), 0);
            }

            inventory.RemoveItem(RoR2Content.Items.MinionLeash, inventory.GetItemCount(RoR2Content.Items.MinionLeash));
            inventory.RemoveItem(RoR2Content.Items.BoostHp, bonusHpToRemove);
            inventory.RemoveItem(RoR2Content.Items.BoostDamage, bonusDamageToRemove);
            if (ModCompats.RiskyModCompat.enabled)
            {
                inventory.RemoveItem(ModCompats.RiskyModCompat.RiskyModAllyScaling, inventory.GetItemCount(ModCompats.RiskyModCompat.RiskyModAllyScaling));
                inventory.RemoveItem(ModCompats.RiskyModCompat.RiskyModAllyMarker, inventory.GetItemCount(ModCompats.RiskyModCompat.RiskyModAllyMarker));
                inventory.RemoveItem(ModCompats.RiskyModCompat.RiskyModAllyRegen, inventory.GetItemCount(ModCompats.RiskyModCompat.RiskyModAllyRegen));
            }
        }

        private int ConvertCoefficientToItemCount(float coefficient)
        {
            return Mathf.RoundToInt((coefficient - 1f) * 10f);
        }
    }
}
